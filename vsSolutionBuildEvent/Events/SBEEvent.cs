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
using net.r_eg.vsSBE.Bridge;
using Newtonsoft.Json;

namespace net.r_eg.vsSBE.Events
{
    public class SBEEvent: ISolutionEvent
    {
        private Guid id = Guid.NewGuid();

        /// <inheritdoc cref="ISolutionEvent.Enabled"/>
        public bool Enabled { get; set; } = true;

        /// <inheritdoc cref="ISolutionEvent.Name"/>
        public string Name { get; set; }

        /// <inheritdoc cref="ISolutionEvent.Caption"/>
        public string Caption { get; set; } = string.Empty;

        /// <inheritdoc cref="ISolutionEvent.SupportMSBuild"/>
        public bool SupportMSBuild { get; set; } = true;

        /// <inheritdoc cref="ISolutionEvent.SupportSBEScripts"/>
        public bool SupportSBEScripts { get; set; } = true;

        /// <inheritdoc cref="ISolutionEvent.IgnoreIfBuildFailed"/>
        public bool IgnoreIfBuildFailed { get; set; } = false;

        /// <inheritdoc cref="ISolutionEvent.BuildType"/>
        public BuildType BuildType { get; set; } = BuildType.Common;

        /// <inheritdoc cref="ISolutionEvent.Confirmation"/>
        public bool Confirmation { get; set; } = false;

        /// <inheritdoc cref="ISolutionEvent.ToConfiguration"/>
        public string[] ToConfiguration { get; set; } = null;

        /// <inheritdoc cref="ISolutionEvent.ExecutionOrder"/>
        [JsonProperty(TypeNameHandling = TypeNameHandling.All)]
        public IExecutionOrder[] ExecutionOrder
        {
            get { return executionOrder; }
            set { executionOrder = (ExecutionOrder[])value; }
        }
        private ExecutionOrder[] executionOrder = null;

        /// <inheritdoc cref="ISolutionEvent.Process"/>
        [JsonProperty(TypeNameHandling = TypeNameHandling.All)]
        public IEventProcess Process
        {
            get { return process; }
            set { process = (EventProcess)value; }
        }
        private EventProcess process = new EventProcess();

        /// <inheritdoc cref="ISolutionEvent.Mode"/>
        [JsonProperty(TypeNameHandling = TypeNameHandling.All)]
        public IMode Mode { get; set; } = new ModeScript();

        /// <inheritdoc cref="ISolutionEvent.Id"/>
        [JsonIgnore]
        public Guid Id => id;

        //TODO: We are currently using compatibility with 0.12.4. True is configured for default values only in v1.14.1
        //public bool ShouldSerializeEnabled() => !Enabled;
        public bool ShouldSerializeCaption() => !string.IsNullOrEmpty(Caption);
        public bool ShouldSerializeSupportMSBuild() => !SupportMSBuild;
        public bool ShouldSerializeSupportSBEScripts() => !SupportSBEScripts;
        public bool ShouldSerializeIgnoreIfBuildFailed() => IgnoreIfBuildFailed;
        public bool ShouldSerializeBuildType() => BuildType != BuildType.Common;
        public bool ShouldSerializeConfirmation() => Confirmation;
        public bool ShouldSerializeToConfiguration() => ToConfiguration?.Length > 0;
        public bool ShouldSerializeExecutionOrder() => ExecutionOrder?.Length > 0;
        public bool ShouldSerializeProcess() => new EventProcess() != Process as EventProcess;
    }
}
