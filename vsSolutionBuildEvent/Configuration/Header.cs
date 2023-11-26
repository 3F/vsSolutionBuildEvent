/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

namespace net.r_eg.vsSBE.Configuration
{
    public class Header
    {
        public string[] _ => new string[]
        {
            " This file for vsSolutionBuildEvent ",
            " https://github.com/3F/vsSolutionBuildEvent "
        };

        public string Compatibility { get; set; } = "0.1";
    }
}
