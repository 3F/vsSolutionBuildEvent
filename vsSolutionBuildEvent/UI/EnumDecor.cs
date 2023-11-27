/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

namespace net.r_eg.vsSBE.UI
{
    public static class EnumDecor
    {
        private const string VS_CONST = "Microsoft.VisualStudio.VSConstants";

        public static string Shorten(string input)
        {
            if(input == null) return null;

            if(input.StartsWith(VS_CONST))
            {
                return input.Substring(VS_CONST.Length);
            }

            return input;
        }
    }
}
