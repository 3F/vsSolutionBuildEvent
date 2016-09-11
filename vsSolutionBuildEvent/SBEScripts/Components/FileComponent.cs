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
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using net.r_eg.vsSBE.Exceptions;
using net.r_eg.vsSBE.Extensions;
using net.r_eg.vsSBE.SBEScripts.Dom;
using net.r_eg.vsSBE.SBEScripts.Exceptions;
using net.r_eg.vsSBE.SBEScripts.SNode;

namespace net.r_eg.vsSBE.SBEScripts.Components
{
    /// <summary>
    /// This is a very old component, so part of method signatures (user code) are left for compatibility 'as is'.
    /// But, it can be changed for major release...
    /// </summary>
    [Component("File", new string[]{ "IO" }, "I/O File operations.")]
    public class FileComponent: Component, IComponent
    {
        /// <summary>
        /// Default limit in seconds for execution processes.
        /// </summary>
        public const int STCALL_TIMEOUT_DEFAULT = 10;

        /// <summary>
        /// I/O Default encoding.
        /// </summary>
        protected Encoding defaultEncoding = new UTF8Encoding(false);

        /// <summary>
        /// Ability to work with data for current component
        /// </summary>
        public override string Condition
        {
            get { return @"(?:File|IO)\s"; }
        }

        /// <summary>
        /// Use regex engine for the Condition property
        /// </summary>
        public override bool CRegex
        {
            get { return true; }
        }

        protected enum SearchType
        {
            Null,
            Basic,
            Regexp,
            Wildcards
        }

        /// <summary>
        /// Get all directories from Environment PATH
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

        /// <param name="loader">Initialize with loader</param>
        public FileComponent(IBootloader loader)
            : base(loader)
        {

        }

        public FileComponent() { }

        /// <summary>
        /// Handler for current data
        /// </summary>
        /// <param name="data">mixed data</param>
        /// <returns>prepared and evaluated data</returns>
        public override string parse(string data)
        {
            var point       = entryPoint(data, RegexOptions.Singleline);
            string subtype  = point.Key;
            string request  = point.Value;

            Log.Trace("`{0}`: subtype - `{1}`, request - `{2}`", ToString(), subtype, request);

            IPM pm = new PM(request, msbuild);

            if(pm.IsData("get")) {
                return stGet(pm);
            }
            if(pm.IsData("call", "out", "scall", "sout", "cmd")) {
                return stCallFamily(pm);
            }
            if(pm.IsData("write", "append", "writeLine", "appendLine")) {
                return stWriteFamily(pm);
            }
            if(pm.IsData("replace")) {
                return stReplace(pm);
            }
            if(pm.IsData("exists")) {
                return stExists(pm);
            }
            if(pm.IsData("remote")) {
                return stRemote(pm);
            }
            if(pm.IsData("copy")) {
                return stCopy(pm);
            }
            if(pm.IsData("delete")) {
                return stDelete(pm);
            }

            throw new IncorrectNodeException(pm);
        }

        [Method("get",
                "Get all data from text file.", 
                new string[] { "name" }, 
                new string[] { "File name" }, 
                CValueType.String, 
                CValueType.String
        )]
        protected string stGet(IPM pm)
        {
            if(!pm.FinalEmptyIs(LevelType.Method, "get")) {
                throw new IncorrectNodeException(pm);
            }
            ILevel level = pm.FirstLevel;

            if(!level.Is(ArgumentType.StringDouble)) {
                throw new ArgumentPMException(level, "get(string name)");
            }
            string file = location((string)level.Args[0].data);

            try {
                return readToEnd(file, detectEncodingFromFile(file));
            }
            catch(FileNotFoundException ex) {
                throw new ScriptException("File '{0}' is not found :: `{1}`", file, ex.Message);
            }
        }

        /// <param name="pm"></param>
        /// <returns></returns>
        protected string stCallFamily(IPM pm)
        {
            if(pm.FinalEmptyIs(LevelType.Method, "call")) {
                return stCall(pm, false, false);
            }

            if(pm.FinalEmptyIs(LevelType.Method, "out")) {
                //return stCall(pm, true, false); obsolete
                return stCall(pm, true, true); // redirect to sout
            }

            if(pm.FinalEmptyIs(LevelType.Method, "scall")) {
                return stCall(pm, false, true);
            }

            if(pm.FinalEmptyIs(LevelType.Method, "sout")) {
                return stCall(pm, true, true);
            }

            if(pm.FinalEmptyIs(LevelType.Method, "cmd")) {
                return stCmd(pm);
            }

            throw new IncorrectNodeException(pm);
        }

