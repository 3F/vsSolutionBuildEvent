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
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using net.r_eg.vsSBE.Exceptions;
using net.r_eg.vsSBE.SBEScripts.Dom;
using net.r_eg.vsSBE.SBEScripts.Exceptions;

namespace net.r_eg.vsSBE.SBEScripts.Components
{
    /// <summary>
    /// Support file operations
    /// I/O, call, etc.
    /// </summary>
    [Definition("File", "I/O operations")]
    public class FileComponent: Component, IComponent
    {
        /// <summary>
        /// Ability to work with data for current component
        /// </summary>
        public override string Condition
        {
            get { return "File "; }
        }

        /// <summary>
        /// Handler for current data
        /// </summary>
        /// <param name="data">mixed data</param>
        /// <returns>prepared and evaluated data</returns>
        public override string parse(string data)
        {
            Match m = Regex.Match(data, @"^\[File
                                              \s+
                                              (                  #1 - full ident
                                                ([A-Za-z_0-9]+)  #2 - subtype
                                                .*
                                              )
                                           \]$", 
                                           RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline);

            if(!m.Success) {
                throw new SyntaxIncorrectException("Failed FileComponent - '{0}'", data);
            }
            string ident = m.Groups[1].Value;

            switch(m.Groups[2].Value) {
                case "get": {
                    Log.nlog.Debug("FileComponent: use stGet");
                    return stGet(ident);
                }
                case "call": {
                    Log.nlog.Debug("FileComponent: use stCall");
                    return stCall(ident, false, false);
                }
                case "out": {
                    Log.nlog.Debug("FileComponent: use stCall");
                    return stCall(ident, true, false);
                }
                case "scall": {
                    Log.nlog.Debug("FileComponent: use stCall");
                    return stCall(ident, false, true);
                }
                case "sout": {
                    Log.nlog.Debug("FileComponent: use stCall");
                    return stCall(ident, true, true);
                }
                case "write": {
                    Log.nlog.Debug("FileComponent: use stWrite");
                    stWrite(ident, false, false);
                    return String.Empty;
                }
                case "append": {
                    Log.nlog.Debug("FileComponent: use stWrite + append");
                    stWrite(ident, true, false);
                    return String.Empty;
                }
                case "writeLine": {
                    Log.nlog.Debug("FileComponent: use stWrite + line");
                    stWrite(ident, false, true);
                    return String.Empty;
                }
                case "appendLine": {
                    Log.nlog.Debug("FileComponent: use stWrite + append + line");
                    stWrite(ident, true, true);
                    return String.Empty;
                }
                case "replace": {
                    Log.nlog.Debug("FileComponent: use stReplace");
                    stReplace(ident);
                    return String.Empty;
                }
            }
            throw new SubtypeNotFoundException("FileComponent: not found subtype - '{0}'", m.Groups[2].Value);
        }

        /// <summary>
        /// Work with:
        /// * #[File get("name")]
        /// </summary>
        /// <param name="data">prepared data</param>
        /// <returns>Received data from file</returns>
        [
            Method
            (
                "get", 
                "Receiving data from file.", 
                new string[] { "name" }, 
                new string[] { "File name" }, 
                CValueType.String, 
                CValueType.String
            )
        ]
        protected string stGet(string data)
        {
            Match m = Regex.Match(data, 
                                    String.Format(@"get
                                                    \s*
                                                    \({0}\)   #1 - file", 
                                                    RPattern.DoubleQuotesContent
                                                  ), RegexOptions.IgnorePatternWhitespace);

            if(!m.Success) {
                throw new SyntaxIncorrectException("Failed stGet - '{0}'", data);
            }

            string file     = location(StringHandler.normalize(m.Groups[1].Value.Trim()));
            string content  = String.Empty;
            try {
                content = readToEnd(file, Encoding.UTF8, true);
                Log.nlog.Debug("FileComponent: successful stGet- '{0}'", file);
            }
            catch(FileNotFoundException ex) {
                throw new ScriptException("stGet: not found - '{0}' :: {1}", file, ex.Message);
            }
            catch(Exception ex) {
                throw new ScriptException("stGet: exception - '{0}'", ex.Message);
            }
            return content;
        }

