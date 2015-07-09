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
using System.Diagnostics;
using System.Text.RegularExpressions;
using net.r_eg.vsSBE.Exceptions;
using net.r_eg.vsSBE.SBEScripts.Components;
using net.r_eg.vsSBE.SBEScripts.Exceptions;
using net.r_eg.vsSBE.Scripts;

namespace net.r_eg.vsSBE.SBEScripts
{
    public class Script: ISBEScript, IEvaluator
    {
        /// <summary>
        /// Maximum of nesting level
        /// </summary>
        const int DEPTH_LIMIT = 70;

        /// <summary>
        /// General container of SBE-Script
        /// </summary>
        public string ContainerPattern
        {
            get
            {
                /*
                     (
                       \#{1,2}
                     )
                     (?=
                       (
                         \[
                           (?>
                             [^\[\]]
                             |
                             (?2)
                           )*
                         \]
                       )
                     )            -> for .NET: v
                */
                return @"(?:\r?\n\x20*)?\r?\n?(
                            \#{1,2}   #1 - # or ##
                          )
                          (           #2 - mixed data of SBE-Script
                            \[
                              (?>
                                [^\[\]]
                                |
                                \[(?<R>)
                                |
                                \](?<-R>)
                              )*
                              (?(R)(?!))
                            \]
                          )\r?\n?";
            }
        }

        /// <summary>
        /// Getting instance of used loader.
        /// Initialization with default loader if not selected.
        /// </summary>
        public IBootloader Bootloader
        {
            get
            {
                if(bootloader == null)
                {
                    Debug.Assert(env != null);
                    Debug.Assert(uvariable != null);

                    bootloader = new Bootloader(env, uvariable);
                    bootloader.register();
                }
                return bootloader;
            }
        }
        protected IBootloader bootloader;

        /// <summary>
        /// Work with user-variables
        /// </summary>
        protected IUserVariable uvariable;

        /// <summary>
        /// Provides operation with environment
        /// </summary>
        protected IEnvironment env;

        /// <summary>
        /// Basic operations with strings
        /// </summary>
        protected StringHandler hString;

        /// <summary>
        /// Flag of required post-processing with MSBuild core.
        /// In general, some components can require immediate processing with evaluation, before passing control to next level
        /// (e.g. FileComponent etc.) For such components need additional flag about allowed processing, if this used of course...
        /// </summary>
        protected bool postProcessingMSBuild;

        /// <summary>
        /// Current level of nesting data.
        /// Aborting if reached limit
        /// </summary>
        private volatile int _depthLevel = 0;

        /// <summary>
        /// object synch.
        /// </summary>
        private Object _lock = new Object();

        /// <summary>
        /// Handler of mixed data SBE-Scripts
        /// Format: https://bitbucket.org/3F/vssolutionbuildevent/issue/22/#comment-12739932
        /// </summary>
        /// <param name="data">mixed data</param>
        /// <param name="allowMSBuild">Allows post-processing with MSBuild or not.
        /// Some components can require immediate processing with evaluation, before passing control to next level.
        /// </param>
        /// <returns>prepared and evaluated data</returns>
        public string parse(string data, bool allowMSBuild)
        {
            lock(_lock)
            {
                _depthLevel = 0;
                postProcessingMSBuild = allowMSBuild;
                StringHandler hString = new StringHandler();
                return hString.recovery(parse(hString.protectMixedQuotes(data), _depthLevel, hString));
            }
        }

        public string parse(string data)
        {
            return parse(data, false);
        }

        /// <summary>
        /// Evaluating data with current object
        /// </summary>
        /// <param name="data">mixed data</param>
        /// <returns>Evaluated end value</returns>
        public string evaluate(string data)
        {
            return parse(data);
        }

        /// <param name="env">Used environment</param>
        /// <param name="uvariable">Used instance of user-variable</param>
        public Script(IEnvironment env, IUserVariable uvariable)
        {
            this.env = env;
            this.uvariable = uvariable;
        }

