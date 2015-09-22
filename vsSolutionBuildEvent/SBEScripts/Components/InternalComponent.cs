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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using net.r_eg.vsSBE.Actions;
using net.r_eg.vsSBE.Events;
using net.r_eg.vsSBE.Exceptions;
using net.r_eg.vsSBE.SBEScripts.Dom;
using net.r_eg.vsSBE.SBEScripts.Exceptions;

namespace net.r_eg.vsSBE.SBEScripts.Components
{
    /// <summary>
    /// All internal operations with vsSBE
    /// </summary>
    [Component("vsSBE", "All internal operations with vsSBE")]
    public class InternalComponent: Component, IComponent
    {
        /// <summary>
        /// Ability to work with data for current component
        /// </summary>
        public override string Condition
        {
            get { return "vsSBE "; }
        }

        /// <summary>
        /// Handler for current data
        /// </summary>
        /// <param name="data">mixed data</param>
        /// <returns>prepared and evaluated data</returns>
        public override string parse(string data)
        {
            Match m = Regex.Match(data, @"^\[vsSBE
                                              \s+
                                              (                  #1 - full ident
                                                ([A-Za-z_0-9]+)  #2 - subtype
                                                .*
                                              )
                                           \]$", 
                                           RegexOptions.IgnorePatternWhitespace);

            if(!m.Success) {
                throw new SyntaxIncorrectException("Failed InternalComponent - '{0}'", data);
            }

            switch(m.Groups[2].Value) {
                case "events": {
                    Log.Debug("InternalComponent: use stEvents");
                    return stEvents(m.Groups[1].Value);
                }
            }
            throw new SubtypeNotFoundException("InternalComponent: not found subtype - '{0}'", m.Groups[2].Value);
        }

        /// <summary>
        /// Work with events subtype
        /// #[vsSBE events.Type.item("name")]
        /// #[vsSBE events.Type.item(index)]
        /// e.g.: 
        /// #[vsSBE events.Pre.item("Act1")]
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [Property("events", "Work with events")]
        [Property("Pre", "Pre-Build\nBefore assembling", "events", "stEvents"), Property("", "Pre", "stEvents")]
        [Property("Post", "Post-Build\nAfter assembling", "events", "stEvents"), Property("", "Post", "stEvents")]
        [Property("Cancel", "Cancel-Build\nby user or when an error occurs", "events", "stEvents"), Property("", "Cancel", "stEvents")]
        [Property("Warnings", "Warnings-Build\nWarnings during assembly processing", "events", "stEvents"), Property("", "Warnings", "stEvents")]
        [Property("Errors", "Errors-Build\nErrors during assembly processing", "events", "stEvents"), Property("", "Errors", "stEvents")]
        [Property("OWP", "Output-Build customization\nFull control", "events", "stEvents"), Property("", "OWP", "stEvents")]
        [Property("Transmitter", "Transmitter\nTransmission of the build-data to outer handler", "events", "stEvents"), Property("", "Transmitter", "stEvents")]
        [Property("Logging", "Logging\nAll processes with internal logging", "events", "stEvents"), Property("", "Logging", "stEvents")]
        protected string stEvents(string data)
        {
            Match m = Regex.Match(data,
                                    String.Format(@"events
                                                      \s*\.\s*
                                                      ([A-Za-z]+)       #1 - Event Type
                                                      \s*\.\s*
                                                      item
                                                      \s*
                                                      \(
                                                        (?:
                                                          (\s*\d+\s*)   #2 - item by index
                                                         |
                                                          {0}           #3 - item by name
                                                        )
                                                      \)
                                                      \s*(.+)           #4 - operation with item
                                                    ", RPattern.DoubleQuotesContent
                                                 ), RegexOptions.IgnorePatternWhitespace);

            if(!m.Success) {
                throw new SyntaxIncorrectException("Failed stEvents - '{0}'", data);
            }

            string typeString   = m.Groups[1].Value;
            string index        = (m.Groups[2].Success)? m.Groups[2].Value : null;
            string name         = (m.Groups[3].Success)? StringHandler.normalize(m.Groups[3].Value) : null;
            string operation    = m.Groups[4].Value;

            SolutionEventType type;
            try {
                type = (SolutionEventType)Enum.Parse(typeof(SolutionEventType), typeString);
            }
            catch(Exception ex) {
                throw new OperandNotFoundException("Event type not found - '{0}' :: ", typeString, ex.Message);
            }

            Log.Debug("stEvents: type - '{0}', index - '{1}', name - '{2}'", type, index, name);
            return stEventItem(type, index, name, operation);
        }