        /// <param name="pm"></param>
        /// <param name="stdOut">Use StandardOutput or not</param>
        /// <param name="silent">Silent mode</param>
        /// <returns>Received data from StandardOutput</returns>
        [Method("call", 
                "Caller of executable files.", 
                new string[] { "name" }, 
                new string[] { "Executable file" }, 
                CValueType.Void, 
                CValueType.String
        )]
        [Method("call", 
                "Caller of executable files with arguments.", 
                new string[] { "name", "args" }, 
                new string[] { "Executable file", "Arguments" }, 
                CValueType.Void, 
                CValueType.String, CValueType.String
        )]
        [Method("call",
                "Caller of executable files with arguments and time limitation settings.",
                new string[] { "name", "args", "timeout" },
                new string[] { "Executable file", "Arguments", "How long to wait the execution, in seconds. 0 value - infinitely" },
                CValueType.Void,
                CValueType.String, CValueType.String, CValueType.Integer
        )]
        [Method("scall", 
                "Caller of executable files in silent mode.", 
                new string[] { "name" }, 
                new string[] { "Executable file" }, 
                CValueType.Void, 
                CValueType.String
        )]
        [Method("scall", 
                "Caller of executable files in silent mode with arguments.", 
                new string[] { "name", "args" }, 
                new string[] { "Executable file", "Arguments" }, 
                CValueType.Void, 
                CValueType.String, CValueType.String
        )]
        [Method("scall",
                "Caller of executable files in silent mode with arguments and time limitation settings.",
                new string[] { "name", "args", "timeout" },
                new string[] { "Executable file", "Arguments", "How long to wait the execution, in seconds. 0 value - infinitely" },
                CValueType.Void,
                CValueType.String, CValueType.String, CValueType.Integer
        )]
        [Method("sout",
                "Receives data from standard streams for executed file.\nTo disable errors use the '2>nul' and similar.", 
                new string[] { "name" }, 
                new string[] { "Executable file" },
                CValueType.String, 
                CValueType.String
        )]
        [Method("sout",
                "Receives data from standard streams for executed file with arguments.\nTo disable errors use the '2>nul' and similar.", 
                new string[] { "name", "args" }, 
                new string[] { "Executable file", "Arguments" },
                CValueType.String, 
                CValueType.String, CValueType.String
        )]
        [Method("sout",
                "Receives data from standard streams for executed file with arguments and time limitation settings.\nTo disable errors use the '2>nul' and similar.",
                new string[] { "name", "args", "timeout" },
                new string[] { "Executable file", "Arguments", "How long to wait the execution, in seconds. 0 value - infinitely" },
                CValueType.String,
                CValueType.String, CValueType.String, CValueType.Integer
        )]
        protected string stCall(IPM pm, bool stdOut, bool silent)
        {
            ILevel level = pm.FirstLevel;

            string file;
            string args = String.Empty;
            int timeout = STCALL_TIMEOUT_DEFAULT;

            if(level.Is(ArgumentType.StringDouble)) {
                file = (string)level.Args[0].data;
            }
            else if(level.Is(ArgumentType.StringDouble, ArgumentType.StringDouble)) {
                file = (string)level.Args[0].data;
                args = (string)level.Args[1].data;
            }
            else if(level.Is(ArgumentType.StringDouble, ArgumentType.StringDouble, ArgumentType.Integer)) {
                file    = (string)level.Args[0].data;
                args    = (string)level.Args[1].data;
                timeout = (int)level.Args[2].data;
            }
            else {
                throw new ArgumentPMException(level, level.Data + "(string filename [, string args [, uinteger timeout]])");
            }

            string pfile = findFile(file);
            if(String.IsNullOrEmpty(pfile)) {
                throw new NotFoundException("File '{0}' was not found.", file);
            }
            return run(pfile, args, silent, stdOut, timeout);
        }

        /// <summary>
        /// Alias to sout() for cmd
        /// - #[File cmd("args")] -> #[File sout("cmd", "/C args")]
        /// </summary>
        [Method("cmd",
                "Alias to #[File sout(\"cmd\", \"/C args\")] \nReceives data from standard streams for cmd process with arguments.",
                new string[] { "args" },
                new string[] { "Arguments" },
                CValueType.String,
                CValueType.String
        )]
        [Method("cmd",
                "Alias to #[File sout(\"cmd\", \"/C args\", timeout)] \nReceives data from standard streams for cmd process with arguments and time limitation settings.",
                new string[] { "args", "timeout" },
                new string[] { "Arguments", "How long to wait the execution, in seconds. 0 value - infinitely" },
                CValueType.String,
                CValueType.String, CValueType.Integer
        )]
        protected string stCmd(IPM pm)
        {
            ILevel origin = pm.FirstLevel;

            if(origin.Is(ArgumentType.StringDouble))
            {
                pm.FirstLevel = new Level() {
                    Type        = LevelType.Method,
                    DataType    = origin.DataType,
                    Data        = "sout",
                    Args = new[] {
                        new Argument() { data = "cmd", type = ArgumentType.StringDouble },
                        new Argument() { data = "/C " + origin.Args[0].data, type = ArgumentType.StringDouble }
                    },
                };
            }
            else if(origin.Is(ArgumentType.StringDouble, ArgumentType.Integer))
            {
                pm.FirstLevel = new Level() {
                    Type        = LevelType.Method,
                    DataType    = origin.DataType,
                    Data        = "sout",
                    Args = new[] {
                        new Argument() { data = "cmd", type = ArgumentType.StringDouble },
                        new Argument() { data = "/C " + origin.Args[0].data, type = ArgumentType.StringDouble },
                        new Argument() { data = origin.Args[1].data, type = ArgumentType.Integer },
                    },
                };
            }
            else {
                throw new ArgumentPMException(origin, "string cmd(string args [, integer timeout])");
            }

            Log.Trace("stCmd redirect to stCall");
            return stCall(pm, true, true);
        }

        /// <param name="pm"></param>
        /// <returns></returns>
        protected string stWriteFamily(IPM pm)
        {
            if(pm.FinalIs(LevelType.Method, "write")) {
                return stWrite(pm, false, false);
            }
            if(pm.FinalIs(LevelType.Method, "append")) {
                return stWrite(pm, true, false);
            }
            if(pm.FinalIs(LevelType.Method, "writeLine")) {
                return stWrite(pm, false, true);
            }
            if(pm.FinalIs(LevelType.Method, "appendLine")) {
                return stWrite(pm, true, true);
            }

            throw new IncorrectNodeException(pm);
        }

