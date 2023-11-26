/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.ComponentModel;
using net.r_eg.vsSBE.Events.Commands;
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
        [JsonIgnore] // "Command" removed after 1.14.0. Minimal 0.12.4
        public string Command
        {
            get => command;
            set
            {
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
            get => _command;
            set
            {
                if(value != null) {
                    command = String.Join("\n", value);
                }
                _command = value;
            } 
        }
        protected string[] _command;
    }
}
