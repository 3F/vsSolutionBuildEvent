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

using System.Collections.Generic;
using net.r_eg.vsSBE.Events;

namespace net.r_eg.vsSBE.UI.WForms.Logic
{
    public class SBEWrap
    {
        /// <summary>
        /// Wrapped event
        /// </summary>
        public List<ISolutionEvent> evt;

        /// <summary>
        /// Specific type
        /// </summary>
        public SolutionEventType type;

        /// <param name="type"></param>
        public SBEWrap(SolutionEventType type)
        {
            this.type = type;
            update();
        }

        /// <summary>
        /// Updating list from used array data
        /// </summary>
        public void update()
        {
            if(Settings.Cfg.getEvent(type) != null) {
                evt = new List<ISolutionEvent>(Settings.Cfg.getEvent(type));
                return;
            }

            Log.Debug("SBEWrap: evt is null for type '{0}'", type);
            evt = new List<ISolutionEvent>();
        }
    }
}