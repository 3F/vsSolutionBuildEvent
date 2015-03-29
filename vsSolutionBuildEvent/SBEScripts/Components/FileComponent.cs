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
using System.Collections.Generic;
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
    [Component("File", "I/O operations")]
    public class FileComponent: Component, IComponent
    {
        /// <summary>
        /// Default limit in seconds for execution processes.
        /// </summary>
        public const uint STCALL_TIMEOUT_DEFAULT = 10;

        /// <summary>
        /// Ability to work with data for current component
        /// </summary>
        public override string Condition
        {
            get { return "File "; }
        }

        /// <summary>
        /// Gets all directories from the PATH of the Environment
        /// </summary>
        protected IEnumerable<string> EnvPath
        {
            get
            {
                if(envPath == null) {
                    envPath = String.Format("{0};{1};{2};{3};{4}", 
                                            System.Environment.SystemDirectory,
                                            System.Environment.GetEnvironmentVariable("SystemRoot"),
                                            System.Environment.GetEnvironmentVariable("SystemRoot") + @"\System32\Wbem",
                                            System.Environment.GetEnvironmentVariable("SystemRoot") + @"\System32\WindowsPowerShell\v1.0\",
                                            System.Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Process)).Split(';');
                }

                foreach(string dir in envPath) {
                    yield return dir;
                }
            }
        }
        protected string[] envPath = null;


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
                                                \s*
                                                [.(=]
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
                    return stGet(ident);
                }
                case "call": {
                    return stCall(ident, false, false);
                }
                case "out": { // is obsolete
                    //return stCall(ident, true, false);
                    return stCall(ident, true, true); // redirect to sout
                }
                case "scall": {
                    return stCall(ident, false, true);
                }
                case "sout": {
                    return stCall(ident, true, true);
                }
                case "write": {
                    stWrite(ident, false, false);
                    return String.Empty;
                }
                case "append": {
                    stWrite(ident, true, false);
                    return String.Empty;
                }
                case "writeLine": {
                    stWrite(ident, false, true);
                    return String.Empty;
                }
                case "appendLine": {
                    stWrite(ident, true, true);
                    return String.Empty;
                }
                case "replace": {
                    stReplace(ident);
                    return String.Empty;
                }
                case "exists": {
                    return stExists(ident);
                }
                case "cmd": {
                    return stCmd(ident);
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
                "Gets all data from file.", 
                new string[] { "name" }, 
                new string[] { "File name" }, 
                CValueType.String, 
                CValueType.String
            )
        ]
        protected string stGet(string data)
        {
            Log.nlog.Trace("FileComponent: use stGet");
            Match m = Regex.Match(data, 
                                    String.Format(@"get
                                                    \s*
                                                    \({0}\)   #1 - file", 
                                                    RPattern.DoubleQuotesContent
                                                  ), RegexOptions.IgnorePatternWhitespace);

            if(!m.Success) {
                throw new SyntaxIncorrectException("Failed stGet - '{0}'", data);
            }

            string file = location(StringHandler.normalize(m.Groups[1].Value.Trim()));
            Log.nlog.Debug("FileComponent: ready for '{0}'", file);

            try {
                return readToEnd(file, detectEncodingFromFile(file));
            }
            catch(FileNotFoundException ex) {
                throw new ScriptException("stGet: not found - '{0}' :: {1}", file, ex.Message);
            }
            catch(Exception ex) {
                throw new ScriptException("stGet: exception - '{0}'", ex.Message);
            }
        }

        /// <summary>
        /// Handler for:
        /// * #[File call(..)] + for silent mode - #[File scall(..)] + #[File sout(..)]
        /// 
        /// NOTE: All errors can be ~disabled with arguments, for example:
        ///       * stderr to stdout: [command] 2>&amp;1
        ///       * stderr to nul i.e. as disabled: [command] 2>nul
        /// </summary>
        /// <param name="data">prepared data</param>
        /// <param name="stdOut">Use StandardOutput or not</param>
        /// <param name="silent">Silent mode</param>
        /// <returns>Received data from StandardOutput</returns>
        [Method (
            "call", 
            "Caller of executable files.", 
            new string[] { "name" }, 
            new string[] { "Executable file" }, 
            CValueType.Void, 
            CValueType.String
        )]
        [Method (
            "call", 
            "Caller of executable files with arguments.", 
            new string[] { "name", "args" }, 
            new string[] { "Executable file", "Arguments" }, 
            CValueType.Void, 
            CValueType.String, CValueType.String
        )]
        [Method(
            "call",
            "Caller of executable files with arguments and with timeout configuration.",
            new string[] { "name", "args", "timeout" },
            new string[] { "Executable file", "Arguments", "How long to wait the execution, in seconds. 0 value - infinitely" },
            CValueType.Void,
            CValueType.String, CValueType.String, CValueType.UInteger
        )]
        [Method (
            "scall", 
            "Caller of executable files in silent mode.", 
            new string[] { "name" }, 
            new string[] { "Executable file" }, 
            CValueType.Void, 
            CValueType.String
        )]
        [Method (
            "scall", 
            "Caller of executable files in silent mode with arguments.", 
            new string[] { "name", "args" }, 
            new string[] { "Executable file", "Arguments" }, 
            CValueType.Void, 
            CValueType.String, CValueType.String
        )]
        [Method(
            "scall",
            "Caller of executable files in silent mode with arguments and with timeout configuration.",
            new string[] { "name", "args", "timeout" },
            new string[] { "Executable file", "Arguments", "How long to wait the execution, in seconds. 0 value - infinitely" },
            CValueType.Void,
            CValueType.String, CValueType.String, CValueType.UInteger
        )]
        [Method (
            "sout",
            "Receives data from standard streams for executed file.\nTo disable errors use the '2>nul' and similar.", 
            new string[] { "name" }, 
            new string[] { "Executable file" },
            CValueType.String, 
            CValueType.String
        )]
        [Method (
            "sout",
            "Receives data from standard streams for executed file with arguments.\nTo disable errors use the '2>nul' and similar.", 
            new string[] { "name", "args" }, 
            new string[] { "Executable file", "Arguments" },
            CValueType.String, 
            CValueType.String, CValueType.String
        )]
        [Method(
            "sout",
            "Receives data from standard streams for executed file with arguments and with timeout configuration.\nTo disable errors use the '2>nul' and similar.",
            new string[] { "name", "args", "timeout" },
            new string[] { "Executable file", "Arguments", "How long to wait the execution, in seconds. 0 value - infinitely" },
            CValueType.String,
            CValueType.String, CValueType.String, CValueType.UInteger
        )]
        protected string stCall(string data, bool stdOut, bool silent)
        {
            Log.nlog.Trace("FileComponent: use stCall");
            Match m = Regex.Match(data, 
                                    String.Format(@"
                                                    \s*
                                                    \(
                                                        {0}           #1 - file
                                                        (?:
                                                            ,
                                                            {0}       #2 - args (optional)
                                                            (?:
                                                                ,
                                                                {1}   #3 - timeout (optional)
                                                            )?
                                                        )?
                                                    \)", 
                                                    RPattern.DoubleQuotesContent,
                                                    RPattern.UnsignedIntegerContent
                                                 ), RegexOptions.IgnorePatternWhitespace);

            if(!m.Success) {
                throw new SyntaxIncorrectException("Failed stCall - '{0}'", data);
            }

            string file     = StringHandler.normalize(m.Groups[1].Value.Trim());
            string args     = StringHandler.normalize(m.Groups[2].Value);
            uint timeout    = (m.Groups[3].Success)? Value.toUInt32(m.Groups[3].Value) : STCALL_TIMEOUT_DEFAULT;

            Log.nlog.Debug("stCall: '{0}', '{1}' :: stdOut {2}, silent {3}", file, args, stdOut, silent);

            string pfile = findFile(file);
            if(String.IsNullOrEmpty(pfile)) {
                throw new NotFoundException("FileComponent: File '{0}' not found", file);
            }

            try {
                string ret = run(pfile, args, silent, stdOut, (int)timeout);
                Log.nlog.Debug("FileComponent: successful stCall - '{0}'", pfile);
                return ret;
            }
            catch(Exception ex) {
                throw new ScriptException("stCall: exception - '{0}'", ex.Message);
            }
        }

        /// <summary>
        /// Alias to sout() for cmd
        /// - #[File cmd("args")] -> #[File sout("cmd", "/C args")]
        /// </summary>
        /// <param name="data"></param>
        /// <param name="silent"></param>
        /// <returns></returns>
        [Method(
            "cmd",
            "Alias to #[File sout(\"cmd\", \"/C args\")] \nReceives data from standard streams for cmd process with arguments.",
            new string[] { "args" },
            new string[] { "Arguments" },
            CValueType.String,
            CValueType.String
        )]
        [Method(
            "cmd",
            "Alias to #[File sout(\"cmd\", \"/C args\", timeout)] \nReceives data from standard streams for cmd process with arguments and with timeout configuration.",
            new string[] { "args", "timeout" },
            new string[] { "Arguments", "How long to wait the execution, in seconds. 0 value - infinitely" },
            CValueType.String,
            CValueType.String, CValueType.UInteger
        )]
        protected string stCmd(string data)
        {
            Log.nlog.Trace("FileComponent: use stCmd");
            Log.nlog.Trace("stCmd redirect to stCall");
            string rmod = String.Format("(\"cmd\", \"/C {0}", Regex.Match(data, @"\(\s*""(.*)$").Groups[1].Value);

            Log.nlog.Debug("stCmd: '{0}' -> '{1}'", data, rmod);
            return stCall(rmod, true, true);
        }

        /// <summary>
        /// Handler for:
        /// * write(), writeLine(), append(), appendLine()
        /// * * #[File write("name", append, line, "encoding"): multiline data]
        /// * * #[File write("..."): multiline data];
        /// </summary>
        /// <param name="data">prepared data</param>
        /// <param name="append">flag</param>
        /// <param name="writeLine">writes with CR?/LF</param>
        /// <param name="enc">Used encoding</param>
        [Method (
            "write", 
            "Writes text data in file.\n * Creates if the file does not exist.\n * Overwrites content if already exist.",
            new string[] { "name", "In" }, 
            new string[] { "File name", "multiline data" },
            CValueType.Void, 
            CValueType.String, CValueType.Input
        )]
        [Method (
            "write",
            "Writes text data in file with selected encoding and with flags: CR/LF & append.\n * Creates if the file does not exist.",
            new string[] { "name", "append", "line", "encoding", "In" },
            new string[] { "File name", "Flag of adding data to the end file", "Adds a line terminator", "Code page name of the preferred encoding", "multiline data" },
            CValueType.Void,
            CValueType.String, CValueType.Boolean, CValueType.Boolean, CValueType.String, CValueType.Input
        )]
        [Method (
            "append",
            "Writes text data in file.\n * Creates if the file does not exist.\n * Adds data to the end file if it already exist.",
            new string[] { "name", "In" }, 
            new string[] { "File name", "multiline data" },
            CValueType.Void, 
            CValueType.String, CValueType.Input
        )]
        [Method (
            "writeLine", 
            "Writes text data with CR/LF in file.\n * Creates if the file does not exist.\n * Overwrites content if already exist.", 
            new string[] { "name", "In" }, 
            new string[] { "File name", "multiline data" },
            CValueType.Void, 
            CValueType.String, CValueType.Input
        )]
        [Method (
            "appendLine", 
            "Writes text data with CR/LF in file.\n * Creates if the file does not exist.\n * Adds data to the end file if it already exist.", 
            new string[] { "name", "In" }, 
            new string[] { "File name", "multiline data" },
            CValueType.Void, 
            CValueType.String, CValueType.Input
        )]
        [Method (
            "write",
            "Writes text data in standard stream.\n * STDOUT - Standard output stream.\n * STDERR - Standard error stream.",
            new string[] { "std", "In" },
            new string[] { "Constant of standard stream", "multiline data" },
            CValueType.Void,
            CValueType.Const, CValueType.Input
        )]
        [Method (
            "writeLine",
            "Writes text data with CR/LF in standard stream.\n * STDOUT - Standard output stream.\n * STDERR - Standard error stream.",
            new string[] { "std", "In" },
            new string[] { "Constant of standard stream", "multiline data" },
            CValueType.Void,
            CValueType.Const, CValueType.Input
        )]
        protected void stWrite(string data, bool append, bool writeLine, Encoding enc)
        {
            Log.nlog.Trace("FileComponent: use stWrite [{0}, {1}]", append, writeLine);
            Match m = Regex.Match(data, 
                                    String.Format(@"
                                                    (\S+)        #1 - type
                                                    \s*
                                                    \(
                                                       (?: 
                                                          (STDOUT|STDERR)  #2 - standard streams
                                                           |
                                                          {0}              #3 - file
                                                       )
                                                       (?:
                                                          ,{1}   #4 - append    (optional)
                                                          ,{1}   #5 - line      (optional)
                                                          ,{0}   #6 - encoding  (optional)
                                                       )?
                                                    \)
                                                    \s*:
                                                    (.*)         #7 - data", 
                                                    RPattern.DoubleQuotesContent,
                                                    RPattern.BooleanContent
                                                 ), RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline);

            if(!m.Success) {
                throw new SyntaxIncorrectException("Failed stWrite - '{0}'", data);
            }

            string type         = m.Groups[1].Value;
            bool stdStream      = m.Groups[2].Success;
            string file         = (stdStream)? m.Groups[2].Value : location(StringHandler.normalize(m.Groups[3].Value.Trim()));
            string appendUser   = (m.Groups[4].Success)? m.Groups[4].Value : null;
            string lineUser     = (m.Groups[5].Success)? m.Groups[5].Value : null;
            string encUser      = (m.Groups[6].Success)? m.Groups[6].Value : null;
            string fdata        = StringHandler.hSymbols(m.Groups[7].Value);

            if(appendUser != null)
            {
                if(type != "write" || lineUser == null || encUser == null) {
                    throw new SyntaxIncorrectException("Failed stWrite :: write - '{0}'", data);
                }

                Log.nlog.Debug("FileComponent: user params: append={0}; line={1}; enc={2};", appendUser, lineUser, encUser);
                append      = Value.toBoolean(appendUser);
                writeLine   = Value.toBoolean(lineUser);
                enc         = Encoding.GetEncoding(encUser);
            }

            Log.nlog.Debug("FileComponent: stWrite started for '{0}'({1})", file, stdStream);
            try
            {
                if(!stdStream) {
                    writeToFile(file, fdata, append, writeLine, enc);
                    Log.nlog.Trace("FileComponent: successful stWrite - '{0}'", file);
                    return;
                }

                if(file == "STDERR") {
                    writeToStdErr(fdata, writeLine);
                }
                else {
                    writeToStdOut(fdata, writeLine);
                }
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
        protected void stReplace(string data)
        {
            Log.nlog.Trace("FileComponent: use stReplace");
            Match m = Regex.Match(data, 
                                    String.Format(@"replace
                                                    (?:
                                                    \s*\.\s*
                                                      (Regexp?|Wildcards)  #1 - Search with type (optional)
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

            string type         = (m.Groups[1].Success)? m.Groups[1].Value : "Basic";
            string file         = location(StringHandler.normalize(m.Groups[2].Value.Trim()));
            string pattern      = StringHandler.normalize(m.Groups[3].Value);
            string replacement  = StringHandler.hSymbols(StringHandler.normalize(m.Groups[4].Value));

            Log.nlog.Debug("stReplace: found file '{0}',  pattern '{1}',  replacement '{2}'", file, pattern, replacement);

            Encoding enc = detectEncodingFromFile(file);
            string content = readToEnd(file, enc);

            Log.nlog.Debug("stReplace: type '{0}' :: received '{1}', Encoding '{2}'", type, content.Length, enc);
            content = stReplaceEngine(type, ref content, pattern, replacement);

            writeToFile(file, content, false, enc);
            Log.nlog.Debug("stReplace: successful :: {0}, Encoding '{1}'", content.Length, enc);
        }

        /// <summary>
        /// 
        /// Work with:
        /// * #[File replace("file", "pattern", "replacement")]
        /// * #[File replace.Regex("file", "pattern", "replacement")]
        /// * #[File replace.Regexp("file", "pattern", "replacement")]
        /// * #[File replace.Wildcards("file", "pattern", "replacement")]
        /// </summary>
        /// <param name="type"></param>
        /// <param name="content"></param>
        /// <param name="pattern"></param>
        /// <param name="replacement"></param>
        /// <returns></returns>
        [Property("replace", "Provides the additional replacement methods")]
        [
            Method
            (
                "Regex",
                "Alias for Regexp", 
                "replace",
                "stReplaceEngine", 
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
                "stReplaceEngine",
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
                "stReplaceEngine",
                new string[] { "file", "pattern", "replacement" },
                new string[] { "Input file", "Pattern with wildcards", "Replacement string" },
                CValueType.Void, 
                CValueType.String, CValueType.String, CValueType.String
            )
        ]
        protected string stReplaceEngine(string type, ref string content, string pattern, string replacement)
        {
            switch(type) {
                case "Regex":
                case "Regexp": {
                    return Regex.Replace(content, pattern, replacement);
                }
                case "Wildcards": {
                    string stub = Regex.Escape(pattern).Replace("\\*", ".*?").Replace("\\+", ".+?").Replace("\\?", ".");
                    return Regex.Replace(content, stub, replacement);
                }
                default: {
                    return content.Replace(pattern, replacement);
                }
            }
        }

        /// <summary>
        /// Determines whether the something exists.
        /// 
        /// Work with:
        ///  * #[File exists.directory("path")]
        ///  * #[File exists.directory("path", false)]
        ///  * #[File exists.file("path")]
        ///  * #[File exists.file("path", true)]
        /// </summary>
        /// <param name="data">prepared data</param>
        /// <returns></returns>
        [Property("exists", "Determines whether the something exists.")]
        [
            Method
            (
                "directory",
                "Determines whether the given path refers to an existing directory on disk.",
                "exists",
                "stExists",
                new string[] { "path" },
                new string[] { "Path to test" },
                CValueType.Boolean,
                CValueType.String
            ),
        ]
        [
            Method
            (
                "directory",
                "Determines whether the given path refers to an existing directory on disk with searching in environment.",
                "exists",
                "stExists",
                new string[] { "path", "environment" },
                new string[] { "Path to test", "Using the PATH of the Environment for searching. Environment associated with the current process." },
                CValueType.Boolean,
                CValueType.String, CValueType.Boolean
            ),
        ]
        [
            Method
            (
                "file",
                "Determines whether the specified file exists.",
                "exists",
                "stExists",
                new string[] { "path" },
                new string[] { "The file to check" },
                CValueType.Boolean,
                CValueType.String
            ),
        ]
        [
            Method
            (
                "file",
                "Determines whether the specified file exists with searching in environment.",
                "exists",
                "stExists",
                new string[] { "path", "environment" },
                new string[] { "The file to check", "Using the PATH of the Environment for searching. Environment associated with the current process." },
                CValueType.Boolean,
                CValueType.String, CValueType.Boolean
            ),
        ]
        protected string stExists(string data)
        {
            Log.nlog.Trace("FileComponent: use stExists");
            Match m = Regex.Match(data,
                                    String.Format(@"exists
                                                    \s*\.\s*
                                                    (directory|file) #1 - type
                                                    \s*
                                                    \(
                                                       {0}           #2 - path to test
                                                       (?:,{1})?     #3 - flag of searching in environment (optional)
                                                    \)
                                                    ",
                                                    RPattern.DoubleQuotesContent,
                                                    RPattern.BooleanContent
                                                 ), RegexOptions.IgnorePatternWhitespace);

            if(!m.Success) {
                throw new SyntaxIncorrectException("Failed stExists - '{0}'", data);
            }

            string type         = m.Groups[1].Value;
            string find         = m.Groups[2].Value;
            string environment  = (m.Groups[3].Success)? m.Groups[3].Value : null;


            if(environment == null || !Value.toBoolean(environment)) {
                return Value.from((type == "file")? File.Exists(location(find)) : Directory.Exists(location(find)));
            }

            foreach(string dir in EnvPath)
            {
                bool found = (type == "file")? File.Exists(location(find, dir)) : Directory.Exists(location(find, dir));
                if(found) {
                    return Value.from(true);
                }
            }
            return Value.from(false);
        }

        /// <summary>
        /// Reads the entire file
        /// </summary>
        /// <param name="file">The file to be read</param>
        /// <param name="enc">The character encoding to use</param>
        /// <param name="detectEncoding">Indicates whether to look for byte order marks at the beginning of the file</param>
        /// <returns></returns>
        protected virtual string readToEnd(string file, Encoding enc, bool detectEncoding = false)
        {
            using(StreamReader stream = new StreamReader(file, enc, detectEncoding)) {
                return stream.ReadToEnd();
            }
        }

        protected string readToEnd(string file)
        {
            return readToEnd(file, Encoding.UTF8, true);
        }

        /// <param name="file">File path to write</param>
        /// <param name="data">The string to write</param>
        /// <param name="append">Determines whether data is to be appended to the file</param>
        /// <param name="writeLine">Writes a string followed by a line terminator if true</param>
        /// <param name="enc">The character encoding to use</param>
        protected virtual void writeToFile(string file, string data, bool append, bool writeLine, Encoding enc)
        {
            Log.nlog.Debug("writeToFile: Encoding '{0}'", enc.EncodingName);
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
        /// Writes to standard output stream.
        /// </summary>
        /// <param name="data">The string to write</param>
        /// <param name="writeLine">Writes a string followed by a line terminator if true</param>
        protected void writeToStdOut(string data, bool writeLine)
        {
            if(writeLine) {
                Console.Out.WriteLine(data);
            }
            else {
                Console.Out.Write(data);
            }
        }

        /// <summary>
        /// Writes to standard error stream.
        /// </summary>
        /// <param name="data">The string to write</param>
        /// <param name="writeLine">Writes a string followed by a line terminator if true</param>
        protected void writeToStdErr(string data, bool writeLine)
        {
            if(writeLine) {
                Console.Error.WriteLine(data);
            }
            else {
                Console.Error.Write(data);
            }
        }

        /// <summary>
        /// Auto detecting the encoding from the file
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        protected virtual Encoding detectEncodingFromFile(string file)
        {
            using(FileStream fs = File.OpenRead(file))
            {
                Ude.CharsetDetector cdet = new Ude.CharsetDetector();
                cdet.Feed(fs);
                cdet.DataEnd();

                if(cdet.Charset == null) {
                    //throw new ComponentException("Ude: Detection failed for '{0}'", file);
                    Log.nlog.Warn("Problem with detection of encoding for '{0}'", file);
                    return Encoding.UTF8; // good luck
                }

                Log.nlog.Debug("Ude: charset '{0}' confidence: '{1}'", cdet.Charset, cdet.Confidence);
                return Encoding.GetEncoding(cdet.Charset);
            }
        }

        /// <summary>
        /// Execute file with arguments
        /// </summary>
        /// <param name="file"></param>
        /// <param name="args"></param>
        /// <param name="silent">Hide process if true</param>
        /// <param name="stdOut">Reads from StandardOutput if true</param>
        /// <param name="timeout">How long to wait the execution, in seconds. 0 value - infinitely</param>
        /// <returns></returns>
        protected virtual string run(string file, string args, bool silent, bool stdOut, int timeout = 0)
        {
            string ret = (new Actions.HProcess(Settings.WorkingPath)).run(file, args, silent, timeout);
            return (stdOut)? ret : String.Empty;
        }

        /// <summary>
        /// Gets full path to file in found directory
        /// </summary>
        /// <param name="file"></param>
        /// <returns>null value if not found in any places</returns>
        protected virtual string findFile(string file)
        {
            string lfile = location(file);
            if(File.Exists(lfile)) {
                return lfile;
            }
            Log.nlog.Trace("trying to find the file '{0}' with environment PATH :: '{1}'", file, lfile);

            string[] exts = System.Environment.GetEnvironmentVariable("PATHEXT").Split(';');
            foreach(string dir in EnvPath)
            {
                lfile = location(file, dir);
                if(File.Exists(lfile) || exts.Any(ext => File.Exists(lfile + ext))) {
                    Log.nlog.Trace("found in: '{0}' :: '{1}'", dir, lfile);
                    return lfile;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets location for specific path
        /// </summary>
        /// <param name="file"></param>
        /// <param name="path"></param>
        /// <returns>Absolute path to file</returns>
        protected string location(string file, string path)
        {
            return Path.Combine(path, file);
        }

        /// <summary>
        /// Gets location for current context
        /// </summary>
        /// <param name="file"></param>
        /// <returns>Absolute path to file</returns>
        protected string location(string file)
        {
            return Path.Combine(Settings.WorkingPath, file);
        }
    }
}
