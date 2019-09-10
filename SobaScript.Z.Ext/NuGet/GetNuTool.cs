/*
 * The MIT License (MIT)
 *
 * Copyright (c) 2014-2019  Denis Kuzmin < x-3F@outlook.com > GitHub/3F
 * Copyright (c) SobaScript.Z.Ext contributors: https://github.com/3F/Varhead/graphs/contributors
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

using System.IO;
using System.Text.RegularExpressions;
using net.r_eg.SobaScript.Z.Ext.Extensions;
using net.r_eg.SobaScript.Z.Ext.IO;

namespace net.r_eg.SobaScript.Z.Ext.NuGet.GetNuTool
{
    internal sealed class GetNuTool
    {
        private IExer exer;
        private string _basePath;

        public string BasePath
        {
            get => _basePath;
            set => _basePath = value.FormatDirPath();
        }

        /// <summary>
        /// Raw command as for original GetNuTool
        /// https://github.com/3F/GetNuTool
        /// </summary>
        /// <param name="data"></param>
        public void Raw(string data)
        {
            data = Regex.Replace
            (
                data, 
                @"\/p(roperty)?\s*?:\s*?wpath\s*?=\s*?""?([^""\s]+)", 
                (Match m) =>
                {
                    var path = m.Groups[1].Value;

                    if(Path.IsPathRooted(path)) {
                        return m.Groups[0].Value;
                    }
                    return $"/p:wpath=\"{Locate(path)}\"";
                }, 
                RegexOptions.IgnoreCase
            );

            exer.UseShell($"{Path.Combine("".GetExecDir(), "gnt.bat")} {data}", true, true);
        }

        public GetNuTool(string basePath)
        {
            BasePath    = basePath;
            exer        = new Exer(basePath);
        }

        private string Locate(string item)
        {
            return Path.Combine(BasePath, item ?? string.Empty);
        }
    }
}