        /// <summary>
        /// 
        /// Work with:
        /// * #[File call("name", "args")]
        /// * #[File call("name")]
        /// * #[File out("name", "args")]
        /// * #[File out("name")]
        /// 
        /// For silent mode use the scall(..) & sout(..)
        /// </summary>
        /// <param name="data">prepared data</param>
        /// <param name="stdOut">Use StandardOutput or not</param>
        /// <param name="silent">Silent mode</param>
        /// <returns>Received data from StandardOutput</returns>
        [
            Method
            (
                "call", 
                "Caller of executable files.", 
                new string[] { "name" }, 
                new string[] { "Executable file" }, 
                CValueType.Void, 
                CValueType.String
            )
        ]
        [
            Method
            (
                "call", 
                "Caller of executable files with arguments.", 
                new string[] { "name", "args" }, 
                new string[] { "Executable file", "Arguments" }, 
                CValueType.Void, 
                CValueType.String, CValueType.String
            )
        ]
        [
            Method
            (
                "scall", 
                "Caller of executable files in silent mode.", 
                new string[] { "name" }, 
                new string[] { "Executable file" }, 
                CValueType.Void, 
                CValueType.String
            )
        ]
        [
            Method
            (
                "scall", 
                "Caller of executable files in silent mode with arguments.", 
                new string[] { "name", "args" }, 
                new string[] { "Executable file", "Arguments" }, 
                CValueType.Void, 
                CValueType.String, CValueType.String
            )
        ]
        [
            Method
            (
                "out", 
                "Receiving data from stdout of executed file.", 
                new string[] { "name" }, 
                new string[] { "Executable file" }, 
                CValueType.Void, 
                CValueType.String
            )
        ]
        [
            Method
            (
                "out", 
                "Receiving data from stdout of executed file with arguments.", 
                new string[] { "name", "args" }, 
                new string[] { "Executable file", "Arguments" }, 
                CValueType.Void, 
                CValueType.String, CValueType.String
            )
        ]
        [
            Method
            (
                "sout", 
                "Receiving data from stdout of executed file in silent mode.", 
                new string[] { "name" }, 
                new string[] { "Executable file" }, 
                CValueType.Void, 
                CValueType.String
            )
        ]
        [
            Method
            (
                "sout", 
                "Receiving data from stdout of executed file in silent mode with arguments.", 
                new string[] { "name", "args" }, 
                new string[] { "Executable file", "Arguments" }, 
                CValueType.Void, 
                CValueType.String, CValueType.String
            )
        ]
        protected string stCall(string data, bool stdOut, bool silent)
        {
            Match m = Regex.Match(data, 
                                    String.Format(@"
                                                    \s*
                                                    \(
                                                        {0}           #1 - file
                                                        (?:
                                                            ,
                                                            {0}       #2 - args (optional)
                                                        )?
                                                    \)", 
                                                    RPattern.DoubleQuotesContent
                                                 ), RegexOptions.IgnorePatternWhitespace);

            if(!m.Success) {
                throw new SyntaxIncorrectException("Failed stCall - '{0}'", data);
            }

            string file = StringHandler.normalize(m.Groups[1].Value.Trim());
            string args = StringHandler.normalize(m.Groups[2].Value);

            Log.nlog.Debug("stCall: '{0}', '{1}' :: stdOut {2}, silent {3}", file, args, stdOut, silent);
            try {
                string ret = run(file, args, silent, stdOut);
                Log.nlog.Debug("FileComponent: successful stCall - '{0}'", file);
                return ret;
            }
            catch(Exception ex) {
                throw new ScriptException("stCall: exception - '{0}'", ex.Message);
            }
        }

