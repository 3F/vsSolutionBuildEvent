/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * This component (part of vsSolutionBuildEvent) is licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/vsSolutionBuildEvent
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