        /// <param name="pm"></param>
        /// <param name="append">Flag to append the content to the end of the file.</param>
        /// <param name="newline">To write with newline.</param>
        /// <param name="enc">Encoding.</param>
        [Method("write",
                "To write data in a text file.\n * Creates if the file does not exist.\n * Overwrites content if it already exists.",
                new string[] { "name", "In" }, 
                new string[] { "File name or standard stream: STDOUT, STDERR", "mixed data" },
                CValueType.Void, 
                CValueType.String, CValueType.Input
        )]
        [Method("writeLine",
                "To write data with newline in a text file.\n * Creates if the file does not exist.\n * Overwrites content if it already exists.", 
                new string[] { "name", "In" }, 
                new string[] { "File name or standard stream: STDOUT, STDERR", "mixed data" },
                CValueType.Void, 
                CValueType.String, CValueType.Input
        )]
        [Method("write",
                "To write data in a text file with selected encoding and flags.\n * Creates if the file does not exist.",
                new string[] { "name", "append", "newline", "encoding", "In" },
                new string[] { "File name or standard stream: STDOUT, STDERR",
                                "Flag to append the content to the end of the file",
                                "To write with newline",
                                "Preferred encoding",
                                "mixed data" },
                CValueType.Void,
                CValueType.String, CValueType.Boolean, CValueType.Boolean, CValueType.String, CValueType.Input
        )]
        [Method("append",
                "To append data to the end of a text file or create new if file does not exist.",
                new string[] { "name", "In" }, 
                new string[] { "File name", "mixed data" },
                CValueType.Void, 
                CValueType.String, CValueType.Input
        )]
        [Method("appendLine",
                "To append data with newline to the end of a text file or create new if file does not exist.", 
                new string[] { "name", "In" }, 
                new string[] { "File name", "mixed data" },
                CValueType.Void, 
                CValueType.String, CValueType.Input
        )]
        protected string stWrite(IPM pm, bool append, bool newline, Encoding enc)
        {
            ILevel level = pm.FirstLevel;

            string file = null;
            string std  = null;

            // basic method signatures

            if(level.Is(ArgumentType.StringDouble) && pm.IsData("write", "writeLine", "append", "appendLine")) {
                file = (string)level.Args[0].data;
            }
            else if(level.Is(ArgumentType.StringDouble, ArgumentType.Boolean, ArgumentType.Boolean, ArgumentType.StringDouble) && pm.IsData("write")) {
                file    = (string)level.Args[0].data;
                append  = (bool)level.Args[1].data;
                newline = (bool)level.Args[2].data;
                enc     = encoding((string)level.Args[3].data);
            }
            else if(level.Is(ArgumentType.EnumOrConst) && pm.IsData("write", "writeLine")) {
                std = (string)level.Args[0].data;
            }
            else if(level.Is(ArgumentType.EnumOrConst, ArgumentType.Boolean, ArgumentType.Boolean, ArgumentType.StringDouble) && pm.IsData("write")) {
                std     = (string)level.Args[0].data;
                append  = (bool)level.Args[1].data;
                newline = (bool)level.Args[2].data;
                enc     = encoding((string)level.Args[3].data);
            }
            else {
                throw new ArgumentPMException(level, "write( (string name | const STD) [, boolean append, boolean newline, string encoding]); writeLine(string name | const STD); append/appendLine(string name)");
            }

            // content

            string fdata;
            if(pm.Levels[1].Type == LevelType.RightOperandColon) {
                fdata = pm.Levels[1].Data;
            }
            else {
                throw new IncorrectNodeException(pm);
            }

            // Text file

            if(file != null)
            {
                file = location(file);
                writeToFile(file, fdata, append, newline, enc);

                Log.Trace("The data:{0} is successfully written - `{1}` : {2}, {3}, '{4}'", 
                                               fdata.Length, file, append, newline, enc.EncodingName);

                return Value.Empty;
            }

            // Streams

            switch(std) {
                case "STDERR": {
                    writeToStdErr(fdata, newline);
                    return Value.Empty;
                }
                case "STDOUT": {
                    writeToStdOut(fdata, newline);
                    return Value.Empty;
                }
                default: {
                    throw new InvalidArgumentException("Incorrect stream type `{0}`", std);
                }
            }

            throw new IncorrectNodeException(pm);
        }

        protected string stWrite(IPM pm, bool append, bool newline)
        {
            return stWrite(pm, append, newline, defaultEncoding);
        }

