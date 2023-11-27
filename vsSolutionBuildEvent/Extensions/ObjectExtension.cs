/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace net.r_eg.vsSBE.Extensions
{
    public static class ObjectExtension
    {
        /// <summary>
        /// Copying of all public properties with Reflection (slowest).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="skipNullVal">To skip properties with null value if true.</param>
        public static void CloneByReflectionInto<T>(this T source, T destination, bool skipNullVal = false)
        {
            if(source == null) {
                return;
            }
            PropertyInfo[] properties = source.GetType().GetProperties();

            foreach(PropertyInfo property in properties)
            {
                if(property.GetSetMethod() == null) {
                    continue;
                }

                object val = property.GetValue(source, null);
                if(skipNullVal && val == null) {
                    continue;
                }
                property.SetValue(destination, val, null);
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
        /// Deep copy through serialization.
        /// Alias to CloneBySerializationWithType.
        /// </summary>
        /// <typeparam name="T">Base type of object.</typeparam>
        /// <param name="obj">Object for cloning.</param>
        /// <returns>The new object.</returns>
        public static T CloneBySerialization<T>(this T obj)
        {
            return obj.CloneBySerializationWithType<T, T>();
        }

        /// <summary>
        /// Deep copy with changing type via serialization (text format).
        /// Supports a lot of objects by default than binary variant above.
        /// The T + T2 useful if needed casting of object, e.g. Parent to Child
        /// </summary>
        /// <typeparam name="T">Base type of object.</typeparam>
        /// <typeparam name="T2">Specific type for new object.</typeparam>
        /// <param name="obj">Object for cloning.</param>
        /// <returns>The new object with specific type.</returns>
        public static T2 CloneBySerializationWithType<T, T2>(this T obj)
        {
            if(obj == null) {
                return default(T2);
            }

            return JsonConvert.DeserializeObject<T2>
            (
                JsonConvert.SerializeObject
                (
                    obj, 
                    Formatting.None, 
                    new JsonSerializerSettings()
                    {
                        NullValueHandling   = NullValueHandling.Include,
                        Formatting          = Formatting.None,
                        TypeNameHandling    = TypeNameHandling.All,
                    }
                ),

                new JsonSerializerSettings() {
                    SerializationBinder = new JsonSerializationBinder(),
                }
            );
        }

        /// <summary>
        /// Comparing objects and array of objects.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <param name="nullAndEmptyStr">Compare null or empty strings as equal if true.</param>
        /// <returns>true value if the objects are considered equal.</returns>
        public static bool EqualsMixedObjects(this object left, object right, bool nullAndEmptyStr = false)
        {
            if(left == null && right == null) return true;
            if(left == null || right == null)
            {
                return nullAndEmptyStr 
                        && 
                        (
                            (left == null && right is string rstr && rstr == string.Empty)
                            || (right == null && left is string lstr && lstr == string.Empty)
                        );
            }
            if(ReferenceEquals(left, right)) return true;

            if(left.GetType().IsArray && right.GetType().IsArray) {
                return Enumerable.SequenceEqual((object[])left, (object[])right);
            }

            if(!left.GetType().IsArray && !right.GetType().IsArray) {
                return left.Equals(right);
            }

            return false;
        }

        /// <summary>
        /// Checks object on null value and empty string if it's string.
        /// </summary>
        /// <param name="obj">Object for checking.</param>
        /// <returns>true if null or empty string, otherwise false.</returns>
        public static bool IsNullOrEmptyString(this object obj)
        {
            return (obj == null || obj is string str && str == string.Empty);
        }

        /// <summary>
        /// Calculate MD5 hash from object.
        /// </summary>
        /// <typeparam name="T">Base type of object.</typeparam>
        /// <param name="obj">Object for calculating.</param>
        /// <returns>MD5 Hash code.</returns>
        public static string MD5Hash<T>(this T obj)
        {
            using(MD5 md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(
                                        Encoding.UTF8.GetBytes(
                                            JsonConvert.SerializeObject(
                                                obj, 
                                                Formatting.None, 
                                                new JsonSerializerSettings()
                                                {
                                                    NullValueHandling   = NullValueHandling.Include,
                                                    Formatting          = Formatting.None,
                                                    TypeNameHandling    = TypeNameHandling.All,
                                                }
                                        )));
                
                return hash.BytesToHexView();
            }
        }
    }
}