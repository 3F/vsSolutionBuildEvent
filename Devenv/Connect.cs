/*
 * The MIT License (MIT)
 * 
 * Copyright (c) 2013-2015  Denis Kuzmin (reg) <entry.reg@gmail.com>
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
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

namespace net.r_eg.vsSBE.Devenv
{
    public sealed class Connect: IDTExtensibility2, IVsSolutionEvents, IVsUpdateSolutionEvents2
    {
        /// <summary>
        /// Our the vsSolutionBuildEvent library
        /// </summary>
        private Provider.ILibrary library;

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
        private DTE2 _dte2;

        /// <summary>
        /// Representing this Add-in.
        /// </summary>
        private AddIn _addIn;

        /// <summary>
        /// Loader of main library
        /// </summary>
        private Provider.ILoader loader;


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
            return library.Event.onPre(ref pfCancelUpdate);
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
            return library.Event.onProjectPre(pHierProj, pCfgProj, pCfgSln, dwAction, ref pfCancel);
        }

        public int UpdateProjectCfg_Done(IVsHierarchy pHierProj, IVsCfg pCfgProj, IVsCfg pCfgSln, uint dwAction, int fSuccess, int fCancel)
        {
            return library.Event.onProjectPost(pHierProj, pCfgProj, pCfgSln, dwAction, fSuccess, fCancel);
        }

        #endregion

        /// <summary>Receives notification that the Add-in is being loaded.</summary>
        /// <param term='application'>Root object of the host application.</param>
        /// <param term='connectMode'>Describes how the Add-in is being loaded.</param>
        /// <param term='addInInst'>Object representing this Add-in.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnConnection(object application, ext_ConnectMode connectMode, object addInInst, ref Array custom)
        {

//#if !DEBUG
            if(connectMode != ext_ConnectMode.ext_cm_CommandLine) {
                msg("[Ignored] Allowed only Command line mode /'{0}'", connectMode);
                return;
            }
//#endif

            loader                      = new Provider.Loader();
            loader.Settings.DebugMode   = Environment.GetCommandLineArgs().Contains("verbosity:diagnostic");
            _dte2                       = (DTE2)application;
            _addIn                      = (AddIn)addInInst;

            try
            {
                library = loader.load(application, _addIn.SatelliteDllPath, _dte2.RegistryRoot);
                msg("Library: loaded from '{0}' :: v{1} [{2}] /'{3}':{4}", 
                                                    library.Dllpath, 
                                                    library.Version.Number.ToString(), 
                                                    library.Version.BranchSha1,
                                                    library.Version.BranchName,
                                                    library.Version.BranchRevCount);

                updateBuildType(Environment.GetCommandLineArgs());
                adviseEvents();
            }
            catch(DllNotFoundException ex)
            {
                msg(ex.Message);
                msg("You can install vsSolutionBuildEvent as plugin for this Visual Studio v{0} or manually place the {1} into the current add-in folder",
                    _dte2.Version,
                    "vsSolutionBuildEvent.dll with dependencies");

                msg("https://visualstudiogallery.msdn.microsoft.com/0d1dbfd7-ed8a-40af-ae39-281bfeca2334/");
                msg("Minimum requirements: vsSolutionBuildEvent.dll v{0}", loader.MinVersion.ToString());
                msg(new String('=', 80));
            }
            catch(ReflectionTypeLoadException ex)
            {
                foreach(FileNotFoundException le in ex.LoaderExceptions) {
                    msg("{2} {0}{3} {0}{0}{4} {0}{1}",
                                        Environment.NewLine, new String('~', 80),
                                        le.FileName, le.Message, le.FusionLog);
                }
            }
            catch(Exception ex) {
                msg("Error with advising '{0}'", ex.ToString());
            }
        }

        /// <summary>Receives notification that the Add-in is being unloaded.</summary>
        /// <param term='disconnectMode'>Describes how the Add-in is being unloaded.</param>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnDisconnection(ext_DisconnectMode disconnectMode, ref Array custom)
        {
            unadviseEvents();
        }

        public Connect()
        {
            msg(new String('=', 60));
            msg("[[ vsSolutionBuildEvent Devenv Command-Line ]] Welcomes You!");
            msg(new String('=', 60));
            msg("Version: v{0}", System.Diagnostics.FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion);
            msg("Feedback: entry.reg@gmail.com");
            msg(new String('_', 60));
        }


        /// <summary>
        /// Defines listeners for main events.
        /// </summary>
        private void adviseEvents()
        {
            // To listen events fires from IVsSolutionEvents
            spSolution = (IVsSolution)ServiceProvider.GlobalProvider.GetService(typeof(SVsSolution));
            spSolution.AdviseSolutionEvents(this, out _pdwCookieSolution);

            // To listen events fires from IVsUpdateSolutionEvents2
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
        /// Updates the type by command-line switches if it exists in BuildType list
        /// https://msdn.microsoft.com/en-us/library/vstudio/xee0c8y7.aspx
        /// </summary>
        /// <param name="switches">command-line switches</param>
        private void updateBuildType(string[] switches)
        {
            foreach(string type in switches.Where(p => p.StartsWith("/")).Select(p => p.Substring(1)))
            {
                debug("updateBuildType: check '{0}'", type);
                if(Enum.IsDefined(typeof(BuildType), type))
                {
                    BuildType buildType = (BuildType)Enum.Parse(typeof(BuildType), type);
                    library.Build.updateBuildType(buildType);
                    debug("updateBuildType: updated as a '{0}'", buildType);
                    return;
                }
            }
        }

        /// <summary>
        /// TODO: logger
        /// </summary>
        /// <param name="data"></param>
        /// <param name="args"></param>
        private void msg(string data, params object[] args)
        {
            Console.WriteLine(data, args);
        }

        /// <summary>
        /// TODO: logger
        /// </summary>
        /// <param name="data"></param>
        /// <param name="args"></param>
        private void debug(string data, params object[] args)
        {
            if(loader.Settings.DebugMode) {
                msg(data, args);
            }
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