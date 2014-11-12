/*
 * Copyright (c) 2013-2014  Denis Kuzmin (reg) <entry.reg@gmail.com>
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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Newtonsoft.Json;

namespace net.r_eg.vsSBE.Extensions
{
    public static class ObjectExtension
    {
        /// <summary>
        /// Deep copy of public properties through Reflection (slowest)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="destination"></param>
        public static void CloneByReflectionInto<T>(this T obj, T destination)
        {
            PropertyInfo[] properties = obj.GetType().GetProperties();

            foreach(PropertyInfo property in properties) {
                property.SetValue(destination, property.GetValue(obj, null), null);
            }
        }

        /// <summary>
        /// Deep copy through serialization (binary format) - Object should be Serializable.
        /// Note: also not so good.. see IL variants
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">Object for cloning</param>
        /// <returns>Cloned</returns>
        public static T CloneBySerializationBinary<T>(this T obj)
        {
            using(MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter fmt = new BinaryFormatter();
                fmt.Serialize(ms, obj);
                ms.Position = 0;
                return (T)fmt.Deserialize(ms);
            }
        }

        /// <summary>
        /// Deep copy through serialization (text format)
        /// Slower than binary, however supports more objects by default
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">Object for cloning</param>
        /// <returns>Cloned</returns>
        public static T CloneBySerialization<T>(this T obj)
        {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(obj));
        }

        /// <summary>
        /// Deep copy with changing type through serialization (text format)
        /// Useful, if need to cast of objects - Parent to Child
        /// </summary>
        /// <typeparam name="T">Base type</typeparam>
        /// <typeparam name="T2">Extended type</typeparam>
        /// <param name="obj"></param>
        /// <returns>Cloned with new type</returns>
        public static T2 CloneBySerializationWithType<T, T2>(this T obj)
        {
            return JsonConvert.DeserializeObject<T2>(JsonConvert.SerializeObject(obj));
        }
    }
}