        [Method("replace",
                "To replace data in files.", 
                new string[] { "file", "pattern", "replacement" },
                new string[] { "Input file", "String to compare", "New data" },
                CValueType.Void, 
                CValueType.String, CValueType.String, CValueType.String
        )]
        [Property("replace", "Provides additional replacement methods.")]
        [Method("Regex",
                "Alias for Regexp.", 
                "replace", "stReplace", 
                new string[] { "file", "pattern", "replacement" },
                new string[] { "Input file", "Regular expression pattern", "Replacement pattern" },
                CValueType.Void, 
                CValueType.String, CValueType.String, CValueType.String
        )]
        [Method("Regexp",
                "To replace data in files with Regular Expression Language.",
                "replace", "stReplace",
                new string[] { "file", "pattern", "replacement" },
                new string[] { "Input file", "Regular expression pattern", "Replacement pattern." },
                CValueType.Void, 
                CValueType.String, CValueType.String, CValueType.String
        )]
        [Method("Wildcards",
                "To replace data in files with Wildcards.",
                "replace", "stReplace",
                new string[] { "file", "pattern", "replacement" },
                new string[] { "Input file", "Pattern with wildcards", "New data" },
                CValueType.Void, 
                CValueType.String, CValueType.String, CValueType.String
        )]
        protected string stReplace(IPM pm)
        {
            SearchType type = SearchType.Null;

            // the old signatures

            if(pm.FinalEmptyIs(LevelType.Method, "replace")) {
                // replace(...)
                type = SearchType.Basic;
            }
            else if(pm.It(LevelType.Property, "replace"))
            {
                if(pm.FinalEmptyIs(LevelType.Method, "Regex") || pm.FinalEmptyIs(LevelType.Method, "Regexp")) {
                    // replace.Regex(...) & replace.Regexp(...)
                    type = SearchType.Regexp;
                }
                else if(pm.FinalEmptyIs(LevelType.Method, "Wildcards")) {
                    // replace.Wildcards(...)
                    type = SearchType.Wildcards;
                }
            }

            if(type == SearchType.Null) {
                throw new IncorrectNodeException(pm);
            }

            // arguments

            ILevel level = pm.FirstLevel;
            
            if(!level.Is(ArgumentType.StringDouble, ArgumentType.StringDouble, ArgumentType.StringDouble)) {
                throw new ArgumentPMException(level, "(string file, string pattern, string replacement)");
            }

            string file         = location((string)level.Args[0].data);
            string pattern      = (string)level.Args[1].data;
            string replacement  = (string)level.Args[2].data;

            Log.Trace("stReplace: found file '{0}',  pattern '{1}',  replacement '{2}'", file, pattern, replacement);

            Encoding enc    = detectEncodingFromFile(file);
            string content  = readToEnd(file, enc);

            Log.Debug("stReplace: type '{0}' :: received '{1}', Encoding '{2}'", type, content.Length, enc);
            content = stReplaceEngine(type, ref content, pattern, replacement);

            writeToFile(file, content, false, enc);
            return Value.Empty;
        }

        /// <param name="type">search type</param>
        /// <param name="content"></param>
        /// <param name="pattern"></param>
        /// <param name="replacement"></param>
        protected string stReplaceEngine(SearchType type, ref string content, string pattern, string replacement)
        {
            switch(type) {
                case SearchType.Regexp: {
                    return Regex.Replace(content, pattern, replacement);
                }
                case SearchType.Wildcards: {
                    string stub = Regex.Escape(pattern).Replace("\\*", ".*?").Replace("\\+", ".+?").Replace("\\?", ".");
                    return Regex.Replace(content, stub, replacement);
                }
                default: {
                    return content.Replace(pattern, replacement);
                }
            }
        }

        [Property("exists", "Determines whether the something exists.")]
        [Method("directory",
                "Determines whether the given path refers to an existing directory on disk.",
                "exists", "stExists",
                new string[] { "path" },
                new string[] { "Path to directory" },
                CValueType.Boolean,
                CValueType.String
        )]
        [Method("directory",
                "Determines whether the given path refers to an existing directory on disk. Searching with Environment path.",
                "exists", "stExists",
                new string[] { "path", "environment" },
                new string[] { "Path to directory", "Use Environment PATH (Associated for current process)." },
                CValueType.Boolean,
                CValueType.String, CValueType.Boolean
        )]
        [Method("file",
                "Determines whether the specified file exists.",
                "exists", "stExists",
                new string[] { "path" },
                new string[] { "Path to file" },
                CValueType.Boolean,
                CValueType.String
        )]
        [Method("file",
                "Determines whether the specified file exists. Searching with Environment path.",
                "exists", "stExists",
                new string[] { "path", "environment" },
                new string[] { "Path to file", "Use Environment PATH (Associated for current process)." },
                CValueType.Boolean,
                CValueType.String, CValueType.Boolean
        )]
        protected string stExists(IPM pm)
        {
            if(!pm.It(LevelType.Property, "exists")) {
                throw new IncorrectNodeException(pm);
            }
            ILevel level = pm.FirstLevel;

            if(!pm.FinalEmptyIs(LevelType.Method, "directory")
                && !pm.FinalEmptyIs(LevelType.Method, "file"))
            {
                throw new IncorrectNodeException(pm);
            }

            if(level.Is(ArgumentType.StringDouble)) {
                return stExists(pm.IsData("file"), (string)level.Args[0].data, false);
            }
            if(level.Is(ArgumentType.StringDouble, ArgumentType.Boolean)) {
                return stExists(pm.IsData("file"), (string)level.Args[0].data, (bool)level.Args[1].data);
            }

            throw new ArgumentPMException(level, level.Data + "(string path [, boolean environment])");
        }

        protected string stExists(bool tFile, string item, bool environment)
        {
            if(!environment) {
                return Value.from((tFile)? File.Exists(location(item)) : Directory.Exists(location(item)));
            }

            foreach(string dir in EnvPath)
            {
                bool found = (tFile)? File.Exists(location(item, dir)) : Directory.Exists(location(item, dir));
                if(found) {
                    return Value.from(true);
                }
            }

            return Value.from(false);
        }

