/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
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
    }
}
