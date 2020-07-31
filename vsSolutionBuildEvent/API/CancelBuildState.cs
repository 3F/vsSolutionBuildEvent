﻿/*
 * Copyright (c) 2013-2016,2019-2020  Denis Kuzmin < x-3F@outlook.com > GitHub/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent
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

namespace net.r_eg.vsSBE.API
{
    internal sealed class CancelBuildState
    {
        /// <summary>
        /// Actual state for VS API.
        /// </summary>
        public int CanceledInt => Canceled ? 1 : 0;

        /// <summary>
        /// Actual state.
        /// </summary>
        public bool Canceled { get; private set; }

        public void Cancel() => Canceled = true;

        public void Reset() => Canceled = false;

        public bool UpdateFlagIfCanceled(ref int flag)
        {
            if(Canceled)
            {
                Log.Debug($"Canceled operation due to {nameof(CancelBuildState)} -> {flag}");
                flag = CanceledInt;
            }
            return Canceled;
        }
    }
}