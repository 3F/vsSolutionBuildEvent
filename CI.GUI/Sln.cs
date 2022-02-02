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
using System.IO;

namespace net.r_eg.vsSBE.CI.GUI
{
    internal static class Sln
    {
        private const string EXT = ".sln";
        private const string DEF_NAME = "vssbe" + EXT;

        /// <param name="input">Path to directory or .sln or null to search using current dir.</param>
        /// <param name="generate">True to generate the new one instead of exception.</param>
        /// <returns>Full path to .sln</returns>
        public static string FindFullPath(string input = null, bool generate = true)
            => Path.GetFullPath(Find(input, generate));

        private static string Find(string input = null, bool generate = true)
        {
            if(!string.IsNullOrWhiteSpace(input))
            {
                if(EXT.Equals(Path.GetExtension(input), StringComparison.OrdinalIgnoreCase))
                {
                    return input;
                }

                var slnFromDir = GetFirstSln(input);
                if(slnFromDir != null)
                {
                    return slnFromDir;
                }
            }

            var _sln = GetFirstSln(System.Environment.CurrentDirectory);
            if(_sln != null)
            {
                return _sln;
            }

            if(generate)
            {
                return GenerateNewSln(System.Environment.CurrentDirectory);
            }
            throw new FileNotFoundException($"{EXT} is not found neither in `{System.Environment.CurrentDirectory}` nor through arguments `{input}`. Please specify a valid {EXT}");
        }

        private static string GenerateNewSln(string dir)
        {
            var _sln = Path.Combine(dir ?? throw new ArgumentNullException(nameof(dir)), DEF_NAME);
            File.WriteAllText(_sln, "\r\nMicrosoft Visual Studio Solution File, Format Version 11.00\r\n");
            return _sln;
        }

        /// <summary>
        /// Get first found .sln file in directory.
        /// But it will keep <see cref="DEF_NAME"/> in the end of the list.
        /// </summary>
        private static string GetFirstSln(string dir)
        {
            if(!Directory.Exists(dir))
            {
                return null;
            }

            string found = null;
            foreach(var f in Directory.GetFiles(dir, "*.sln", SearchOption.TopDirectoryOnly))
            {
                found = f;
                if(Path.GetFileName(f) == DEF_NAME) continue;
                return f;
            }
            return found;
        }
    }
}
