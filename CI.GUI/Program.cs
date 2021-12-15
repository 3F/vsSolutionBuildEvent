/*
 * The MIT License (MIT)
 * 
 * Copyright (c) 2013-2021  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent
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
                var evl = new API.EventLevel();
                evl.load(GetSln(args), new Dictionary<string, string>());

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