        [Property("remote", "Remote servers.")]
        [Method("download",
                "To download file from remote server.",
                "remote", "stRemote",
                new string[] { "addr", "output" },
                new string[] { "Full address to remote file. e.g.: ftp://... http://...", "Output file name." },
                CValueType.Void,
                CValueType.String, CValueType.String
        )]
        [Method("download",
                "To download file from remote server.",
                "remote", "stRemote",
                new string[] { "addr", "output", "user", "pwd" },
                new string[] { "Full address to remote file. e.g.: ftp://... http://...", "Output file name.", "Username", "Password" },
                CValueType.Void,
                CValueType.String, CValueType.String, CValueType.String, CValueType.String
        )]
        protected string stRemote(IPM pm)
        {
            if(!pm.It(LevelType.Property, "remote")) {
                throw new IncorrectNodeException(pm);
            }
            ILevel level = pm.FirstLevel;

            if(pm.FinalEmptyIs(LevelType.Method, "download"))
            {
                if(level.Is(ArgumentType.StringDouble, ArgumentType.StringDouble)) {
                    return download((string)level.Args[0].data, (string)level.Args[1].data);
                }
                if(level.Is(ArgumentType.StringDouble, ArgumentType.StringDouble, ArgumentType.StringDouble, ArgumentType.StringDouble)) {
                    return download((string)level.Args[0].data, (string)level.Args[1].data, (string)level.Args[2].data, (string)level.Args[3].data);
                }

                throw new ArgumentPMException(level, "(string addr, string output [, string user, string pwd])");
            }

            throw new IncorrectNodeException(pm);
        }

        protected virtual string download(string addr, string output, string user = null, string pwd = null)
        {
            var wc = new WebClient();
            if(user != null) {
                wc.Credentials = new NetworkCredential(user, pwd ?? "");
            }

            wc.DownloadFile(addr, location(output));
            return Value.Empty;
        }

        [Property("copy", "The copy operations.")]
        protected string stCopy(IPM pm)
        {
            if(!pm.It(LevelType.Property, "copy")) {
                throw new IncorrectNodeException(pm);
            }
            ILevel level = pm.FirstLevel;

            if(pm.FinalEmptyIs(LevelType.Method, "file")) {
                return copyFile(level, pm);
            }
            if(pm.FinalEmptyIs(LevelType.Method, "directory")) {
                return copyDirectory(level, pm);
            }

            throw new IncorrectNodeException(pm);
        }

        /// <summary>
        ///  `copy.file((string src | object src), string dest, bool overwrite [, object except])`
        /// </summary>
        [Method("file",
                "To copy selected file to the destination. Creates the destination path if not exists.",
                "copy", "stCopy",
                new string[] { "src", "dest", "overwrite" },
                new string[] { "Source file. May contain mask as *.dll, *.*, ...",
                                "The destination path. May contain path to file or directory (end with \\ or /).",
                                "Overwrite file/s if already exists." },
                CValueType.Void,
                CValueType.String, CValueType.String, CValueType.Boolean
        )]
        [Method("file",
                "To copy selected file to the destination. Creates the destination path if not exists.",
                "copy", "stCopy",
                new string[] { "src", "dest", "overwrite", "except" },
                new string[] { "Source file. May contain mask as *.dll, *.*, ...",
                                "The destination path. May contain path to file or directory (end with \\ or /).",
                                "Overwrite file/s if already exists.",
                                "List of files to exclude from input source as {\"f1\", \"path\\*.dll\", ...}" },
                CValueType.Void,
                CValueType.String, CValueType.String, CValueType.Boolean, CValueType.Object
        )]
        [Method("file",
                "To copy selected files to the destination. Creates the destination path if not exists.",
                "copy", "stCopy",
                new string[] { "srclist", "dest", "overwrite" },
                new string[] { "List of source files as {\"f1\", \"path\\*.dll\", ..}",
                                "The destination path. Should contain path to directory.",
                                "Overwrite file/s if already exists." },
                CValueType.Void,
                CValueType.Object, CValueType.String, CValueType.Boolean
        )]
        [Method("file",
                "To copy selected files to the destination. Creates the destination path if not exists.",
                "copy", "stCopy",
                new string[] { "srclist", "dest", "overwrite", "except" },
                new string[] { "List of source files as {\"f1\", \"path\\*.dll\", ..}",
                                "The destination path. Should contain path to directory.",
                                "Overwrite file/s if already exists.",
                                "List of files to exclude from input source as {\"f1\", \"path\\*.dll\", ...}" },
                CValueType.Void,
                CValueType.Object, CValueType.String, CValueType.Boolean, CValueType.Object
        )]
        protected string copyFile(ILevel level, IPM pm)
        {
            if(level.Is(ArgumentType.StringDouble, ArgumentType.StringDouble, ArgumentType.Boolean)) {
                return copyFile(pm.pinTo(1), (string)level.Args[0].data, (string)level.Args[1].data, (bool)level.Args[2].data);
            }
            if(level.Is(ArgumentType.StringDouble, ArgumentType.StringDouble, ArgumentType.Boolean, ArgumentType.Object)) {
                return copyFile(pm.pinTo(1), (string)level.Args[0].data, (string)level.Args[1].data, (bool)level.Args[2].data, (Argument[])level.Args[3].data);
            }
            if(level.Is(ArgumentType.Object, ArgumentType.StringDouble, ArgumentType.Boolean)) {
                return copyFile(pm.pinTo(1), (Argument[])level.Args[0].data, (string)level.Args[1].data, (bool)level.Args[2].data);
            }
            if(level.Is(ArgumentType.Object, ArgumentType.StringDouble, ArgumentType.Boolean, ArgumentType.Object)) {
                return copyFile(pm.pinTo(1), (Argument[])level.Args[0].data, (string)level.Args[1].data, (bool)level.Args[2].data, (Argument[])level.Args[3].data);
            }

            throw new ArgumentPMException(level, "copy.file((string src | object srclist), string dest, bool overwrite [, object except])");
        }

