/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * This component (part of vsSolutionBuildEvent) is licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using EnvDTE;
using EnvDTE80;
using Extensibility;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using net.r_eg.vsSBE.Bridge;
using net.r_eg.vsSBE.Provider;

namespace net.r_eg.vsSBE.Devenv
{
    /// <summary>
    /// 
    /// ! https://connect.microsoft.com/VisualStudio/feedback/details/1075033/
    ///   https://bitbucket.org/3F/vssolutionbuildevent/issues/25/#comment-14586721
    ///   http://vssbe.r-eg.net/doc/Scheme/
    ///   
    /// Alternative variant with MSBuild: 
    ///   http://vssbe.r-eg.net/doc/CI/CI.MSBuild/
    /// </summary>
    public sealed class Connect: IDTExtensibility2, IVsSolutionEvents, IVsUpdateSolutionEvents2
    {
        /// <summary>
        /// Used logger
        /// </summary>
        internal ILog log;

        /// <summary>
        /// Our vsSolutionBuildEvent library
        /// </summary>
        private Provider.ILibrary library;

        /// <summary>
        /// Support of the core commands.
        /// </summary>
        private CoreCommand coreCommand;

        /// <summary>
        /// For IVsSolutionEvents events
        /// http://msdn.microsoft.com/en-us/library/microsoft.visualstudio.shell.interop.ivssolution.aspx
        /// </summary>
        private IVsSolution spSolution;

        /// <summary>
        /// Contains the cookie for advising IVsSolution
        /// http://msdn.microsoft.com/en-us/library/microsoft.visualstudio.shell.interop.ivssolution.advisesolutionevents.aspx
        /// </summary>
        private uint _pdwCookieSolution;

        /// <summary>
        /// For IVsUpdateSolutionEvents2 events
        /// http://msdn.microsoft.com/en-us/library/microsoft.visualstudio.shell.interop.ivssolutionbuildmanager2.aspx
        /// </summary>
        private IVsSolutionBuildManager2 spSolutionBM;

        /// <summary>
        /// Contains the cookie for advising IVsSolutionBuildManager2 / IVsSolutionBuildManager
        /// http://msdn.microsoft.com/en-us/library/bb141335.aspx
        /// </summary>
        private uint _pdwCookieSolutionBM;

        /// <summary>
        /// DTE2 Context from the root object of the host application.
        /// </summary>
        private DTE2 dte2;

        /// <summary>
        /// Representing this Add-in.
        /// </summary>
        private AddIn addIn;


        #region pingpong

        public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
        {
            return library.Event.solutionOpened(pUnkReserved, fNewSolution);
        }

        public int OnAfterCloseSolution(object pUnkReserved)
        {
            return library.Event.solutionClosed(pUnkReserved);
        }

        public int UpdateSolution_Begin(ref int pfCancelUpdate)
        {
            try {
                return library.Event.onPre(ref pfCancelUpdate);
            }
            finally {
                termination();
            }
        }

        public int UpdateSolution_Cancel()
        {
            return library.Event.onCancel();
        }

        public int UpdateSolution_Done(int fSucceeded, int fModified, int fCancelCommand)
        {
            return library.Event.onPost(fSucceeded, fModified, fCancelCommand);
        }

        public int UpdateProjectCfg_Begin(IVsHierarchy pHierProj, IVsCfg pCfgProj, IVsCfg pCfgSln, uint dwAction, ref int pfCancel)
        {
            try {
                return library.Event.onProjectPre(pHierProj, pCfgProj, pCfgSln, dwAction, ref pfCancel);
            }
            finally {
                termination();
            }
        }

        public int UpdateProjectCfg_Done(IVsHierarchy pHierProj, IVsCfg pCfgProj, IVsCfg pCfgSln, uint dwAction, int fSuccess, int fCancel)
        {
            try {
                return library.Event.onProjectPost(pHierProj, pCfgProj, pCfgSln, dwAction, fSuccess, fCancel);
            }
            finally {
                termination();
            }
        }

        #endregion

        /// <summary>Receives notification that the Add-in is being loaded.</summary>
        /// <param term='application'>Root object of the host application.</param>
        /// <param term='connectMode'>Describes how the Add-in is being loaded.</param>
        /// <param term='addInInst'>Object representing this Add-in.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnConnection(object application, ext_ConnectMode connectMode, object addInInst, ref Array custom)
        {
            if(connectMode != ext_ConnectMode.ext_cm_CommandLine) {
                log.info("[Ignored] Allowed only Command-line mode /'{0}'", connectMode);
                return;
            }

            dte2    = (DTE2)application;
            addIn   = (AddIn)addInInst;

            ILoader loader = new Loader(
                                    new Provider.Settings()
                                    {
                                        DebugMode = log.IsDiagnostic,
                                        LibSettings = new LibSettings()
                                        {
                                            DebugMode = log.IsDiagnostic,
                                        },
                                    }
                                );

            init(loader, dte2, addIn);
        }

        /// <summary>Receives notification that the Add-in is being unloaded.</summary>
        /// <param term='disconnectMode'>Describes how the Add-in is being unloaded.</param>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnDisconnection(ext_DisconnectMode disconnectMode, ref Array custom)
        {
            unadviseEvents();
            coreCommand.detachCoreCommandListener();
        }

        public Connect()
        {
            log = new Log(Environment.GetCommandLineArgs().Contains("verbosity:diagnostic"));
            header();
        }