        /// <summary>
        /// Work with:
        /// * #[File write("name"): multiline data]
        /// * #[File append("name"): multiline data]
        /// * #[File writeLine("name"): multiline data]
        /// * #[File appendLine("name"): multiline data]
        /// </summary>
        /// <param name="data">prepared data</param>
        /// <param name="append">flag</param>
        /// <param name="writeLine">writes with CR?/LF</param>
        /// <param name="enc">Used encoding</param>
        [
            Method
            (
                "write", 
                "Writes text data in file.\n * Creates if the file does not exist.\n * Overwrites content if already exist.",
                new string[] { "name", "In" }, 
                new string[] { "File name", "multiline data" },
                CValueType.Void, 
                CValueType.String, CValueType.Input
            )
        ]
        [
            Method
            (
                "append",
                "Writes text data in file.\n * Creates if the file does not exist.\n * Adds data to the end file if it already exist.",
                new string[] { "name", "In" }, 
                new string[] { "File name", "multiline data" },
                CValueType.Void, 
                CValueType.String, CValueType.Input
            )
        ]
        [
            Method
            (
                "writeLine", 
                "Writes text data with CR/LF in file.\n * Creates if the file does not exist.\n * Overwrites content if already exist.", 
                new string[] { "name", "In" }, 
                new string[] { "File name", "multiline data" },
                CValueType.Void, 
                CValueType.String, CValueType.Input
            )
        ]
        [
            Method
            (
                "appendLine", 
                "Writes text data with CR/LF in file.\n * Creates if the file does not exist.\n * Adds data to the end file if it already exist.", 
                new string[] { "name", "In" }, 
                new string[] { "File name", "multiline data" },
                CValueType.Void, 
                CValueType.String, CValueType.Input
            )
        ]
        protected void stWrite(string data, bool append, bool writeLine, Encoding enc)
        {
            Match m = Regex.Match(data, 
                                    String.Format(@"
                                                    \s*
                                                    \({0}\)  #1 - file
                                                    \s*:
                                                    (.*)     #2 - data", 
                                                    RPattern.DoubleQuotesContent
                                                 ), RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline);

            if(!m.Success) {
                throw new SyntaxIncorrectException("Failed stWrite - '{0}'", data);
            }

            string file     = location(StringHandler.normalize(m.Groups[1].Value.Trim()));
            string fdata    = StringHandler.hSymbols(m.Groups[2].Value);

            Log.nlog.Debug("FileComponent: stWrite started for '{0}'", file);
            try {
                writeToFile(file, fdata, append, writeLine, enc);
                Log.nlog.Debug("FileComponent: successful stWrite - '{0}'", file);
            }
            catch(Exception ex) {
                throw new ScriptException("FileComponent: Cannot write {0}", ex.Message);
            }
        }

        protected void stWrite(string data, bool append, bool writeLine)
        {
            stWrite(data, append, writeLine, Encoding.UTF8);
        }

        /// <summary>
        /// 
        /// Work with:
        /// * #[File replace.Regex("file", "pattern", "replacement")]
        /// * #[File replace.Wildcards("file", "pattern", "replacement")]
        /// * #[File replace("file", "pattern", "replacement")]
        /// </summary>
        /// <param name="data">prepared data</param>
        [
            Method
            (
                "replace", 
                "Replacing the strings in files.", 
                new string[] { "file", "pattern", "replacement" },
                new string[] { "Input file", "String to comparison", "Replacement string" },
                CValueType.Void, 
                CValueType.String, CValueType.String, CValueType.String
            )
        ]
        [
            Method
            (
                "Regex", 
                "Replacing the strings in files with Regular expression.", 
                "replace", 
                "stReplace", 
                new string[] { "file", "pattern", "replacement" },
                new string[] { "Input file", "Regular expression pattern", "Replacement string" },
                CValueType.Void, 
                CValueType.String, CValueType.String, CValueType.String
            )
        ]
        [
            Method
            (
                "Regexp",
                "Replacing the strings in files with Regular expression.",
                "replace",
                "stReplace",
                new string[] { "file", "pattern", "replacement" },
                new string[] { "Input file", "Regular expression pattern", "Replacement string" },
                CValueType.Void, 
                CValueType.String, CValueType.String, CValueType.String
            )
        ]
        [
            Method
            (
                "Wildcards",
                "Replacing the strings in files with Wildcards.",
                "replace",
                "stReplace",
                new string[] { "file", "pattern", "replacement" },
                new string[] { "Input file", "Pattern with wildcards", "Replacement string" },
                CValueType.Void, 
                CValueType.String, CValueType.String, CValueType.String
            )
        ]
        protected void stReplace(string data)
        {
            Match m = Regex.Match(data, 
                                    String.Format(@"replace
                                                    (?:
                                                    \s*\.\s*
                                                      (Regexp|Wildcards)  #1 - Search with type (optional)
                                                    \s*
                                                    )?
                                                    \(
                                                        {0}               #2 - file
                                                        ,                 
                                                        {0}               #3 - pattern
                                                        ,                 
                                                        {0}               #4 - replacement
                                                    \)", 
                                                    RPattern.DoubleQuotesContent
                                                  ), RegexOptions.IgnorePatternWhitespace);

            if(!m.Success) {
                throw new SyntaxIncorrectException("Failed stReplace - '{0}'", data);
            }

            string type         = "Basic";
            string file         = location(StringHandler.normalize(m.Groups[2].Value.Trim()));
            string pattern      = StringHandler.normalize(m.Groups[3].Value);
            string replacement  = StringHandler.hSymbols(StringHandler.normalize(m.Groups[4].Value));

            Log.nlog.Debug("stReplace: found file '{0}',  pattern '{1}',  replacement '{2}'", file, pattern, replacement);
                        
            Encoding enc = Encoding.UTF8;
            string content = readToEnd(file, out enc);

            if(m.Groups[1].Success) {
                type = m.Groups[1].Value;
            }
            Log.nlog.Debug("stReplace: type '{0}' :: received '{1}', Encoding '{2}'", type, content.Length, enc);

            switch(type) {
                case "Regex":
                case "Regexp": {
                    content = Regex.Replace(content, pattern, replacement);
                    break;
                }
                case "Wildcards": {
                    string stub = Regex.Escape(pattern).Replace("\\*", ".*?").Replace("\\+", ".+?").Replace("\\?", ".");
                    content     = Regex.Replace(content, stub, replacement);
                    break;
                }
                default: {
                    content = content.Replace(pattern, replacement);
                    break;
                }
            }

            writeToFile(file, content, false, enc);
            Log.nlog.Debug("stReplace: successful :: {0}, Encoding '{1}'", content.Length, enc);
        }