        protected string copyFile(IPM pm, Argument[] files, string dest, bool overwrite, Argument[] except = null)
        {
            dest = dest.PathFormat();

            foreach(Argument src in files) {
                if(src.type != ArgumentType.StringDouble) {
                    throw new InvalidArgumentException("Incorrect data from input files. Define as {\"f1\", \"f2\", ...}");
                }
                copyFile(pm, src.data.ToString(), dest, overwrite, except);
            }

            return Value.Empty;
        }

        protected string copyFile(IPM pm, string src, string dest, bool overwrite, Argument[] except = null)
        {
            if(String.IsNullOrWhiteSpace(src) || String.IsNullOrWhiteSpace(dest)) {
                throw new InvalidArgumentException("The source file or the destination path argument is empty.");
            }

            if(except != null && except.Any(p => p.type != ArgumentType.StringDouble)) {
                throw new InvalidArgumentException("Incorrect data from the 'except' argument. Define as {\"f1\", \"f2\", ...}");
            }

            dest = location(dest.TrimEnd());
            string destDir  = Path.GetDirectoryName(dest);
            string destFile = Path.GetFileName(dest);

            src = location(src);
            string[] input = new[] { src }.ExtractFiles();
#if DEBUG
            Log.Trace("Found files to copy `{0}`", String.Join(", ", input));
#endif
            if(except != null) {
                string path = Path.GetDirectoryName(src);
                input = input.Except(except
                                        .Where(f => !String.IsNullOrWhiteSpace((string)f.data))
                                        .Select(f => location((string)f.data, path))
                                        .ToArray()
                                        .ExtractFiles()
                                    ).ToArray();
            }

            if(input.Length < 1) {
                throw new InvalidArgumentException("The input files was not found. Check your mask and the exception list if used.");
            }

            copyFile(destDir, destFile, overwrite, input);
            return Value.Empty;
        }

        protected virtual void copyFile(string destDir, string destFile, bool overwrite, params string[] files)
        {
            if(!Directory.Exists(destDir)) {
                Log.Trace("Trying to create directory `{0}`", destDir);
                Directory.CreateDirectory(destDir);
            }

            bool isDestFile = !String.IsNullOrWhiteSpace(destFile);
            if(isDestFile && files.Length > 1) {
                throw new InvalidArgumentException("The destination path `{0}` cannot contain file name `{1}` if the source has 2 or more files for used mask. End with `{1}\\` or `{1}/` if it directory.", destDir, destFile);
            }

            foreach(string file in files) {
                string dest = Path.Combine(destDir, isDestFile ? destFile : Path.GetFileName(file));
                Log.Trace("Copy file `{0}` to `{1}` overwrite({2})", file, dest, overwrite);
                File.Copy(file, dest, overwrite);
            }
        }

        /// <summary>
        ///  `copy.directory(string src, string dest, bool force [, bool overwrite])`
        /// </summary>
        [Method("directory",
                "To copy selected directory and subdirectories to the destination.",
                "copy", "stCopy",
                new string[] { "src", "dest", "force" },
                new string[] { "The source directory. Can be empty as new directory.", "The destination directory.", "Create the destination path if not exists." },
                CValueType.Void,
                CValueType.String, CValueType.String, CValueType.Boolean
        )]
        [Method("directory",
                "To copy selected directory and subdirectories to the destination.",
                "copy", "stCopy",
                new string[] { "src", "dest", "force", "overwrite" },
                new string[] { "The source directory. Can be empty as new directory.",
                                "The destination directory.",
                                "Create the destination path if not exists.",
                                "Overwrite files if already exists." },
                CValueType.Void,
                CValueType.String, CValueType.String, CValueType.Boolean, CValueType.Boolean
        )]
        protected string copyDirectory(ILevel level, IPM pm)
        {
            if(level.Is(ArgumentType.StringDouble, ArgumentType.StringDouble, ArgumentType.Boolean)) {
                return copyDirectory(pm.pinTo(1), (string)level.Args[0].data, (string)level.Args[1].data, (bool)level.Args[2].data);
            }
            if(level.Is(ArgumentType.StringDouble, ArgumentType.StringDouble, ArgumentType.Boolean, ArgumentType.Boolean)) {
                return copyDirectory(pm.pinTo(1), (string)level.Args[0].data, (string)level.Args[1].data, (bool)level.Args[2].data, (bool)level.Args[3].data);
            }

            throw new ArgumentPMException(level, "copy.directory(string src, string dest, bool force [, bool overwrite])");
        }

        protected string copyDirectory(IPM pm, string src, string dest, bool force, bool overwrite = false)
        {
            if(String.IsNullOrWhiteSpace(dest)) {
                throw new InvalidArgumentException("The destination directory argument is empty.");
            }

            dest = Path.GetDirectoryName(location(dest.PathFormat()));

            if(String.IsNullOrWhiteSpace(src)) {
                if(force) {
                    mkdir(dest);
                    return Value.Empty;
                }
                throw new InvalidArgumentException("Use `force` flag if you want to create directory `{0}`", dest);
            }

            src = location(src.PathFormat());

            var files = Directory.EnumerateFiles(src, "*.*", SearchOption.AllDirectories)
                                    .Select(f => new[] { f, Path.Combine(dest, f.Substring(src.Length)) });

            copyDirectory(files, dest, force, overwrite);
            return Value.Empty;
        }

