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

using System;
using System.ComponentModel;
using Newtonsoft.Json;

namespace net.r_eg.vsSBE.Events.Mapping.Json
{
    /// <summary>
    /// Adds convenient and compatible way to working with ICommand objects for Json serializers.
    /// </summary>
    public abstract class ModeCommand: ICommand
    {
        /// <summary>
        /// Command for handling.
        /// 
        /// Compatible get/set as 'Command' property.
        /// +Updates of friendly 'Command__' property from this data.
        /// 
        /// TODO: Combine this with Command__ wrapper if we're ready for incompatible changes.
        ///       And, how about all as a ICommandArray ? this used with ModeOperation for example.
        /// </summary>
        [Browsable(false)]
        public string Command
        {
            get {
                return command; 
            }
            set {
                if(value != null) {
                    _command = value.Replace("\r\n", "\n").Split('\n');
                }
                command = value;
            }
        }
        protected string command = String.Empty;

        /// <summary>
        /// Readable or friendly  'Command' property as wrapper of original.
        /// Allows updates from this to original.
        /// </summary>
        [JsonProperty(PropertyName = "Command__")]
        protected string[] _Command
        {
            get {
                return _command;
            }
            set {
                if(value != null) {
                    command = String.Join("\n", value);
                }
                _command = value;
            } 
        }
        protected string[] _command;
    }
}
