/*
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

using System;
using System.IO;
using System.Runtime.Serialization.Formatters;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace net.r_eg.vsSBE.Configuration
{
    /// <summary>
    /// Contains logic of packaging objects for configuration.
    /// 
    /// The TOut can be used for providing specific implementation type
    /// for work without custom converters and TypeNameHandling property for all.
    /// </summary>
    /// <typeparam name="TIn">Abstract or specific type for serialization.</typeparam>
    /// <typeparam name="TOut">Specific type for deserialization.</typeparam>
    public abstract class PackerAbstract<TIn, TOut>
        where TOut: TIn // the TOut should be equivalent to TIn, for example: extending TIn or have a common interface, or simple equal to TIn.
    {
        /// <summary>
        /// Link to configuration file.
        /// </summary>
        public string Link
        {
            get;
            protected set;
        }

        /// <summary>
        /// Get status of configuration data.
        /// true value if data exists only in RAM, otherwise used existing file.
        /// </summary>
        public bool InRAM
        {
            get;
            protected set;
        }

        /// <summary>
        /// Defines full path to configuration file.
        /// </summary>
        /// <param name="path">Base path.</param>
        /// <param name="name">Name of configuration file.</param>
        /// <param name="prefix">File prefix if used.</param>
        /// <returns>Link to configuration file.</returns>
        public string getLink(string path, string name, string prefix)
        {
            string link = formatLink(path, name, prefix);
            if(!File.Exists(link))
            {
                Log.Trace("Configuration: special version of '{0}' is not found /'{1}':'{2}'", name, prefix, link);
                link = formatLink(path, name);
            }
            return link;
        }

        /// <summary>
        /// Serialize data from object to string.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public string serialize(TIn data)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                NullValueHandling       = NullValueHandling.Ignore,
                StringEscapeHandling    = StringEscapeHandling.EscapeNonAscii,
                Formatting              = Formatting.Indented,
                MissingMemberHandling   = MissingMemberHandling.Ignore,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
            };

            settings.Converters.Add(new StringEnumConverter{ 
                AllowIntegerValues  = false
                //CamelCaseText       = true
            });

            return JsonConvert.SerializeObject(data, Formatting.Indented, settings);
        }

        /// <summary>
        /// Deserialize data from string to specific type.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public TOut deserialize(string data)
        {
            return JsonConvert.DeserializeObject<TOut>(data, new JsonSerializerSettings() {
                SerializationBinder = new JsonSerializationBinder()
            });
        }

        /// <summary>
        /// Deserialize data from stream to specific type.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        protected TOut deserialize(StreamReader stream)
        {
            using(JsonTextReader reader = new JsonTextReader(stream))
            {
                JsonSerializer js = new JsonSerializer() {
                    SerializationBinder = new JsonSerializationBinder() 
                };
                return js.Deserialize<TOut>(reader);
            }
        }

        /// <summary>
        /// Serialize data from object to stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="data"></param>
        protected void serialize(TextWriter stream, TIn data)
        {
            stream.Write(serialize(data));
        }

        /// <param name="path">Full path to configuration file</param>
        /// <param name="name">File name</param>
        /// <param name="prefix">Special version of configuration file if used</param>
        /// <returns></returns>
        protected string formatLink(string path, string name, string prefix = null)
        {
            return Path.Combine(path, String.Format("{0}{1}", prefix, name));
        }
    }
}