        /// <summary>
        /// Work with event-item
        /// #[vsSBE events.Pre.item("Act1").Enabled]
        /// #[vsSBE events.Pre.item("Act1").Status]
        /// </summary>
        /// <param name="type">Type of available events</param>
        /// <param name="index">access by index if used or null value</param>
        /// <param name="name">access by name if used or null value</param>
        /// <param name="data">Operation with event-item</param>
        /// <returns>evaluated data</returns>
        [
            Method
            (
                "item", 
                "Event item by name", 
                "", "stEvents", 
                new string[] { "name" }, 
                new string[] { "Name of the event" }, 
                CValueType.Void, 
                CValueType.String
            )
        ]
        [
            Method
            (
                "item", 
                "Event item by index", 
                "", 
                "stEvents", 
                new string[] { "index" }, 
                new string[] { "Index of the event" },
                CValueType.Void, 
                CValueType.Integer
            )
        ]
        protected string stEventItem(SolutionEventType type, string index, string name, string data)
        {
            Debug.Assert((index == null && name != null) || (index != null && name == null));

            Match m = Regex.Match(data,
                                    String.Format(@"\.\s*
                                                    ([A-Za-z_0-9]+)  #1 - property
                                                    (?:
                                                      \s*\(  
                                                      {0}            #2 - arg (optional)
                                                      \)\s*
                                                    )?
                                                    \s*(.*)          #3 - operation
                                                   ", RPattern.DoubleQuotesContent
                                                 ), RegexOptions.IgnorePatternWhitespace);

            if(!m.Success) {
                throw new SyntaxIncorrectException("Failed stEventItem - '{0}'", data);
            }

            string property     = m.Groups[1].Value;
            string operation    = m.Groups[3].Value.Trim();

            Log.Debug("stEventItem: property - '{0}', operation - '{1}'", property, operation);
            int sIndex = -1;
            ISolutionEvent evt = (name != null)? getEventByName(type, name, out sIndex) : getEventByIndex(type, index, out sIndex);

            switch(property)
            {
                case "Enabled": {
                    return pEnabled(evt, operation);
                }
                case "Status": {
                    return pStatus(type, sIndex, operation);
                }
            }
            throw new SubtypeNotFoundException("stEventItem: not found subtype - '{0}'", property);
        }

        /// <summary>
        /// Work with 'Status' property for selected ISolutionEvent
        /// #[vsSBE events.Pre.item("Act1").Status.HasErrors]
        /// </summary>
        /// <param name="type">Selected event type</param>
        /// <param name="index">access by index</param>
        /// <param name="data">String data with operations</param>
        /// <returns></returns>
        [Property("Status", "Available statuses for selected event-item.", "item", "stEventItem")]
        [Property("HasErrors", "Checking existence of errors after executed action for selected event-item.", "Status", "pStatus", CValueType.Boolean)]
        protected string pStatus(SolutionEventType type, int index, string data)
        {
            Match m = Regex.Match(data, @"\.\s*
                                         ([A-Za-z_0-9]+)  #1 - property
                                         \s*$", 
                                         RegexOptions.IgnorePatternWhitespace);

            if(!m.Success) {
                throw new OperandNotFoundException("Failed pStatus - '{0}'", data);
            }

            string property = m.Groups[1].Value;
            if(property == "HasErrors")
            {
                string status = (Status._.get(type, index) == StatusType.Fail).ToString().ToLower();
                Log.Debug("pStatus: status - '{0}'", status);
                return status;
            }

            throw new SubtypeNotFoundException("pStatus: not found subtype - '{0}'", property);
        }

        /// <summary>
        /// Work with 'Enabled' property for selected ISolutionEvent
        /// get: #[vsSBE events.Pre.item("Act1").Enabled]
        /// set: #[vsSBE events.Pre.item("Act1").Enabled = false]
        /// </summary>
        /// <param name="evt">Selected event</param>
        /// <param name="data">String data with operations</param>
        /// <returns></returns>
        [Property("Enabled", "Gets or Sets Enabled status for selected event-item", "item", "stEventItem", CValueType.Boolean, CValueType.Boolean)]
        protected string pEnabled(ISolutionEvent evt, string data)
        {
            if(data.Trim().Length < 1) {
                return evt.Enabled.ToString().ToLower();
            }

            Match m = Regex.Match(data, @"=\s*(false|true|1|0)\s*$", RegexOptions.IgnoreCase);
            if(!m.Success) {
                throw new OperandNotFoundException("Failed pEnabled - '{0}'", data);
            }
            evt.Enabled = Value.toBoolean(m.Groups[1].Value);

            Log.Debug("pEnabled: setted as '{0}' for '{1}'", evt.Enabled, evt.Name);
            return String.Empty;
        }

        /// <param name="sIndex">Binding index with the Execution status</param>
        protected ISolutionEvent getEventByName(SolutionEventType type, string name, out int sIndex)
        {
            sIndex = -1;
            if(String.IsNullOrEmpty(name)) {
                throw new NotFoundException("getEventByName: name is null or empty");
            }
            ISolutionEvent[] evt = getEvent(type);

            foreach(ISolutionEvent item in evt) {
                ++sIndex;
                if(item.Name == name) {
                    return item;
                }
            }
            throw new NotFoundException("getEvent: not found name - '{0}' with type - '{1}'", name, type);
        }

        /// <param name="sIndex">Binding index with the Execution status</param>
        protected ISolutionEvent getEventByIndex(SolutionEventType type, string index, out int sIndex)
        {
            sIndex = -1;
            ISolutionEvent[] evt = getEvent(type);
            try {
                sIndex = Int32.Parse(index) - 1; // >= 1
                return evt[sIndex];
            }
            catch(Exception) {
                throw new NotFoundException("getEvent: incorrect index - '{0}'({1}) with type - '{2}'", index, evt.Length, type);
            }
        }

        protected virtual ISolutionEvent[] getEvent(SolutionEventType type)
        {
            try {
                return Settings.Cfg.getEvent(type);
            }
            catch(Exception) {
                throw new NotSupportedOperationException("getEvent: Not yet supported event type - '{0}'", type);
            }
        }
    }
}
