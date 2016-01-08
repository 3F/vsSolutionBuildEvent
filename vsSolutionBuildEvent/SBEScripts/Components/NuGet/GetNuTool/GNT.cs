/*
 * Copyright (c) 2013-2016  Denis Kuzmin (reg) <entry.reg@gmail.com>
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
using System.IO;
using System.Text.RegularExpressions;
using net.r_eg.vsSBE.SBEScripts.Exceptions;

namespace net.r_eg.vsSBE.SBEScripts.Components.NuGet.GetNuTool
{
    public class GNT: Core
    {
        /// <summary>
        /// Target by default
        /// </summary>
        public const string DEF_TARGET = "get";

        /// <summary>
        /// Debug mode for core
        /// </summary>
        protected bool debug;

        /// <summary>
        /// Raw command as for original GetNuTool
        /// https://github.com/3F/GetNuTool
        /// </summary>
        /// <param name="data"></param>
        public void raw(string data)
        {
            Match mtarget = Regex.Match(data, @"\/t(?:arget)?:(\w+)");
            string target = (!mtarget.Success)? DEF_TARGET : mtarget.Groups[1].Value;

            // arguments for target
            var props = Regex.Matches(data, @"\/p(?:roperty)?
                                              :(?'name'\w+)
                                              \s*=\s*
                                              (?:
                                                ""(?'sval'.+?)""
                                                   (?=
                                                      (?:
                                                          \s+\/[a-zA-Z]+?:
                                                        |
                                                          \s*$
                                                      )
                                                    )
                                              |
                                                (?'val'[^\s]+)
                                              )",
                                              RegexOptions.IgnorePatternWhitespace);

            var prop = new Dictionary<string, string>();
            foreach(Match p in props) {
                prop[p.Groups["name"].Value] = p.Groups[p.Groups["val"].Success ? "val" : "sval"].Value;
            }

            Func<string, string> _ = delegate (string key) {
                return prop.ContainsKey(key) ? prop[key] : null;
            };

            debug = Value.toBoolean(_("debug") ?? "false");

            switch(target) {
                case "get": {
                    getCommand(_("ngpackages"), _("ngconfig"), _("ngpath"), _("ngserver"));
                    return;
                }
                case "pack": {
                    packCommand(_("ngin"), _("ngout"));
                    return;
                }
            }

            throw new OperandNotFoundException("`{0}` Can't find command - `{1}` :: raw({2})", ToString(), target, data);
        }

        /// <param name="packages"></param>
        /// <param name="config"></param>
        /// <param name="path"></param>
        /// <param name="server"></param>
        public void getCommand(string packages, string config = null, string path = null, string server = null)
        {
            string list = prepareList(config ?? String.Empty, packages ?? String.Empty);
            downloader(list, server ?? ngserver, path ?? ngpath, debug);
        }

        /// <param name="dir"></param>
        /// <param name="dout"></param>
        public void packCommand(string dir, string dout)
        {
            packing(location(dir), location(dout), debug);
        }

        /// <summary>
        /// Gets path from working directory to selected item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override string location(string item)
        {
            return Path.Combine(Settings.WPath, item ?? String.Empty);
        }
    }
}