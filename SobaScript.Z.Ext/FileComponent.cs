/*
 * The MIT License (MIT)
 *
 * Copyright (c) 2014-2019  Denis Kuzmin < x-3F@outlook.com > GitHub/3F
 * Copyright (c) SobaScript.Z.Ext contributors: https://github.com/3F/Varhead/graphs/contributors
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using net.r_eg.Components;
using net.r_eg.SobaScript.Components;
using net.r_eg.SobaScript.Exceptions;
using net.r_eg.SobaScript.Mapper;
using net.r_eg.SobaScript.SNode;
using net.r_eg.SobaScript.Z.Ext.IO;

namespace net.r_eg.SobaScript.Z.Ext
{
    [Component("File", new[] { "IO" }, "I/O File operations.")]
    public class FileComponent: FileIOAbstract, IComponent
    {
        /*         
        This is a very old component. Part of method signatures (user code) requires modernization with an modern IMP analyzer.
        And moreover splitting into various nodes per separated classes. Please fix me.
             */

        /// <summary>
        /// Default limit in seconds for execution processes.
        /// </summary>
        public const int STCALL_TIMEOUT_DEFAULT = 10;

        protected Lazy<IEnumerable<string>> _envPath;

        /// <summary>
        /// Expression when to start processing.
        /// </summary>
        public override string Activator =>  @"(?:File|IO)\s";

        /// <summary>
        /// Use regex engine for the Condition property
        /// </summary>
        public override bool ARegex => true;

        public override IExer Exer { get; protected set; }

        public override IEnumerable<string> EnvPath => _envPath.Value;

        protected override IEncDetector EncDetector { get; set; }

        /// <summary>
        /// Prepare, parse, and evaluate mixed data through SobaScript supported syntax.
        /// </summary>
        /// <param name="data">Mixed input data.</param>
        /// <returns>Evaluated end value.</returns>
        public override string Eval(string data)
        {
            var point       = EntryPoint(data, RegexOptions.Singleline);
            string subtype  = point.Key;
            string request  = point.Value;

            LSender.Send(this, $"`{ToString()}`: subtype - `{subtype}`, request - `{request}`", MsgLevel.Trace);

            IPM pm = new PM(request, emsbuild);

            if(pm.IsData("get")) {
                return StGet(pm);
            }
            if(pm.IsData("call", "out", "scall", "sout", "cmd")) {
                return StCallFamily(pm);
            }
            if(pm.IsData("write", "append", "writeLine", "appendLine")) {
                return StWriteFamily(pm);
            }
            if(pm.IsData("replace")) {
                return StReplace(pm);
            }
            if(pm.IsData("exists")) {
                return StExists(pm);
            }
            if(pm.IsData("remote")) {
                return StRemote(pm);
            }
            if(pm.IsData("copy")) {
                return StCopy(pm);
            }
            if(pm.IsData("delete")) {
                return StDelete(pm);
            }

            throw new IncorrectNodeException(pm);
        }

        public FileComponent(ISobaScript soba, string basePath)
            : this(soba, null, basePath)
        {

        }

        public FileComponent(ISobaScript soba, IEncDetector detector, string basePath)
            : this(soba, detector, new Exer(basePath))
        {

        }

        public FileComponent(ISobaScript soba, IExer exer)
            : this(soba, null, exer)
        {

        }

        public FileComponent(ISobaScript soba, IEncDetector detector, IExer exer)
            : base(soba)
        {
            EncDetector = detector;
            Exer        = exer ?? throw new ArgumentNullException(nameof(exer));
            _envPath    = new Lazy<IEnumerable<string>>(() => GetEnvPath(EnvironmentVariableTarget.Process));
        }

        [Method("get",
                "Get all data from text file.", 
                new[] { "name" }, 
                new[] { "File name" }, 
                CValType.String, 
                CValType.String)]
        protected string StGet(IPM pm)
        {
            if(!pm.FinalEmptyIs(LevelType.Method, "get")) {
                throw new IncorrectNodeException(pm);
            }
            ILevel level = pm.FirstLevel;

            if(!level.Is(ArgumentType.StringDouble)) {
                throw new PMLevelException(level, "get(string name)");
            }
            string file = GetLocation((string)level.Args[0].data);

            return ReadToEnd(file, DetectEncodingFromFile(file));
            // NOTE: x NotFoundException -> FileNotFoundException
        }

        /// <param name="pm"></param>
        /// <returns></returns>
        protected string StCallFamily(IPM pm)
        {
            if(pm.FinalEmptyIs(LevelType.Method, "call")) {
                return StCall(pm, false, false);
            }

            if(pm.FinalEmptyIs(LevelType.Method, "out")) {
                return StCall(pm, true, true); // redirect to sout
            }

            if(pm.FinalEmptyIs(LevelType.Method, "scall")) {
                return StCall(pm, false, true);
            }

            if(pm.FinalEmptyIs(LevelType.Method, "sout")) {
                return StCall(pm, true, true);
            }

            if(pm.FinalEmptyIs(LevelType.Method, "cmd")) {
                return StCmd(pm);
            }

            throw new IncorrectNodeException(pm);
        }

        /// <param name="pm"></param>
        /// <param name="stdOut">Use StandardOutput or not</param>
        /// <param name="silent">Silent mode</param>
        /// <returns>Received data from StandardOutput</returns>
        [Method("call", 
                "Caller of executable files.", 
                new[] { "name" }, 
                new[] { "Executable file" }, 
                CValType.Void, 
                CValType.String)]
        [Method("call", 
                "Caller of executable files with arguments.", 
                new[] { "name", "args" }, 
                new[] { "Executable file", "Arguments" }, 
                CValType.Void, 
                CValType.String, CValType.String)]
        [Method("call",
                "Caller of executable files with arguments and time limitation settings.",
                new[] { "name", "args", "timeout" },
                new[] { "Executable file", "Arguments", "How long to wait the execution, in seconds. 0 value - infinitely" },
                CValType.Void,
                CValType.String, CValType.String, CValType.Integer)]
        [Method("scall", 
                "Caller of executable files in silent mode.", 
                new[] { "name" }, 
                new[] { "Executable file" }, 
                CValType.Void, 
                CValType.String)]
        [Method("scall", 
                "Caller of executable files in silent mode with arguments.", 
                new[] { "name", "args" }, 
                new[] { "Executable file", "Arguments" }, 
                CValType.Void, 
                CValType.String, CValType.String)]
        [Method("scall",
                "Caller of executable files in silent mode with arguments and time limitation settings.",
                new[] { "name", "args", "timeout" },
                new[] { "Executable file", "Arguments", "How long to wait the execution, in seconds. 0 value - infinitely" },
                CValType.Void,
                CValType.String, CValType.String, CValType.Integer)]
        [Method("sout",
                "Receives data from standard streams for executed file.\nTo disable errors use the '2>nul' and similar.", 
                new[] { "name" }, 
                new[] { "Executable file" },
                CValType.String, 
                CValType.String)]
        [Method("sout",
                "Receives data from standard streams for executed file with arguments.\nTo disable errors use the '2>nul' and similar.", 
                new[] { "name", "args" }, 
                new[] { "Executable file", "Arguments" },
                CValType.String, 
                CValType.String, CValType.String)]
        [Method("sout",
                "Receives data from standard streams for executed file with arguments and time limitation settings.\nTo disable errors use the '2>nul' and similar.",
                new[] { "name", "args", "timeout" },
                new[] { "Executable file", "Arguments", "How long to wait the execution, in seconds. 0 value - infinitely" },
                CValType.String,
                CValType.String, CValType.String, CValType.Integer)]
        protected string StCall(IPM pm, bool stdOut, bool silent)
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
                throw new PMLevelException(level, level.Data + "(string filename [, string args [, uinteger timeout]])");
            }

            string pfile = FindFile(file);
            if(String.IsNullOrEmpty(pfile)) {
                throw new NotFoundException(file);
            }
            return Run(pfile, args, silent, stdOut, timeout);
        }

        /// <summary>
        /// Alias to sout() for cmd
        /// - #[File cmd("args")] -> #[File sout("cmd", "/C args")]
        /// </summary>
        [Method("cmd",
                "Alias to #[File sout(\"cmd\", \"/C args\")] \nReceives data from standard streams for cmd process with arguments.",
                new[] { "args" },
                new[] { "Arguments" },
                CValType.String,
                CValType.String)]
        [Method("cmd",
                "Alias to #[File sout(\"cmd\", \"/C args\", timeout)] \nReceives data from standard streams for cmd process with arguments and time limitation settings.",
                new[] { "args", "timeout" },
                new[] { "Arguments", "How long to wait the execution, in seconds. 0 value - infinitely" },
                CValType.String,
                CValType.String, CValType.Integer)]
        protected string StCmd(IPM pm)
        {
            ILevel origin = pm.FirstLevel;

            if(origin.Is(ArgumentType.StringDouble))
            {
                pm.FirstLevel = new Level
                (
                    new Argument() { data = "cmd", type = ArgumentType.StringDouble },
                    new Argument() { data = "/C " + origin.Args[0].data, type = ArgumentType.StringDouble }
                ) {
                    Type        = LevelType.Method,
                    DataType    = origin.DataType,
                    Data        = "sout",
                };
            }
            else if(origin.Is(ArgumentType.StringDouble, ArgumentType.Integer))
            {
                pm.FirstLevel = new Level
                (
                    new Argument() { data = "cmd", type = ArgumentType.StringDouble },
                    new Argument() { data = "/C " + origin.Args[0].data, type = ArgumentType.StringDouble },
                    new Argument() { data = origin.Args[1].data, type = ArgumentType.Integer }
                ) {
                    Type        = LevelType.Method,
                    DataType    = origin.DataType,
                    Data        = "sout",
                };
            }
            else {
                throw new PMLevelException(origin, "string cmd(string args [, integer timeout])");
            }

            LSender.Send(this, $"StCmd redirect to StCall", MsgLevel.Trace);
            return StCall(pm, true, true);
        }

        protected string StWriteFamily(IPM pm)
        {
            if(pm.FinalIs(LevelType.Method, "write")) {
                return StWrite(pm, false, false);
            }
            if(pm.FinalIs(LevelType.Method, "append")) {
                return StWrite(pm, true, false);
            }
            if(pm.FinalIs(LevelType.Method, "writeLine")) {
                return StWrite(pm, false, true);
            }
            if(pm.FinalIs(LevelType.Method, "appendLine")) {
                return StWrite(pm, true, true);
            }

            throw new IncorrectNodeException(pm);
        }

        /// <param name="pm"></param>
        /// <param name="append">Flag to append the content to the end of the file.</param>
        /// <param name="newline">To write with newline.</param>
        /// <param name="enc">Encoding.</param>
        [Method("write",
                "To write data in a text file.\n * Creates if the file does not exist.\n * Overwrites content if it already exists.",
                new[] { "name", "In" }, 
                new[] { "File name or standard stream: STDOUT, STDERR", "mixed data" },
                CValType.Void, 
                CValType.String, CValType.Input)]
        [Method("writeLine",
                "To write data with newline in a text file.\n * Creates if the file does not exist.\n * Overwrites content if it already exists.", 
                new[] { "name", "In" }, 
                new[] { "File name or standard stream: STDOUT, STDERR", "mixed data" },
                CValType.Void, 
                CValType.String, CValType.Input)]
        [Method("write",
                "To write data in a text file with selected encoding and flags.\n * Creates if the file does not exist.",
                new[] { "name", "append", "newline", "encoding", "In" },
                new[] { "File name or standard stream: STDOUT, STDERR",
                                "Flag to append the content to the end of the file",
                                "To write with newline",
                                "Preferred encoding",
                                "mixed data" },
                CValType.Void,
                CValType.String, CValType.Boolean, CValType.Boolean, CValType.String, CValType.Input)]
        [Method("append",
                "To append data to the end of a text file or create new if file does not exist.",
                new[] { "name", "In" }, 
                new[] { "File name", "mixed data" },
                CValType.Void, 
                CValType.String, CValType.Input)]
        [Method("appendLine",
                "To append data with newline to the end of a text file or create new if file does not exist.", 
                new[] { "name", "In" }, 
                new[] { "File name", "mixed data" },
                CValType.Void, 
                CValType.String, CValType.Input)]
        protected string StWrite(IPM pm, bool append, bool newline, Encoding enc)
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
                enc     = GetEncoding((string)level.Args[3].data);
            }
            else if(level.Is(ArgumentType.EnumOrConst) && pm.IsData("write", "writeLine")) {
                std = (string)level.Args[0].data;
            }
            else if(level.Is(ArgumentType.EnumOrConst, ArgumentType.Boolean, ArgumentType.Boolean, ArgumentType.StringDouble) && pm.IsData("write")) {
                std     = (string)level.Args[0].data;
                append  = (bool)level.Args[1].data;
                newline = (bool)level.Args[2].data;
                enc     = GetEncoding((string)level.Args[3].data);
            }
            else {
                throw new PMLevelException(level, "write( (string name | const STD) [, boolean append, boolean newline, string encoding]); writeLine(string name | const STD); append/appendLine(string name)");
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
                file = GetLocation(file);
                WriteToFile(file, fdata, append, newline, enc);

                LSender.Send(this, $"The data:{fdata.Length} is successfully written - `{file}` : {append}, {newline}, '{enc.EncodingName}'", MsgLevel.Trace);

                return Value.Empty;
            }

            // Streams

            switch(std) {
                case "STDERR": {
                    WriteToStdErr(fdata, newline);
                    return Value.Empty;
                }
                case "STDOUT": {
                    WriteToStdOut(fdata, newline);
                    return Value.Empty;
                }
                default: {
                    throw new IncorrectSyntaxException($"Incorrect stream type `{std}`");
                }
            }

            throw new IncorrectNodeException(pm);
        }

        protected string StWrite(IPM pm, bool append, bool newline)
        {
            return StWrite(pm, append, newline, defaultEncoding);
        }

        [Method("replace",
                "To replace data in files.", 
                new[] { "file", "pattern", "replacement" },
                new[] { "Input file", "String to compare", "New data" },
                CValType.Void, 
                CValType.String, CValType.String, CValType.String)]
        [Property("replace", "Provides additional replacement methods.")]
        [Method("Regex",
                "Alias for Regexp.", 
                "replace", nameof(StReplace), 
                new[] { "file", "pattern", "replacement" },
                new[] { "Input file", "Regular expression pattern", "Replacement pattern" },
                CValType.Void, 
                CValType.String, CValType.String, CValType.String)]
        [Method("Regexp",
                "To replace data in files with Regular Expression Language.",
                "replace", nameof(StReplace),
                new[] { "file", "pattern", "replacement" },
                new[] { "Input file", "Regular expression pattern", "Replacement pattern." },
                CValType.Void, 
                CValType.String, CValType.String, CValType.String)]
        [Method("Wildcards",
                "To replace data in files with Wildcards.",
                "replace", nameof(StReplace),
                new[] { "file", "pattern", "replacement" },
                new[] { "Input file", "Pattern with wildcards", "New data" },
                CValType.Void, 
                CValType.String, CValType.String, CValType.String)]
        protected string StReplace(IPM pm)
        {
            SearchType type = SearchType.Null;

            // old signatures

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
                throw new PMLevelException(level, "(string file, string pattern, string replacement)");
            }

            string file         = GetLocation((string)level.Args[0].data);
            string pattern      = (string)level.Args[1].data;
            string replacement  = (string)level.Args[2].data;

            LSender.Send(this, $"StReplace: found file '{file}',  pattern '{pattern}',  replacement '{replacement}'", MsgLevel.Trace);

            Encoding enc    = DetectEncodingFromFile(file);
            string content  = ReadToEnd(file, enc);

            LSender.Send(this, $"StReplace: type '{type}' :: received '{content.Length}', Encoding '{enc}'");
            content = StReplaceEngine(type, ref content, pattern, replacement);

            WriteToFile(file, content, false, enc);
            return Value.Empty;
        }

        [Property("exists", "Determines whether the something exists.")]
        [Method("directory",
                "Determines whether the given path refers to an existing directory on disk.",
                "exists", nameof(StExists),
                new[] { "path" },
                new[] { "Path to directory" },
                CValType.Boolean,
                CValType.String)]
        [Method("directory",
                "Determines whether the given path refers to an existing directory on disk. Searching with Environment path.",
                "exists", nameof(StExists),
                new[] { "path", "environment" },
                new[] { "Path to directory", "Use Environment PATH (Associated for current process)." },
                CValType.Boolean,
                CValType.String, CValType.Boolean)]
        [Method("file",
                "Determines whether the specified file exists.",
                "exists", nameof(StExists),
                new[] { "path" },
                new[] { "Path to file" },
                CValType.Boolean,
                CValType.String)]
        [Method("file",
                "Determines whether the specified file exists. Searching with Environment path.",
                "exists", nameof(StExists),
                new[] { "path", "environment" },
                new[] { "Path to file", "Use Environment PATH (Associated for current process)." },
                CValType.Boolean,
                CValType.String, CValType.Boolean)]
        protected string StExists(IPM pm)
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
                return StExists(pm.IsData("file"), (string)level.Args[0].data, false);
            }
            if(level.Is(ArgumentType.StringDouble, ArgumentType.Boolean)) {
                return StExists(pm.IsData("file"), (string)level.Args[0].data, (bool)level.Args[1].data);
            }

            throw new PMLevelException(level, level.Data + "(string path [, boolean environment])");
        }

        protected string StExists(bool tFile, string item, bool environment)
        {
            if(!environment) {
                return Value.From((tFile)? File.Exists(GetLocation(item)) : Directory.Exists(GetLocation(item)));
            }

            foreach(string dir in EnvPath)
            {
                bool found = (tFile)? File.Exists(GetLocation(item, dir)) : Directory.Exists(GetLocation(item, dir));
                if(found) {
                    return Value.From(true);
                }
            }

            return Value.From(false);
        }

        [Property("remote", "Remote servers.")]
        [Method("download",
                "To Download file from remote server.",
                "remote", nameof(StRemote),
                new[] { "addr", "output" },
                new[] { "Full address to remote file. eg.: ftp://... http://...", "Output file name." },
                CValType.Void,
                CValType.String, CValType.String)]
        [Method("download",
                "To Download file from remote server.",
                "remote", nameof(StRemote),
                new[] { "addr", "output", "user", "pwd" },
                new[] { "Full address to remote file. eg.: ftp://... http://...", "Output file name.", "Username", "Password" },
                CValType.Void,
                CValType.String, CValType.String, CValType.String, CValType.String)]
        protected string StRemote(IPM pm)
        {
            if(!pm.It(LevelType.Property, "remote")) {
                throw new IncorrectNodeException(pm);
            }
            ILevel level = pm.FirstLevel;

            if(pm.FinalEmptyIs(LevelType.Method, "download"))
            {
                if(level.Is(ArgumentType.StringDouble, ArgumentType.StringDouble)) {
                    return Download((string)level.Args[0].data, (string)level.Args[1].data);
                }
                if(level.Is(ArgumentType.StringDouble, ArgumentType.StringDouble, ArgumentType.StringDouble, ArgumentType.StringDouble)) {
                    return Download((string)level.Args[0].data, (string)level.Args[1].data, (string)level.Args[2].data, (string)level.Args[3].data);
                }

                throw new PMLevelException(level, "(string addr, string output [, string user, string pwd])");
            }

            throw new IncorrectNodeException(pm);
        }

        [Property("copy", "The copy operations.")]
        protected string StCopy(IPM pm)
        {
            if(!pm.It(LevelType.Property, "copy")) {
                throw new IncorrectNodeException(pm);
            }
            ILevel level = pm.FirstLevel;

            if(pm.FinalEmptyIs(LevelType.Method, "file")) {
                return CopyFile(level, pm);
            }
            if(pm.FinalEmptyIs(LevelType.Method, "directory")) {
                return CopyDirectory(level, pm);
            }

            throw new IncorrectNodeException(pm);
        }

        /// <summary>
        ///  `copy.file((string src | object src), string dest, bool overwrite [, object except])`
        /// </summary>
        [Method("file",
                "To copy selected file to the destination. Creates the destination path if not exists.",
                "copy", nameof(StCopy),
                new[] { "src", "dest", "overwrite" },
                new[] { "Source file. May contain mask as *.dll, *.*, ...",
                                "The destination path. May contain path to file or directory (end with \\ or /).",
                                "Overwrite file/s if already exists." },
                CValType.Void,
                CValType.String, CValType.String, CValType.Boolean)]
        [Method("file",
                "To copy selected file to the destination. Creates the destination path if not exists.",
                "copy", nameof(StCopy),
                new[] { "src", "dest", "overwrite", "except" },
                new[] { "Source file. May contain mask as *.dll, *.*, ...",
                                "The destination path. May contain path to file or directory (end with \\ or /).",
                                "Overwrite file/s if already exists.",
                                "List of files to exclude from input source as {\"f1\", \"path\\*.dll\", ...}" },
                CValType.Void,
                CValType.String, CValType.String, CValType.Boolean, CValType.Object)]
        [Method("file",
                "To copy selected files to the destination. Creates the destination path if not exists.",
                "copy", nameof(StCopy),
                new[] { "srclist", "dest", "overwrite" },
                new[] { "List of source files as {\"f1\", \"path\\*.dll\", ..}",
                                "The destination path. Should contain path to directory.",
                                "Overwrite file/s if already exists." },
                CValType.Void,
                CValType.Object, CValType.String, CValType.Boolean)]
        [Method("file",
                "To copy selected files to the destination. Creates the destination path if not exists.",
                "copy", nameof(StCopy),
                new[] { "srclist", "dest", "overwrite", "except" },
                new[] { "List of source files as {\"f1\", \"path\\*.dll\", ..}",
                                "The destination path. Should contain path to directory.",
                                "Overwrite file/s if already exists.",
                                "List of files to exclude from input source as {\"f1\", \"path\\*.dll\", ...}" },
                CValType.Void,
                CValType.Object, CValType.String, CValType.Boolean, CValType.Object)]
        protected string CopyFile(ILevel level, IPM pm)
        {
            if(level.Is(ArgumentType.StringDouble, ArgumentType.StringDouble, ArgumentType.Boolean)) {
                return CopyFile(pm.PinTo(1), (string)level.Args[0].data, (string)level.Args[1].data, (bool)level.Args[2].data);
            }
            if(level.Is(ArgumentType.StringDouble, ArgumentType.StringDouble, ArgumentType.Boolean, ArgumentType.Object)) {
                return CopyFile(pm.PinTo(1), (string)level.Args[0].data, (string)level.Args[1].data, (bool)level.Args[2].data, (RArgs)level.Args[3].data);
            }
            if(level.Is(ArgumentType.Object, ArgumentType.StringDouble, ArgumentType.Boolean)) {
                return CopyFile(pm.PinTo(1), (RArgs)level.Args[0].data, (string)level.Args[1].data, (bool)level.Args[2].data);
            }
            if(level.Is(ArgumentType.Object, ArgumentType.StringDouble, ArgumentType.Boolean, ArgumentType.Object)) {
                return CopyFile(pm.PinTo(1), (RArgs)level.Args[0].data, (string)level.Args[1].data, (bool)level.Args[2].data, (RArgs)level.Args[3].data);
            }

            throw new PMLevelException(level, "copy.file((string src | object srclist), string dest, bool overwrite [, object except])");
        }

        /// <summary>
        ///  `copy.directory(string src, string dest, bool force [, bool overwrite])`
        /// </summary>
        [Method("directory",
                "To copy selected directory and subdirectories to the destination.",
                "copy", nameof(StCopy),
                new[] { "src", "dest", "force" },
                new[] { "The source directory. Can be empty as new directory.", "The destination directory.", "Create the destination path if not exists." },
                CValType.Void,
                CValType.String, CValType.String, CValType.Boolean)]
        [Method("directory",
                "To copy selected directory and subdirectories to the destination.",
                "copy", nameof(StCopy),
                new[] { "src", "dest", "force", "overwrite" },
                new[] { "The source directory. Can be empty as new directory.",
                                "The destination directory.",
                                "Create the destination path if not exists.",
                                "Overwrite files if already exists." },
                CValType.Void,
                CValType.String, CValType.String, CValType.Boolean, CValType.Boolean)]
        protected string CopyDirectory(ILevel level, IPM pm)
        {
            if(level.Is(ArgumentType.StringDouble, ArgumentType.StringDouble, ArgumentType.Boolean)) {
                return CopyDirectory(pm.PinTo(1), (string)level.Args[0].data, (string)level.Args[1].data, (bool)level.Args[2].data);
            }
            if(level.Is(ArgumentType.StringDouble, ArgumentType.StringDouble, ArgumentType.Boolean, ArgumentType.Boolean)) {
                return CopyDirectory(pm.PinTo(1), (string)level.Args[0].data, (string)level.Args[1].data, (bool)level.Args[2].data, (bool)level.Args[3].data);
            }

            throw new PMLevelException(level, "copy.directory(string src, string dest, bool force [, bool overwrite])");
        }

        [Property("delete", "The delete operations.")]
        protected string StDelete(IPM pm)
        {
            if(!pm.It(LevelType.Property, "delete")) {
                throw new IncorrectNodeException(pm);
            }
            ILevel level = pm.FirstLevel;

            if(pm.FinalEmptyIs(LevelType.Method, "files")) {
                return DeleteFiles(level, pm);
            }
            if(pm.FinalEmptyIs(LevelType.Method, "directory")) {
                return DeleteDirectory(level, pm);
            }

            throw new IncorrectNodeException(pm);
        }

        /// <summary>
        ///  `delete.files(object files [, object except])`
        /// </summary>
        [Method("files",
                "To delete selected files.",
                "delete", nameof(StDelete),
                new[] { "files" },
                new[] { "List of files to deletion as {\"f1\", \"path\\*.dll\", ..}" },
                CValType.Void,
                CValType.Object)]
        [Method("files",
                "To delete selected files.",
                "delete", nameof(StDelete),
                new[] { "files", "except" },
                new[] { "List of files to deletion as {\"f1\", \"path\\*.dll\", ..}", "List of files to exclude from input list." },
                CValType.Void,
                CValType.Object, CValType.Object)]
        protected string DeleteFiles(ILevel level, IPM pm)
        {
            if(level.Is(ArgumentType.Object)) {
                return DeleteFiles(pm.PinTo(1), (RArgs)level.Args[0].data);
            }
            if(level.Is(ArgumentType.Object, ArgumentType.Object)) {
                return DeleteFiles(pm.PinTo(1), (RArgs)level.Args[0].data, (RArgs)level.Args[1].data);
            }

            throw new PMLevelException(level, "delete.files(object files [, object except])");
        }

        /// <summary>
        ///  `delete.directory(string dir, bool force)`
        /// </summary>
        [Method("directory",
                "To delete selected directory.",
                "delete", nameof(StDelete),
                new[] { "dir", "force" },
                new[] { "Path to directory for deletion.", "To remove non-empty directories." },
                CValType.Void,
                CValType.String, CValType.Boolean)]
        protected string DeleteDirectory(ILevel level, IPM pm)
        {
            if(level.Is(ArgumentType.StringDouble, ArgumentType.Boolean)) {
                return DeleteDirectory(pm.PinTo(1), (string)level.Args[0].data, (bool)level.Args[1].data);
            }

            throw new PMLevelException(level, "delete.directory(string dir, bool force)");
        }
    }
}