        /// <param name="loader">Initialization with IBootloader</param>
        public Script(IBootloader loader)
        {
            bootloader  = loader;
            env         = loader.Env;
            uvariable   = loader.UVariable;
        }

        /// <param name="data">Mixed data</param>
        /// <param name="level">Nesting level</param>
        /// <param name="hString">Handler of strings if exist</param>
        /// <returns>Prepared & evaluated data</returns>
        protected string parse(string data, int level, StringHandler hString = null)
        {
            if(level >= DEPTH_LIMIT) {
                _depthLevel = 0;
                throw new LimitException("Nesting level of '{0}' reached. Aborted.", DEPTH_LIMIT);
            }

            return Regex.Replace(data, ContainerPattern, delegate(Match m)
            {
                if(m.Groups[1].Value.Length > 1) { //escape
                    Log.nlog.Debug("SBEScripts: escape - '{0}'", m.Groups[2].Value);
                    return "#" + escapeMSBuildData(m.Groups[2].Value, true);
                }
                string raw = m.Groups[2].Value;

                Log.nlog.Trace("SBEScripts-data: to parse '{0}'", raw);
                if(hString != null) {
                    return selector(hString.recovery(raw));
                }
                return selector(raw);
            }, 
            RegexOptions.IgnorePatternWhitespace);
        }

        /// <summary>
        /// Parse data for concrete component
        /// </summary>
        /// <param name="data">Mixed data</param>
        /// <param name="c">Component</param>
        /// <returns>Prepared & evaluated data by component</returns>
        protected string parse(string data, IComponent c)
        {
            string ret = c.parse(data);

            if(c.PostParse)
            {
                ++_depthLevel;
                ret = parse(ret, _depthLevel);
                --_depthLevel;
            }
            return ret;
        }

        /// <param name="data"></param>
        /// <param name="force">only $(..) -> $$(..) if false / and $$(..) -> $$$(..), etc. if true</param>
        /// <returns></returns>
        protected virtual string escapeMSBuildData(string data, bool force)
        {
            string pattern = String.Format(@"{0}{1}", 
                                            (force)? String.Empty : @"(?<!\$)", 
                                            MSBuild.RPattern.ContainerOuter);
            return Regex.Replace(data, pattern, delegate(Match m) { return "$" + m.Value; }, RegexOptions.IgnorePatternWhitespace);
        }

        /// <summary>
        /// Checking ability to parse the data for specific component
        /// </summary>
        /// <param name="data">Mixed data</param>
        /// <param name="c">Component</param>
        /// <returns>ready to parse or not</returns>
        protected bool isReadyToParse(string data, IComponent c)
        {
            if(!c.CRegex) {
                return data.StartsWith(String.Format("[{0}", c.Condition));
            }
            return Regex.IsMatch(data, String.Format("^\\[{0}", c.Condition), RegexOptions.IgnorePatternWhitespace);
        }

        /// <summary>
        /// Work with SBE-Script by components
        /// </summary>
        /// <param name="data">mixed data</param>
        /// <returns>prepared & evaluated data</returns>
        protected string selector(string data)
        {
            Log.nlog.Debug("SBEScripts-selector: started with '{0}'", data);

            foreach(IComponent c in Bootloader.Components)
            {
                c.PostProcessingMSBuild = postProcessingMSBuild;

                if(!c.BeforeDeepen) {
                    continue;
                }

                if(isReadyToParse(data, c)) {
                    return parse(data, c);
                }
            }

            if(deepen(ref data)) {
                ++_depthLevel;
                data = parse(data, _depthLevel);
                --_depthLevel;
            }

            foreach(IComponent c in Bootloader.Components)
            {
                if(c.BeforeDeepen) {
                    continue; // should already parsed above
                }

                if(isReadyToParse(data, c)) {
                    return parse(data, c);
                }
            }

            throw new SelectorMismatchException("SBEScripts-selector: not found component for handling - '{0}'", data);
        }

        protected bool deepen(ref string data)
        {
            return Regex.IsMatch(data, ContainerPattern, RegexOptions.IgnorePatternWhitespace);
        }
    }
}
