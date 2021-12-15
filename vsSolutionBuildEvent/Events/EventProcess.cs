/*
 * Copyright (c) 2013-2021  Denis Kuzmin <x-3F@outlook.com> github/3F
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

using net.r_eg.MvsSln.Extensions;

namespace net.r_eg.vsSBE.Events
{
    public class EventProcess: IEventProcess
    {
        /// <inheritdoc cref="IEventProcess.Waiting"/>
        public bool Waiting { get; set; } = true;

        /// <inheritdoc cref="IEventProcess.Hidden"/>
        public bool Hidden { get; set; } = true;

        /// <inheritdoc cref="IEventProcess.TimeLimit"/>
        public int TimeLimit { get; set; } = 30;

        public static bool operator ==(EventProcess a, EventProcess b) => a.Equals(b);

        public static bool operator !=(EventProcess a, EventProcess b) => !(a == b);

        public override bool Equals(object obj)
        {
            if(obj is null || !(obj is EventProcess)) {
                return false;
            }

            var b = (EventProcess)obj;

            return Waiting == b.Waiting
                    && Hidden == b.Hidden
                    && TimeLimit == b.TimeLimit;
        }

        public override int GetHashCode()
        {
            return 0.CalculateHashCode
            (
                Waiting,
                Hidden,
                TimeLimit
            );
        }
    }
}