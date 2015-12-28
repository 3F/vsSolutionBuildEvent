/*
 * Copyright (c) 2013-2015  Denis Kuzmin (reg) <entry.reg@gmail.com>
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
using System.IO;
using System.Text;

namespace net.r_eg.vsSBE.Extensions
{
    public static class ArrayExtension
    {
        /// <summary>
        /// Adding new element into the data and returning as new array
        /// </summary>
        /// <typeparam name="T">Type of array</typeparam>
        /// <param name="data"></param>
        /// <param name="item"></param>
        /// <returns>array with added item</returns>
        public static T[] GetWithAdded<T>(this T[] data, T item)
        {
            T[] ret = new T[data.Length + 1];
            data.CopyTo(ret, 0);
            ret[data.Length] = item;
            return ret;
        }

        /// <summary>
        /// Removing element from the data and returning as new array
        /// </summary>
        /// <typeparam name="T">Type of array</typeparam>
        /// <param name="data"></param>
        /// <param name="index">Index of item for removing</param>
        /// <returns>array without element at the specified index</returns>
        public static T[] GetWithRemoved<T>(this T[] data, int index)
        {
            T[] ret = new T[data.Length - 1];
            int idx = 0;
            for(int i = 0; i < data.Length; ++i) {
                if(i != index) {
                    ret[idx++] = data[i];
                }
            }
            return ret;
        }

        /// <summary>
        /// Inserts an element into the data at the specified index and returning as new array
        /// </summary>
        /// <typeparam name="T">Type of array</typeparam>
        /// <param name="data"></param>
        /// <param name="index">The zero-based index at which item should be inserted. If index is equal to Length, item is added to the end.</param>
        /// <param name="item"></param>
        /// <returns>array with added item at the specified index</returns>
        public static T[] GetWithInserted<T>(this T[] data, int index, T item)
        {
            if(data.Length == index) {
                return GetWithAdded(data, item);
            }
            T[] ret = new T[data.Length + 1];
            int idx = 0;
            for(int i = 0; i < data.Length; ++i) {
                if(i == index) {
                    ret[idx++] = item;
                }
                ret[idx++] = data[i];
            }
            return ret;
        }

        /// <summary>
        /// Moving element into the new position
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="from">old index</param>
        /// <param name="to">new index</param>
        /// <returns>array with moved item at the specified indexes</returns>
        public static T[] GetWithMoved<T>(this T[] data, int from, int to)
        {
            T moving = data[from];
            return GetWithInserted(data.GetWithRemoved(from), to, moving);
        }

        /// <summary>
        /// To format bytes data to hex view.
        /// </summary>
        /// <param name="data">Bytes data.</param>
        /// <returns>Hex view of bytes.</returns>
        public static string BytesToHexView(this byte[] data)
        {
            StringBuilder ret = new StringBuilder();
            foreach(byte b in data) {
                ret.Append(b.ToString("X2"));
            }
            return ret.ToString();
        }

        /// <summary>
        /// Extracts absolute paths to files with mask (*.*, *.dll, ..)
        /// </summary>
        /// <param name="files">List of files.</param>
        /// <param name="path">Base path.</param>
        /// <returns></returns>
        public static string[] ExtractFiles(this string[] files, string path = null)
        {
            List<string> ret = new List<string>();
            foreach(string file in files)
            {
                string mask     = Path.GetFileName(file);
                string fullname = Path.Combine(path ?? Settings.WPath, file);

                if(mask.IndexOf('*') != -1) {
                    ret.AddRange(Directory.GetFiles(Path.GetDirectoryName(fullname), mask));
                    continue;
                }
                ret.Add(fullname);
            }
            return ret.ToArray();
        }
    }
}
