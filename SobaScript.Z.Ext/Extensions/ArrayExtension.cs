﻿/*
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

using System.Collections.Generic;
using System.IO;
using System.Text;

namespace net.r_eg.SobaScript.Z.Ext.Extensions
{
    public static class ArrayExtension
    {
        /// <summary>
        /// To format bytes data to hex view.
        /// </summary>
        /// <param name="data">Bytes data.</param>
        /// <returns>Hex view of bytes.</returns>
        internal static string BytesToHexView(this byte[] data)
        {
            StringBuilder ret = new StringBuilder();

            foreach(byte b in data) {
                ret.Append(b.ToString("X2"));
            }

            return ret.ToString();
        }

        /// <summary>
        /// Extracts absolute paths to files with mask (*.*, *.dll, ..)
        /// </summary>
        /// <param name="files">List of files.</param>
        /// <param name="basePath"></param>
        /// <returns></returns>
        public static string[] ExtractFiles(this string[] files, string basePath)
        {
            var ret = new List<string>();
            foreach(string file in files)
            {
                string mask     = Path.GetFileName(file);
                string fullname = Path.Combine(basePath, file);

                if(mask.IndexOf('*') != -1) {
                    ret.AddRange(Directory.GetFiles(Path.GetDirectoryName(fullname), mask));
                    continue;
                }
                ret.Add(fullname);
            }
            return ret.ToArray();
        }
    }
}