        protected virtual void copyDirectory(IEnumerable<string[]> files, string dest, bool force, bool overwrite)
        {
            if(!Directory.Exists(dest)) {
                if(!force) {
                    throw new NotFoundException("Part of path `{0}` of the destination directory is not exists. Check path or use `force` flag", dest);
                }
                Log.Trace("Trying to create directory `{0}`", dest);
                Directory.CreateDirectory(dest);
            }

            foreach(var file in files.ToArray())
            {
                string from = file[0];
                string to   = file[1];

                string subdir = Path.GetDirectoryName(to);
                if(!Directory.Exists(subdir)) {
                    Directory.CreateDirectory(subdir);
                }

                Log.Trace("Copy directory: file `{0}` to `{1}` overwrite({2})", from, to, overwrite);
                File.Copy(from, to, overwrite);
            }
        }

        [Property("delete", "The delete operations.")]
        protected string stDelete(IPM pm)
        {
            if(!pm.It(LevelType.Property, "delete")) {
                throw new IncorrectNodeException(pm);
            }
            ILevel level = pm.FirstLevel;

            if(pm.FinalEmptyIs(LevelType.Method, "files")) {
                return deleteFiles(level, pm);
            }
            if(pm.FinalEmptyIs(LevelType.Method, "directory")) {
                return deleteDirectory(level, pm);
            }

            throw new IncorrectNodeException(pm);
        }

        /// <summary>
        ///  `delete.files(object files [, object except])`
        /// </summary>
        [Method("files",
                "To delete selected files.",
                "delete", "stDelete",
                new string[] { "files" },
                new string[] { "List of files to deletion as {\"f1\", \"path\\*.dll\", ..}" },
                CValueType.Void,
                CValueType.Object
        )]
        [Method("files",
                "To delete selected files.",
                "delete", "stDelete",
                new string[] { "files", "except" },
                new string[] { "List of files to deletion as {\"f1\", \"path\\*.dll\", ..}", "List of files to exclude from input list." },
                CValueType.Void,
                CValueType.Object, CValueType.Object
        )]
        protected string deleteFiles(ILevel level, IPM pm)
        {
            if(level.Is(ArgumentType.Object)) {
                return deleteFiles(pm.pinTo(1), (Argument[])level.Args[0].data);
            }
            if(level.Is(ArgumentType.Object, ArgumentType.Object)) {
                return deleteFiles(pm.pinTo(1), (Argument[])level.Args[0].data, (Argument[])level.Args[1].data);
            }

            throw new ArgumentPMException(level, "delete.files(object files [, object except])");
        }

        protected string deleteFiles(IPM pm, Argument[] files, Argument[] except = null)
        {
            if(files.Any(p => p.type != ArgumentType.StringDouble)) {
                throw new InvalidArgumentException("Incorrect data from input files. Define as {\"f1\", \"f2\", ...}");
            }

            if(except != null && except.Any(p => p.type != ArgumentType.StringDouble)) {
                throw new InvalidArgumentException("Incorrect data from the 'except' argument. Define as {\"f1\", \"f2\", ...}");
            }

            Func<string, int, string> exs = delegate(string file, int idx) {
                if(!String.IsNullOrWhiteSpace(file)) {
                    return location(file);
                }
                throw new InvalidArgumentException("File name is empty. Fail in '{0}' position.", idx);
            };

            string[] input = files.Select((f, i) => exs((string)f.data, i)).ToArray().ExtractFiles();
#if DEBUG
            Log.Trace("deleteFiles: Found files `{0}`", String.Join(", ", input));
#endif
            if(except != null) {
                input = input.Except(except
                                        .Where(f => !String.IsNullOrWhiteSpace((string)f.data))
                                        .Select(f => location((string)f.data))
                                        .ToArray()
                                        .ExtractFiles()
                                    ).ToArray();
            }

            if(input.Length < 1) {
                throw new InvalidArgumentException("The input files was not found. Check your mask and the exception list if used.");
            }

            deleteFiles(input);
            return Value.Empty;
        }

        protected virtual void deleteFiles(string[] files)
        {
            foreach(string file in files) {
                Log.Trace("Delete file `{0}`", file);
                File.Delete(file);
            }
        }

        /// <summary>
        ///  `delete.directory(string dir, bool force)`
        /// </summary>
        [Method("directory",
                "To delete selected directory.",
                "delete", "stDelete",
                new string[] { "dir", "force" },
                new string[] { "Path to directory for deletion.", "To remove non-empty directories." },
                CValueType.Void,
                CValueType.String, CValueType.Boolean
        )]
        protected string deleteDirectory(ILevel level, IPM pm)
        {
            if(level.Is(ArgumentType.StringDouble, ArgumentType.Boolean)) {
                return deleteDirectory(pm.pinTo(1), (string)level.Args[0].data, (bool)level.Args[1].data);
            }

            throw new ArgumentPMException(level, "delete.directory(string dir, bool force)");
        }

        protected string deleteDirectory(IPM pm, string src, bool force)
        {
            if(String.IsNullOrWhiteSpace(src)) {
                throw new InvalidArgumentException("The source directory is empty.");
            }
            deleteDirectory(location(src), force);
            return Value.Empty;
        }

        protected virtual void deleteDirectory(string src, bool force)
        {
            Log.Trace("Delete directory `{0}` /force: {1}", src, force);
            if(Directory.Exists(src)) { // to avoid errors.. like `File.Delete`
                Directory.Delete(src, force);
            }
        }

        protected virtual void mkdir(string path)
        {
            if(!Directory.Exists(path)) {
                Log.Trace("Create empty directory `{0}`", path);
                Directory.CreateDirectory(path);
            }
        }

