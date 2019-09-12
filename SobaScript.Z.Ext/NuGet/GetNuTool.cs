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

using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using net.r_eg.SobaScript.Z.Ext.Extensions;
using net.r_eg.SobaScript.Z.Ext.IO;

namespace net.r_eg.SobaScript.Z.Ext.NuGet
{
    internal sealed class GetNuTool
    {
        const string GNT = "gnt.bat";

        private IExer exer;
        private string _basePath;

        private readonly Type gtype = typeof(GetNuTool);

        public string BasePath
        {
            get => _basePath;
            set => _basePath = value.FormatDirPath();
        }

        private string GntBat => Path.Combine("".GetExecDir(), GNT);

        /// <summary>
        /// Raw command as for original GetNuTool
        /// https://github.com/3F/GetNuTool
        /// </summary>
        /// <param name="data"></param>
        public void Raw(string data)
        {
            bool success = false;

            data = Regex.Replace
            (
                data, 
                @"(?'left'
                   \/p(?:roperty)?
                    \s*?:\s*?
                    wpath\s*?=\s*?
                  )
                  (?:
                    ""(?'str'[^""]+)
                    |
                    (?'val'[^\s\/]+)
                )", 

                (Match m) => 
                {
                    success = true;

                    string ret = m.Groups["left"].Value;

                    if(m.Groups["str"].Success) {
                        return ret + "\"" + Locate(m.Groups["str"].Value);
                    }

                    return ret + Locate(m.Groups["val"].Value);
                },

                RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace
            );

            if(!success) {
                data += $" /p:wpath=\"{Locate()}\" ";
            }

            exer.UseShell($"{UseGnt(GntBat)} {data}", true, true);
        }

        public GetNuTool(string basePath)
        {
            BasePath    = basePath;
            exer        = new Exer(basePath);
        }

        private string UseGnt(string src)
        {
            if(File.Exists(src)) {
                return src;
            }

            using(var ws = new StreamWriter(src, false, new UTF8Encoding(false)))
            using(var rs = gtype.Assembly.GetManifestResourceStream(gtype.Namespace + "." + GNT)) {
                rs.CopyTo(ws.BaseStream);
            }

            return src;
        }

        private string Locate(string item = null)
        {
            if(item == null) {
                return ChkPath(BasePath);
            }

            return ChkPath
            (
                Path.IsPathRooted(item) ? item 
                    : Path.Combine(BasePath, item)
            );
        }

        private string ChkPath(string path)
            => path.TrimEnd(new[] { '\\', '/' }) + @"\\"; // we only care about the last slashes \" -> \\"
    }
}