/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

namespace net.r_eg.vsSBE.UI.WForms.Wizards.Version
{
    internal enum GenType
    {
        /// <summary>
        /// C# Struct
        /// </summary>
        CSharpStruct,

        /// <summary>
        /// C++ Struct
        /// </summary>
        CppStruct,

        /// <summary>
        /// C++ macro definitions ( #define )
        /// </summary>
        CppDefinitions,

        /// <summary>
        /// Direct replacement
        /// </summary>
        Direct,
    }
}