        /// <summary>
        /// Initialize library
        /// </summary>
        /// <param name="loader"></param>
        /// <param name="dte2"></param>
        /// <param name="addIn"></param>
        private void init(ILoader loader, DTE2 dte2, AddIn addIn)
        {
            try
            {
                library = loader.load(dte2, addIn.SatelliteDllPath, dte2.RegistryRoot);
                log.info("Library: loaded from '{0}' :: v{1} [{2}] API: v{3} /'{4}':{5}", 
                                                    library.Dllpath, 
                                                    library.Version.Number.ToString(), 
                                                    library.Version.BranchSha1,
                                                    library.Version.Bridge.Number.ToString(2),
                                                    library.Version.BranchName,
                                                    library.Version.BranchRevCount);


                coreCommand = new CoreCommand(library);
                coreCommand.attachCoreCommandListener();
                
                updateBuildType(Environment.GetCommandLineArgs());
                adviseEvents();
                return;
            }
            catch(DllNotFoundException ex)
            {
                log.info(ex.Message);
                log.info(new String('.', 80));
                log.info("How about:");

                log.info("");
                log.info("* Install vsSolutionBuildEvent as plugin for your Visual Studio v{0}", dte2.Version);
                log.info("* Or manually place the 'vsSolutionBuildEvent.dll' with dependencies into AddIn folder: '{0}\\'", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
                log.info("");

                log.info("See documentation for more details:");
                log.info("- http://vssbe.r-eg.net");
                log.info("- http://visualstudiogallery.msdn.microsoft.com/0d1dbfd7-ed8a-40af-ae39-281bfeca2334/");
                log.info("");

                log.info("Minimum requirements: vsSolutionBuildEvent.dll v{0}", loader.MinVersion.ToString());
                log.info(new String('.', 80));
            }
            catch(ReflectionTypeLoadException ex)
            {
                log.info(ex.ToString());
                log.info(new String('.', 80));

                foreach(FileNotFoundException le in ex.LoaderExceptions) {
                    log.info("{2} {0}{3} {0}{0}{4} {0}{1}",
                                        Environment.NewLine, 
                                        new String('~', 80),
                                        le.FileName, 
                                        le.Message, 
                                        le.FusionLog);
                }
            }
            catch(Exception ex) {
                log.info("Error with advising '{0}'", ex.ToString());
            }

            termination(true);
        }

        /// <summary>
        /// Defines listeners for main events.
        /// </summary>
        private void adviseEvents()
        {
            // To listen events that fired as a IVsSolutionEvents
            spSolution = (IVsSolution)ServiceProvider.GlobalProvider.GetService(typeof(SVsSolution));
            spSolution.AdviseSolutionEvents(this, out _pdwCookieSolution);

            // To listen events that fired as a IVsUpdateSolutionEvents2
            spSolutionBM = (IVsSolutionBuildManager2)ServiceProvider.GlobalProvider.GetService(typeof(SVsSolutionBuildManager));
            spSolutionBM.AdviseUpdateSolutionEvents(this, out _pdwCookieSolutionBM);
        }

        private void unadviseEvents()
        {
            if(spSolutionBM != null && _pdwCookieSolutionBM != 0) {
                spSolutionBM.UnadviseUpdateSolutionEvents(_pdwCookieSolutionBM);
            }

            if(spSolution != null && _pdwCookieSolution != 0) {
                spSolution.UnadviseSolutionEvents(_pdwCookieSolution);
            }
        }

        /// <summary>
        /// Logic of termination of the devenv job
        /// </summary>
        /// <param name="force">Terminate manually if true.</param>
        private void termination(bool force = false)
        {
            if(!force && (coreCommand == null || !coreCommand.IsAborted)) {
                return;
            }
            
            //TODO: Another way ? fix me
            //throw new AbortException(); // not effective for devenv case. See also in CIM

            log.info("The build has been canceled{0}.", (force)? "" : " by user script");
            Environment.Exit(VSConstants.S_FALSE); // we work in command line mode with devenv, stop it immediately
        }

        /// <summary>
        /// Updates the type by command-line switches if it exists in BuildType list
        /// https://msdn.microsoft.com/en-us/library/vstudio/xee0c8y7.aspx
        /// </summary>
        /// <param name="switches">command-line switches</param>
        private void updateBuildType(string[] switches)
        {
            foreach(string type in switches.Where(p => p.StartsWith("/")).Select(p => p.Substring(1)))
            {
                log.debug("updateBuildType: check '{0}'", type);
                if(Enum.IsDefined(typeof(BuildType), type))
                {
                    BuildType buildType = (BuildType)Enum.Parse(typeof(BuildType), type);
                    library.Build.updateBuildType(buildType);
                    log.debug("updateBuildType: updated as a '{0}'", buildType);
                    return;
                }
            }
        }

        private void header()
        {
            log.info(new String('=', 60));
            log.info("[[ vsSolutionBuildEvent Devenv ]] Welcomes You!");
            log.info(new String('=', 60));
            log.info("Version: v{0}", System.Diagnostics.FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion);
            log.info("Feedback: https://github.com/3F/vsSolutionBuildEvent");
            log.info(new String('_', 60));
        }


        #region unused

        /// <summary>Receives notification when the collection of Add-ins has changed.</summary>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />		
        public void OnAddInsUpdate(ref Array custom)
        {

        }

        /// <summary>Receives notification that the host application has completed loading.</summary>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnStartupComplete(ref Array custom)
        {

        }

        /// <summary>Receives notification that the host application is being unloaded.</summary>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnBeginShutdown(ref Array custom)
        {

        }

        public int UpdateSolution_StartUpdate(ref int pfCancelUpdate)
        {
            return VSConstants.S_OK;
        }

        public int OnActiveProjectCfgChange(IVsHierarchy pIVsHierarchy)
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved)
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnBeforeCloseSolution(object pUnkReserved)
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy)
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnQueryCloseSolution(object pUnkReserved, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        #endregion

    }
}