        /// <summary>
        /// Reads the entire file
        /// </summary>
        /// <param name="file">The file to be read</param>
        /// <param name="enc">The character encoding to use</param>
        /// <param name="detectEncoding">Indicates whether to look for byte order marks at the beginning of the file</param>
        /// <param name="current">Gets the current character encoding</param>
        /// <returns></returns>
        protected virtual string readToEnd(string file, Encoding enc, bool detectEncoding, out Encoding current)
        {
            using(StreamReader stream = new StreamReader(file, enc, detectEncoding)) {
                current = stream.CurrentEncoding;
                return stream.ReadToEnd();
            }
        }

        protected string readToEnd(string file)
        {
            Encoding current;
            return readToEnd(file, Encoding.UTF8, true, out current);
        }

        protected string readToEnd(string file, Encoding enc, bool detectEncoding = true)
        {
            Encoding current;
            return readToEnd(file, enc, detectEncoding, out current);
        }

        protected string readToEnd(string file, out Encoding enc)
        {
            return readToEnd(file, Encoding.UTF8, true, out enc);
        }

        /// <param name="file">File path to write</param>
        /// <param name="data">The string to write</param>
        /// <param name="append">Determines whether data is to be appended to the file</param>
        /// <param name="writeLine">Writes a string followed by a line terminator if true</param>
        /// <param name="enc">The character encoding to use</param>
        protected virtual void writeToFile(string file, string data, bool append, bool writeLine, Encoding enc)
        {
            using(TextWriter stream = new StreamWriter(file, append, enc)) {
                if(writeLine) {
                    stream.WriteLine(data);
                }
                else {
                    stream.Write(data);
                }
            }
        }

        protected void writeToFile(string file, string data, bool append, bool writeLine)
        {
            writeToFile(file, data, append, writeLine, Encoding.UTF8);
        }

        protected void writeToFile(string file, string data, bool append, Encoding enc)
        {
            writeToFile(file, data, append, false, enc);
        }

        /// <summary>
        /// Execute file with arguments
        /// </summary>
        /// <param name="file"></param>
        /// <param name="args"></param>
        /// <param name="silent">Hide process if true</param>
        /// <param name="stdOut">Reads from StandardOutput if true</param>
        /// <returns></returns>
        protected virtual string run(string file, string args, bool silent, bool stdOut)
        {
            string ret = (new Actions.HProcess(Settings.WorkPath)).run(file, args, silent, null);
            return (stdOut)? ret : String.Empty;
        }

        /// <summary>
        /// File location for current context
        /// </summary>
        /// <param name="file"></param>
        protected string location(string file)
        {
            return Path.Combine(Settings.WorkPath, file);
        }
    }
}
