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
using System.Runtime.InteropServices;
using EnvDTE;
using EnvDTE80;

namespace net.r_eg.vsSBE.Provider.Services
{
    internal class _DTE2: EnvDTE80.DTE2, EnvDTE.DTE
    {
        [DispId(221)]
        public Document ActiveDocument { get; set; }
        [DispId(237)]
        public object ActiveSolutionProjects { get; set; }
        [DispId(205)]
        public Window ActiveWindow { get; set; }
        [DispId(200)]
        public AddIns AddIns { get; set; }
        [DispId(240)]
        public DTE Application { get; set; }
        [DispId(108)]
        public object CommandBars { get; set; }
        [DispId(214)]
        public string CommandLineArguments { get; set; }
        [DispId(210)]
        public Commands Commands { get; set; }
        [DispId(241)]
        public ContextAttributes ContextAttributes { get; set; }
        [DispId(244)]
        public Debugger Debugger { get; set; }
        [DispId(208)]
        public vsDisplay DisplayMode { get; set; }
        [DispId(220)]
        public Documents Documents { get; set; }
        [DispId(217)]
        public DTE DTE { get; set; }
        [DispId(246)]
        public string Edition { get; set; }
        [DispId(111)]
        public Events Events { get; set; }
        [DispId(10)]
        public string FileName { get; set; }
        [DispId(229)]
        public Find Find { get; set; }
        [DispId(226)]
        public string FullName { get; set; }
        [DispId(223)]
        public Globals Globals { get; set; }
        [DispId(233)]
        public ItemOperations ItemOperations { get; set; }
        [DispId(218)]
        public int LocaleID { get; set; }
        [DispId(236)]
        public Macros Macros { get; set; }
        [DispId(238)]
        public DTE MacrosIDE { get; set; }
        [DispId(204)]
        public Window MainWindow { get; set; }
        [DispId(230)]
        public vsIDEMode Mode { get; set; }
        [DispId(0)]
        public string Name { get; set; }
        [DispId(228)]
        public ObjectExtenders ObjectExtenders { get; set; }
        [DispId(239)]
        public string RegistryRoot { get; set; }
        [DispId(213)]
        public SelectedItems SelectedItems { get; set; }
        [DispId(209)]
        public Solution Solution { get; set; }
        [DispId(242)]
        public SourceControl SourceControl { get; set; }
        [DispId(225)]
        public StatusBar StatusBar { get; set; }
        [DispId(243)]
        public bool SuppressUI { get; set; }
        [DispId(300)]
        public ToolWindows ToolWindows { get; set; }
        [DispId(235)]
        public UndoContext UndoContext { get; set; }
        [DispId(227)]
        public bool UserControl { get; set; }
        [DispId(100)]
        public string Version { get; set; }
        [DispId(219)]
        public WindowConfigurations WindowConfigurations { get; set; }
        [DispId(110)]
        public Windows Windows { get; set; }

        [DispId(222)]
        public void ExecuteCommand(string CommandName, string CommandArgs = "")
        {
            Console.WriteLine("ExecuteCommand: Disabled for this DTE2. Command: '{0}', args: '{1}'", CommandName, CommandArgs);
        }

        [DispId(216)]
        [TypeLibFunc(64)]
        public bool get_IsOpenFile(string ViewKind, string FileName) { return false; }
        [DispId(212)]
        public Properties get_Properties(string Category, string Page) { return null; }
        [DispId(211)]
        public object GetObject(string Name) { return null; }
        [DispId(301)]
        public uint GetThemeColor(vsThemeColors Element) { return 0; }
        [DispId(232)]
        public wizardResult LaunchWizard(string VSZFile, ref object[] ContextParams) { return wizardResult.wizardResultSuccess; }
        [DispId(215)]
        [TypeLibFunc(64)]
        public Window OpenFile(string ViewKind, string FileName) { return null; }
        [DispId(207)]
        public void Quit() { }
        [DispId(245)]
        public string SatelliteDllPath(string Path, string Name) { return String.Empty; }
    }
}
