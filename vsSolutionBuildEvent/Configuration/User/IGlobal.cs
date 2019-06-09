/*
 * Copyright (c) 2013-2016,2019  Denis Kuzmin < entry.reg@gmail.com > GitHub/3F
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace net.r_eg.vsSBE.Configuration.User
{
    [Guid("D9460698-4331-4843-BDAC-F8C7C5F845A2")]
    public interface IGlobal
    {
        /// <summary>
        /// Debug mode for application.
        /// </summary>
        bool DebugMode { get; set; }

        /// <summary>
        /// Suppress the 'Command__' property for main configuration if true.
        /// 
        /// This property is temporary and used for compatibility with format v0.9 of conf. file.
        /// However, this can be inconvenient and while we can't upgrade format, we should also provide a some option to turn off one field at least.
        /// </summary>
        bool SuppressDualCommand { get; set; }

        /// <summary>
        /// List of levels for disabling from logger.
        /// </summary>
        Dictionary<string, bool> LogIgnoreLevels { get; set; }
    }
}
