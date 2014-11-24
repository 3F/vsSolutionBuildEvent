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
using Newtonsoft.Json;

namespace net.r_eg.vsSBE.Events
{
    public class SBEEvent: ISolutionEvent
    {
        /// <summary>
        /// Status of activation
        /// </summary>
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }
        private bool enabled = false;

        /// <summary>
        /// Optional, unique name for manually identification
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        private string name = null;

        /// <summary>
        /// Short header about this
        /// </summary>
        public string Caption
        {
            get { return caption; }
            set { caption = value; }
        }
        private string caption = String.Empty;
        
        /// <summary>
        /// Support of MSBuild environment variables (properties)
        /// </summary>
        public bool SupportMSBuild
        {
            get { return supportMSBuild; }
            set { supportMSBuild = value; }
        }
        private bool supportMSBuild = true;

        /// <summary>
        /// Support of SBE-Scripts
        /// </summary>
        public bool SupportSBEScripts
        {
            get { return supportSBEScripts; }
            set { supportSBEScripts = value; }
        }
        private bool supportSBEScripts = true;

        /// <summary>
        /// Ignore all actions if the build failed
        /// </summary>
        public bool IgnoreIfBuildFailed
        {
            get { return ignoreIfBuildFailed; }
            set { ignoreIfBuildFailed = value; }
        }
        private bool ignoreIfBuildFailed = false;

        /// <summary>
        /// Type of build action
        /// </summary>
        public BuildType BuildType
        {
            get { return buildType; }
            set { buildType = value; }
        }
        private BuildType buildType = BuildType.Common;

        /// <summary>
        /// User interaction.
        /// Waiting until user presses yes/no etc,
        /// </summary>
        public bool Confirmation
        {
            get { return confirmation; }
            set { confirmation = value; }
        }
        private bool confirmation = false;

        /// <summary>
        /// Run only for a specific configuration of solution
        /// strings format as:
        ///   'configname'|'platformname'
        ///   Compatible with: http://msdn.microsoft.com/en-us/library/microsoft.visualstudio.shell.interop.ivscfg.get_displayname.aspx
        /// </summary>
        public string[] ToConfiguration
        {
            get { return toConfiguration; }
            set { toConfiguration = value; }
        }
        private string[] toConfiguration = null;
        
        /// <summary>
        /// Run for selected projects with the Execution-Order
        /// </summary>
        [JsonProperty(TypeNameHandling = TypeNameHandling.All)]
        public IExecutionOrder[] ExecutionOrder
        {
            get { return executionOrder; }
            set { executionOrder = (ExecutionOrder[])value; }
        }
        private ExecutionOrder[] executionOrder = null;

        /// <summary>
        /// Handling process
        /// </summary>
        [JsonProperty(TypeNameHandling = TypeNameHandling.All)]
        public IEventProcess Process
        {
            get { return process; }
            set { process = (EventProcess)value; }
        }
        private EventProcess process = new EventProcess();

        /// <summary>
        /// Available mode
        /// </summary>
        [JsonProperty(TypeNameHandling = TypeNameHandling.All)]
        public IMode Mode
        {
            get { return mode; }
            set { mode = value; }
        }
        private IMode mode = new ModeFile();
    }
}
