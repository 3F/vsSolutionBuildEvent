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

using System.Collections;
using System.Runtime.InteropServices;
using EnvDTE;

namespace net.r_eg.vsSBE.Provider.Services
{
    internal class _Solution: EnvDTE.Solution
    {
        [DispId(32)]
        public AddIns AddIns { get; set; }
        [DispId(12)]
        public int Count { get; set; }
        [DispId(10)]
        public DTE DTE { get; set; }
        [DispId(35)]
        public string ExtenderCATID { get; set; }
        [DispId(34)]
        public object ExtenderNames { get; set; }
        [DispId(13)]
        public string FileName { get; set; }
        [DispId(28)]
        public string FullName { get; set; }
        [DispId(31)]
        public Globals Globals { get; set; }
        [DispId(22)]
        public bool IsDirty { get; set; }
        [DispId(36)]
        public bool IsOpen { get; set; }
        [DispId(11)]
        public DTE Parent { get; set; }
        [DispId(41)]
        public Projects Projects { get; set; }
        [DispId(19)]
        public Properties Properties { get; set; }
        [DispId(29)]
        public bool Saved { get; set; }
        [DispId(38)]
        public SolutionBuild SolutionBuild { get; set; }

        [DispId(16)]
        public Project AddFromFile(string FileName, bool Exclusive = false) { return null; }
        [DispId(15)]
        public Project AddFromTemplate(string FileName, string Destination, string ProjectName, bool Exclusive = false) { return null; }
        [DispId(18)]
        public void Close(bool SaveFirst = false) { }
        [DispId(40)]
        public void Create(string Destination, string Name) { }
        [DispId(42)]
        public ProjectItem FindProjectItem(string FileName) { return null; }
        [TypeLibFunc(1024)]
        [DispId(33)]
        public object get_Extender(string ExtenderName) { return null; }
        [DispId(26)]
        public string get_TemplatePath(string ProjectType) { return null; }
        [DispId(-4)]
        [TypeLibFunc(1)]
        public IEnumerator GetEnumerator() { return null; }
        [DispId(0)]
        public Project Item(object index) { return null; }
        [DispId(17)]
        public void Open(string FileName) { }
        [DispId(43)]
        public string ProjectItemsTemplatePath(string ProjectKind) { return null; }
        [DispId(25)]
        public void Remove(Project proj) { }
        [DispId(14)]
        public void SaveAs(string FileName) { }
    }
}
