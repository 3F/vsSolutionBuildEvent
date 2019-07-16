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

namespace net.r_eg.vsSBE.Extensions
{
    public static class CollectionExtension
    {
        /// <summary>
        /// Returns either value from dictionary or configured default value.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TVal"></typeparam>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <param name="def">Use this if key is not found.</param>
        /// <returns></returns>
        public static TVal GetOrDefault<TKey, TVal>(this IDictionary<TKey, TVal> data, TKey key, TVal def = default(TVal))
        {
            if(data == null) {
                return def;
            }
            return data.ContainsKey(key) ? data[key] : def;
        }
    }
}
