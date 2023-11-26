/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

namespace net.r_eg.vsSBE.Configuration.User
{
    public class Common: ICommon
    {
        /// <summary>
        /// Word wrapping for main editor.
        /// </summary>
        public bool WordWrap
        {
            get { return wordWrap; }
            set { wordWrap = value; }
        }
        private bool wordWrap = true;

        /// <summary>
        /// Value of zooming for main editor.
        /// </summary>
        public int Zoomed
        {
            get { return zoomed; }
            set { zoomed = value; }
        }
        private int zoomed = 100;
    }
}
