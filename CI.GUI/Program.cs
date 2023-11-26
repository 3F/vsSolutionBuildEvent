/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * This component (part of vsSolutionBuildEvent) is licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using net.r_eg.vsSBE.UI.WForms;

namespace net.r_eg.vsSBE.CI.GUI
{
    internal static class Program
    {
        [STAThread]
        internal static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += (sender, e) => Fail(e.Exception);

            try
            {
                API.EventLevel evl = new();
                evl.load
                (
                    GetSln(args),
                    new Dictionary<string, string>(),
                    Settings._.Config.Sys.Data?.DebugMode ??
#if DEBUG
                    true
#else
                    false
#endif
                );

                var pUnkReserved = new object();
                evl.solutionOpened(pUnkReserved, 0);
                {
                    Application.Run(new EventsFrm(Bootloader.Init(evl.Environment)));
                }
                evl.solutionClosed(pUnkReserved);
            }
            catch(Exception ex)
            {
                Fail(ex);
            }
        }

        private static string GetSln(string[] args) => Sln.FindFullPath(args?.Length < 1 ? null : args[0]);

        private static void Fail(Exception ex) => Console.Error.WriteLine($"Failed due to:\n\"{ex.Message}\"\n\nStack trace:\n{ex.StackTrace}");
    }
}