        /// <param name="file">The file to be read</param>
        /// <param name="enc">The character encoding</param>
        /// <param name="detectEncoding">Indicates whether to look for byte order marks at the beginning of the file</param>
        protected virtual string readToEnd(string file, Encoding enc, bool detectEncoding = false)
        {
            using(StreamReader stream = new StreamReader(file, enc, detectEncoding)) {
                return stream.ReadToEnd();
            }
        }

        protected string readToEnd(string file)
        {
            return readToEnd(file, defaultEncoding, true);
        }

        /// <param name="file"></param>
        /// <param name="data"></param>
        /// <param name="append">Flag to append the content to the end of the file</param>
        /// <param name="newline">To write with newline if true</param>
        /// <param name="enc">The character encoding</param>
        protected virtual void writeToFile(string file, string data, bool append, bool newline, Encoding enc)
        {
#if DEBUG
            Log.Trace("File `{0}` write with Encoding '{1}'", file, enc.EncodingName);
#endif
            using(TextWriter stream = new StreamWriter(file, append, enc)) {
                if(newline) {
                    stream.WriteLine(data);
                }
                else {
                    stream.Write(data);
                }
            }
        }

        protected void writeToFile(string file, string data, bool append, bool newline)
        {
            writeToFile(file, data, append, newline, defaultEncoding);
        }

        protected void writeToFile(string file, string data, bool append, Encoding enc)
        {
            writeToFile(file, data, append, false, enc);
        }

        /// <summary>
        /// Write into standard output stream.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="newline">To write with newline if true</param>
        protected void writeToStdOut(string data, bool newline)
        {
            if(newline) {
                Console.Out.WriteLine(data);
            }
            else {
                Console.Out.Write(data);
            }
        }

        /// <summary>
        /// Write into standard error stream.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="newline">To write with newline if true</param>
        protected void writeToStdErr(string data, bool newline)
        {
            if(newline) {
                Console.Error.WriteLine(data);
            }
            else {
                Console.Error.Write(data);
            }
        }

        /// <summary>
        /// Auto detecting encoding from the file.
        /// </summary>
        protected virtual Encoding detectEncodingFromFile(string file)
        {
            using(FileStream fs = File.OpenRead(file))
            {
                Ude.CharsetDetector cdet = new Ude.CharsetDetector();
                cdet.Feed(fs);
                cdet.DataEnd();

                if(cdet.Charset == null) {
                    //throw new ComponentException("Ude: Detection failed for '{0}'", file);
                    Log.Warn("Problem with detection of encoding for '{0}'", file);
                    return defaultEncoding; // good luck
                }

                Log.Debug("Ude: charset '{0}' confidence: '{1}'", cdet.Charset, cdet.Confidence);
                Encoding enc = Encoding.GetEncoding(cdet.Charset);

                if(enc == Encoding.UTF8) {
                    fs.Seek(0, SeekOrigin.Begin);
                    return (fs.ReadByte() == 0xEF && 
                            fs.ReadByte() == 0xBB && 
                            fs.ReadByte() == 0xBF) ? new UTF8Encoding(true) : new UTF8Encoding(false);
                }

                return enc;
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
        protected virtual string run(string file, string args, bool silent, bool stdOut, int timeout = 0)
        {
            string ret = (new Actions.HProcess(Settings.WPath)).run(file, args, silent, timeout);
            return (stdOut)? ret : String.Empty;
        }

        /// <summary>
        /// Gets full path to file with environment PATH.
        /// </summary>
        /// <returns>null value if file is not found</returns>
        protected virtual string findFile(string file)
        {
            string lfile = location(file);
            if(File.Exists(lfile)) {
                return lfile;
            }
            Log.Trace("finding file with environment PATH :: `{0}`({1})", file, lfile);

            string[] exts = System.Environment.GetEnvironmentVariable("PATHEXT").Split(';');
            foreach(string dir in EnvPath)
            {
                lfile = location(file, dir);
                if(File.Exists(lfile) || exts.Any(ext => File.Exists(lfile + ext))) {
                    Log.Trace("found in: '{0}' :: '{1}'", dir, lfile);
                    return lfile;
                }
            }
            return null;
        }

        /// <summary>
        /// Location of file for specific path.
        /// </summary>
        protected string location(string file, string path)
        {
            return Path.Combine(path, file);
        }

        /// <summary>
        /// Location of file for current context.
        /// </summary>
        /// <returns>Full path to file</returns>
        protected string location(string file)
        {
            return Path.Combine(Settings.WPath, file);
        }

        private Encoding encoding(string name)
        {
            if(String.IsNullOrWhiteSpace(name)) {
                throw new InvalidArgumentException("Name of encoding is null or empty.");
            }

            if(name.Equals("utf-8", StringComparison.OrdinalIgnoreCase)) {
                return new UTF8Encoding(false); // to disable the BOM for Encoding.UTF8 by default
            }

            if(name.Equals("utf-8-bom", StringComparison.OrdinalIgnoreCase))
            {
                /* The custom name to enable BOM. 
                        Use of a BOM is neither required nor recommended for UTF-8, but may 
                        be encountered in contexts where UTF-8 data is converted from other encoding forms that 
                        use a BOM or where the BOM is used as a UTF-8 signature.  
                        http://www.unicode.org/versions/Unicode5.0.0/ch02.pdf
                    
                   So we use this as additional settings.
                */
                return new UTF8Encoding(true);
            }

            return Encoding.GetEncoding(name);
        }
    }
}
