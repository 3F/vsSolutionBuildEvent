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
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using net.r_eg.Components;
using net.r_eg.SobaScript.Components;
using net.r_eg.SobaScript.Exceptions;
using net.r_eg.SobaScript.Mapper;
using net.r_eg.SobaScript.SNode;
using net.r_eg.SobaScript.Z.Ext.Extensions;
using net.r_eg.SobaScript.Z.Ext.SevenZip;

namespace net.r_eg.SobaScript.Z.Ext
{
    [Component("7z", "7-Zip.\nFile archiver with high compression ratio.\nwww.7-zip.org")]
    public class SevenZipComponent: ComponentAbstract, IComponent
    {
        protected IArchiver archiver;
        private string _basePath;

        /// <summary>
        /// Expression when to start processing.
        /// </summary>
        public override string Activator => "7z ";

        public string BasePath
        {
            get => _basePath;
            set => _basePath = value.FormatDirPath();
        }

        /// <summary>
        /// Prepare, parse, and evaluate mixed data through SobaScript supported syntax.
        /// </summary>
        /// <param name="data">Mixed input data.</param>
        /// <returns>Evaluated end value.</returns>
        public override string Eval(string data)
        {
            var point       = EntryPoint(data);
            string subtype  = point.Key;
            string request  = point.Value;

            LSender.Send(this, $"`{ToString()}`: subtype - `{subtype}`, request - `{request}`", MsgLevel.Trace);

            IPM pm = new PM(request, emsbuild);
            switch(subtype)
            {
                case "pack": {
                    return StPack(pm);
                }
                case "unpack": {
                    return StUnpack(pm);
                }
                case "check": {
                    return StCheck(pm);
                }
            }

            throw new SubtypeNotFoundException(subtype);
        }

        public SevenZipComponent(ISobaScript soba, IArchiver archiver, string basePath)
            : base(soba)
        {
            this.archiver   = archiver ?? throw new ArgumentNullException(nameof(archiver));
            BasePath        = basePath;
        }

        /// <summary>
        /// Work with packing node.
        /// Sample: #[7z pack]
        /// </summary>
        /// <param name="pm"></param>
        /// <returns></returns>
        [Property("pack", "Packing with 7-zip engine.")]
        protected string StPack(IPM pm)
        {
            if(!pm.It(LevelType.Property, "pack")) {
                throw new IncorrectNodeException(pm);
            }
            ILevel level = pm.FirstLevel; // level of the pack property

            if(pm.FinalEmptyIs(LevelType.Method, "files")) {
                return PackFiles(level, pm);
            }

            if(pm.FinalEmptyIs(LevelType.Method, "directory")) {
                return PackDirectory(level, pm);
            }

            throw new IncorrectNodeException(pm);
        }

        /// <summary>
        /// Prepares signatures:
        ///     pack.files({"f1", "f2", "*.dll"}, "output" [, format, method, level])
        ///     pack.files({"f1", "f2", "*.dll"}, "output", {" f3.dll"} [, format, method, level])
        /// </summary>
        /// <param name="level"></param>
        /// <param name="pm"></param>
        /// <returns></returns>
        protected string PackFiles(ILevel level, IPM pm)
        {
            if(level.Is(ArgumentType.Object, ArgumentType.StringDouble)) {
                return StPackFiles((RArgs)level.Args[0].data, (string)level.Args[1].data, pm.PinTo(1));
            }

            if(level.Is(ArgumentType.Object, ArgumentType.StringDouble, ArgumentType.EnumOrConst, ArgumentType.EnumOrConst, ArgumentType.Integer))
            {
                return StPackFiles(
                            (RArgs)level.Args[0].data, 
                            (string)level.Args[1].data,
                            (FormatType)Enum.Parse(typeof(FormatType), (string)level.Args[2].data),
                            (MethodType)Enum.Parse(typeof(MethodType), (string)level.Args[3].data),
                            (RateType)(int)level.Args[4].data,
                            pm.PinTo(1));
            }

            if(level.Is(ArgumentType.Object, ArgumentType.StringDouble, ArgumentType.Object)) {
                return StPackFiles((RArgs)level.Args[0].data, (string)level.Args[1].data, (RArgs)level.Args[2].data, pm.PinTo(1));
            }

            if(level.Is(ArgumentType.Object, ArgumentType.StringDouble, ArgumentType.Object, ArgumentType.EnumOrConst, ArgumentType.EnumOrConst, ArgumentType.Integer))
            {
                return StPackFiles(
                            (RArgs)level.Args[0].data,
                            (string)level.Args[1].data,
                            (RArgs)level.Args[2].data,
                            (FormatType)Enum.Parse(typeof(FormatType), (string)level.Args[3].data),
                            (MethodType)Enum.Parse(typeof(MethodType), (string)level.Args[4].data),
                            (RateType)(int)level.Args[5].data,
                            pm.PinTo(1));
            }

            throw new PMLevelException(level, "pack.files(object files, string output [, object except][, enum format, enum method, integer level])");
        }

        /// <summary>
        /// Packing files for signature:
        ///     `pack.files(object files, string output)`
        /// </summary>
        /// <param name="files">Input files.</param>
        /// <param name="name">Output archive.</param>
        /// <param name="pm"></param>
        /// <returns></returns>
        protected string StPackFiles(RArgs files, string name, IPM pm)
            => StPackFiles(files, name, null, pm);

        /// <summary>
        /// Packing files for signature:
        ///     `pack.files(object files, string output, object except)`
        /// </summary>
        /// <param name="files">Input files.</param>
        /// <param name="name">Output archive.</param>
        /// <param name="except">List of files to exclude from input list.</param>
        /// <param name="pm"></param>
        /// <returns></returns>
        protected string StPackFiles(RArgs files, string name, RArgs except, IPM pm)
            => StPackFiles(files, name, except, FormatType.Zip, MethodType.Deflate, RateType.Normal, pm);

        /// <summary>
        /// Packing files for signature:
        ///     `pack.files(object files, string output, enum format, enum method, integer level)`
        /// </summary>
        /// <param name="files">Input files.</param>
        /// <param name="name">Output archive.</param>
        /// <param name="type"></param>
        /// <param name="method"></param>
        /// <param name="rate"></param>
        /// <param name="pm"></param>
        /// <returns></returns>
        protected string StPackFiles(RArgs files, string name, FormatType type, MethodType method, RateType rate, IPM pm)
            => StPackFiles(files, name, null, type, method, rate, pm);

        /// <summary>
        /// Packing files for signature:
        ///     `pack.files(object files, string output, object except, enum format, enum method, integer level)`
        /// </summary>
        /// <param name="files">Input files.</param>
        /// <param name="name">Output archive.</param>
        /// <param name="except">List of files to exclude from input list.</param>
        /// <param name="fmt"></param>
        /// <param name="method"></param>
        /// <param name="rate"></param>
        /// <param name="pm"></param>
        /// <returns></returns>
        [Method("files",
                "To compress selected files with default settings.",
                "pack",
                nameof(StPack),
                new[] { "files", "output" },
                new[] { "List of files as {\"f1\", \"path\\*.dll\", ..}", "Archive name" },
                CValType.Void,
                CValType.Object, CValType.String)]
        [Method("files",
                "To compress selected files with default settings.",
                "pack",
                nameof(StPack),
                new[] { "files", "output", "except" },
                new[] {
                    "List of files as {\"f1\", \"path\\*.dll\", ..}",
                    "Archive name",
                    "List of files to exclude from input list"
                },
                CValType.Void,
                CValType.Object, CValType.String, CValType.Object)]
        [Method("files",
                "To compress selected files with custom settings.",
                "pack",
                nameof(StPack),
                new[] { "files", "output", "format", "method", "level" },
                new[] {
                    "List of files as {\"f1\", \"path\\*.dll\", ..}",
                    "Archive name",
                    "Type of archive: SevenZip, Zip, GZip, BZip2, Tar, XZ",
                    "Compression method: Copy, Deflate, Deflate64, BZip2, Lzma, Lzma2, Ppmd",
                    "Compression level: 0 (None) to 5 (Maximum)"
                },
                CValType.Void,
                CValType.Object, CValType.String, CValType.Enum, CValType.Enum, CValType.UInteger)]
        [Method("files",
                "To compress selected files with custom settings.",
                "pack",
                nameof(StPack),
                new[] { "files", "output", "except", "format", "method", "level" },
                new[] {
                    "List of files as {\"f1\", \"path\\*.dll\", ..}",
                    "Archive name",
                    "List of files to exclude from input list",
                    "Type of archive: SevenZip, Zip, GZip, BZip2, Tar, XZ",
                    "Compression method: Copy, Deflate, Deflate64, BZip2, Lzma, Lzma2, Ppmd",
                    "Compression level: 0 (None) to 5 (Maximum)"
                },
                CValType.Void,
                CValType.Object, CValType.String, CValType.Object, CValType.Enum, CValType.Enum, CValType.UInteger)]
        protected string StPackFiles(RArgs files, string name, RArgs except, FormatType fmt, MethodType method, RateType rate, IPM pm)
        {
            LSender.Send(this, $"StPackFiles: `{name}` : type({fmt}), method({method}), level({rate})", MsgLevel.Trace);

            if(string.IsNullOrWhiteSpace(name)) {
                throw new ArgumentException("The output name of archive is empty.");
            }

            if(files.Length < 1) {
                throw new PMArgException(files, "List of files cannot be empty.");
            }

            if(files.Any(p => p.type != ArgumentType.StringDouble)) {
                throw new PMArgException(files, "Define as {\"f1\", \"f2\", ...}");
            }

            if(except != null && except.Any(p => p.type != ArgumentType.StringDouble)) {
                throw new PMArgException(except, "'except' argument. Define as {\"f1\", \"f2\", ...}");
            }

            // additional checking of input files. 
            // SevenZipSharp creates directories if input file does not exist o_O ! wtfeature
            string[] input = files.Select((f, i) => PathToFile((string)f.data, i)).ToArray().ExtractFiles(BasePath);
#if DEBUG
            LSender.Send(this, $"StPackFiles: Found files `{string.Join(", ", input)}`", MsgLevel.Trace);
#endif
            if(except != null)
            {
                input = input.Except
                (
                    except
                    .Where(f => !string.IsNullOrWhiteSpace((string)f.data))
                    .Select(f => Location((string)f.data))
                    .ToArray()
                    .ExtractFiles(BasePath)
                )
                .ToArray();
            }

            if(input.Length < 1) {
                throw new PMArgException(files, "The input files was not found. Check your mask and the exception list if used.");
            }

            archiver.Compress(input, Location(name), method, rate, fmt);
            return Value.Empty;
        }

        /// <summary>
        /// Prepares signatures:
        ///     pack.directory(string dir, string output [, enum format, enum method, integer level])
        /// </summary>
        /// <param name="level"></param>
        /// <param name="pm"></param>
        /// <returns></returns>
        protected string PackDirectory(ILevel level, IPM pm)
        {
            if(level.Is(ArgumentType.StringDouble, ArgumentType.StringDouble)) {
                return StPackDirectory((string)level.Args[0].data, (string)level.Args[1].data, pm.PinTo(1));
            }

            if(level.Is(ArgumentType.StringDouble, ArgumentType.StringDouble, ArgumentType.EnumOrConst, ArgumentType.EnumOrConst, ArgumentType.Integer))
            {
                return StPackDirectory
                (
                    (string)level.Args[0].data, 
                    (string)level.Args[1].data,
                    (FormatType)Enum.Parse(typeof(FormatType), (string)level.Args[2].data),
                    (MethodType)Enum.Parse(typeof(MethodType), (string)level.Args[3].data),
                    (RateType)(int)level.Args[4].data,
                    pm.PinTo(1)
                );
            }

            throw new PMLevelException(level, "pack.directory(string dir, string output [, enum format, enum method, integer level])");
        }

        /// <summary>
        /// Packing directory for signature:
        ///     pack.directory(string dir, string output)
        /// </summary>
        /// <param name="dir">Input directory.</param>
        /// <param name="name">Output archive.</param>
        /// <param name="pm"></param>
        /// <returns></returns>
        protected string StPackDirectory(string dir, string name, IPM pm)
            => StPackDirectory(dir, name, FormatType.Zip, MethodType.Deflate, RateType.Normal, pm);

        /// <summary>
        /// Packing directory for signature:
        ///     pack.directory(string dir, string output, enum format, enum method, integer level)
        /// </summary>
        /// <param name="dir">Input directory.</param>
        /// <param name="name">Output archive.</param>
        /// <param name="fmt"></param>
        /// <param name="method"></param>
        /// <param name="rate"></param>
        /// <param name="pm"></param>
        /// <returns></returns>
        [Method("directory",
                "To compress selected directory with default settings.",
                "pack",
                nameof(StPack),
                new[] { "path", "output" },
                new[] { "Path to directory for packing", "Archive name" },
                CValType.Void,
                CValType.String, CValType.String)]
        [Method("directory",
                "To compress selected directory with custom settings.",
                "pack",
                nameof(StPack),
                new[] { "path", "output", "format", "method", "level" },
                new[] {
                    "Path to directory for packing",
                    "Archive name",
                    "Type of archive: SevenZip, Zip, GZip, BZip2, Tar, XZ",
                    "Compression method: Copy, Deflate, Deflate64, BZip2, Lzma, Lzma2, Ppmd",
                    "Compression level: 0 (None) to 5 (Maximum)"
                },
                CValType.Void,
                CValType.String, CValType.String, CValType.Enum, CValType.Enum, CValType.UInteger)]
        protected string StPackDirectory(string dir, string name, FormatType fmt, MethodType method, RateType rate, IPM pm)
        {
            LSender.Send(this, $"StPackDirectory: `{dir}` -> `{name}` : type({fmt}), method({method}), level({rate})", MsgLevel.Trace);

            if(string.IsNullOrWhiteSpace(dir)) {
                throw new ArgumentException("The path to directory is empty.");
            }

            if(string.IsNullOrWhiteSpace(name)) {
                throw new ArgumentException("The output name of archive is empty.");
            }

            string fullpath = Location(dir);

            // additional checking of input directory. 
            // SevenZipSharp creates empty file of archive even if the input directory does not exist o_O ! wtfeature
            if(!Directory.Exists(fullpath)) {
                throw new NotFoundException(dir, $"Directory inside `{fullpath}`", fullpath);
            }

            archiver.Compress(fullpath, Location(name), method, rate, fmt);
            return Value.Empty;
        }

        /// <summary>
        /// Work with unpacking node.
        /// Sample: #[7z unpack]
        /// </summary>
        /// <param name="pm"></param>
        /// <returns></returns>
        [Method("unpack",
                "Extract archive data into same directory.",
                new[] { "file" },
                new[] { "Archive for unpacking" },
                CValType.Void,
                CValType.String)]
        [Method("unpack",
                "Extract archive data into selected directory.",
                new[] { "file", "output" },
                new[] { "Archive for unpacking", "Output path to unpacking archive data" },
                CValType.Void,
                CValType.String, CValType.String)]
        [Method("unpack",
                "Extract data from protected archive into selected directory.",
                new[] { "file", "output", "pwd" },
                new[] { "Archive for unpacking", "Output path to unpacking archive data", "Password of archive" },
                CValType.Void,
                CValType.String, CValType.String, CValType.String)]
        [Method("unpack",
                "Extract archive data into selected directory and delete it after extraction if needed.",
                new[] { "file", "output", "delete" },
                new[] { "Archive for unpacking", "Output path to unpacking archive data", "To delete archive after extraction if true" },
                CValType.Void,
                CValType.String, CValType.String, CValType.Boolean)]
        [Method("unpack",
                "Extract data from protected archive into selected directory and delete it after extraction if needed.",
                new[] { "file", "output", "delete", "pwd" },
                new[] { "Archive for unpacking", "Output path to unpacking archive data", "To delete archive after extraction if true", "Password of archive" },
                CValType.Void,
                CValType.String, CValType.String, CValType.Boolean, CValType.String)]
        [Method("unpack",
                "Extract archive data into same directory and delete it after extraction if needed.",
                new[] { "file", "delete" },
                new[] { "Archive for unpacking", "To delete archive after extraction if true" },
                CValType.Void,
                CValType.String, CValType.Boolean)]
        [Method("unpack",
                "Extract data from protected archive into same directory and delete it after extraction if needed.",
                new[] { "file", "delete", "pwd" },
                new[] { "Archive for unpacking", "To delete archive after extraction if true", "Password of archive" },
                CValType.Void,
                CValType.String, CValType.Boolean, CValType.String)]
        protected string StUnpack(IPM pm)
        {
            if(pm.FinalEmptyIs(LevelType.Method, "unpack")) {
                return Unpack(pm.FirstLevel, pm);
            }

            //TODO: ~ unpack.files(...)
            throw new IncorrectNodeException(pm);
        }

        /// <summary>
        /// Prepares signatures:
        ///     unpack(file [, output][, delete][, pwd])
        /// </summary>
        /// <param name="level"></param>
        /// <param name="pm"></param>
        /// <returns></returns>
        protected string Unpack(ILevel level, IPM pm)
        {
            // unpack(string file)
            if(level.Is(ArgumentType.StringDouble)) {
                return StUnpackMethod(pm, (string)level.Args[0].data);
            }

            // unpack(string file, string output)
            if(level.Is(ArgumentType.StringDouble, ArgumentType.StringDouble)) {
                return StUnpackMethod(pm, (string)level.Args[0].data, (string)level.Args[1].data);
            }

            // unpack(string file, string output, string pwd)
            if(level.Is(ArgumentType.StringDouble, ArgumentType.StringDouble, ArgumentType.StringDouble)) {
                return StUnpackMethod(pm, (string)level.Args[0].data, (string)level.Args[1].data, false, (string)level.Args[2].data);
            }

            // unpack(string file, string output, boolean delete)
            if(level.Is(ArgumentType.StringDouble, ArgumentType.StringDouble, ArgumentType.Boolean)) {
                return StUnpackMethod(pm, (string)level.Args[0].data, (string)level.Args[1].data, (bool)level.Args[2].data);
            }

            // unpack(string file, string output, boolean delete, string pwd)
            if(level.Is(ArgumentType.StringDouble, ArgumentType.StringDouble, ArgumentType.Boolean, ArgumentType.StringDouble)) {
                return StUnpackMethod(pm, (string)level.Args[0].data, (string)level.Args[1].data, (bool)level.Args[2].data, (string)level.Args[3].data);
            }

            // unpack(string file, boolean delete)
            if(level.Is(ArgumentType.StringDouble, ArgumentType.Boolean)) {
                return StUnpackMethod(pm, (string)level.Args[0].data, null, (bool)level.Args[1].data);
            }

            // unpack(string file, boolean delete, string pwd)
            if(level.Is(ArgumentType.StringDouble, ArgumentType.Boolean, ArgumentType.StringDouble)) {
                return StUnpackMethod(pm, (string)level.Args[0].data, null, (bool)level.Args[1].data, (string)level.Args[2].data);
            }

            throw new PMLevelException(level, "`unpack(string file [, string output][, boolean delete][, string pwd])`");
        }

        /// <summary>
        /// Unpacking archive for signature:
        ///     unpack(string file [, string output][, boolean delete][, string pwd])
        /// </summary>
        /// <param name="pm"></param>
        /// <param name="file">Archive for unpacking.</param>
        /// <param name="output">Output path to unpacking archive data.</param>
        /// <param name="delete">To delete archive after extracting data if true.</param>
        /// <param name="pwd">password of archive if used.</param>
        /// <returns></returns>
        protected string StUnpackMethod(IPM pm, string file, string output = null, bool delete = false, string pwd = null)
        {
            LSender.Send(this, $"StUnpackMethod: `{file}` -> `{output}` : delete({delete}), pwd({((pwd == null) ? "none" : "***")})", MsgLevel.Trace);

            file = PathToFile(file);

            if(output == null) {
                output = GetDirectoryFromFile(file); // same path
            }
            else if(string.IsNullOrWhiteSpace(output)) {
                throw new ArgumentException("The `output` argument can't be empty.");
            }
            else {
                output = Location(output);
            }

            ExtractArchive(file, output, delete, pwd);
            return Value.Empty;
        }

        /// <param name="file">Archive for unpacking.</param>
        /// <param name="output">Output path to unpacking archive data.</param>
        /// <param name="delete">To delete archive after extracting data if true.</param>
        /// <param name="pwd">password of archive if used.</param>
        protected virtual void ExtractArchive(string file, string output, bool delete, string pwd)
        {
            if(!archiver.Extract(file, output, pwd)) {
                throw new ExternalException("Failed to extract from archive. Check your password if it's required.");
            }

            if(delete) {
                File.Delete(file);
            }
        }

        /// <summary>
        /// Work with the checking node.
        /// Sample: #[7z check]
        /// </summary>
        /// <param name="pm"></param>
        /// <returns></returns>
        [Method("check",
                "Verification data of archive.\nReturns true if no any errors for archive.",
                new[] { "file" },
                new[] { "Archive for testing" },
                CValType.Boolean,
                CValType.String)]
        [Method("check",
                "Verification data of archive.\nReturns true if no any errors for archive.",
                new[] { "file", "pwd" },
                new[] { "Archive for testing", "Password of archive" },
                CValType.Boolean,
                CValType.String, CValType.String)]
        protected string StCheck(IPM pm)
        {
            if(pm.FinalEmptyIs(LevelType.Method, "check")) {
                return Check(pm.FirstLevel, pm);
            }

            throw new IncorrectNodeException(pm);
        }

        /// <summary>
        /// Prepares signatures:
        ///     check(file [, pwd])
        /// </summary>
        /// <param name="level"></param>
        /// <param name="pm"></param>
        /// <returns></returns>
        protected string Check(ILevel level, IPM pm)
        {
            // check(string file)
            if(level.Is(ArgumentType.StringDouble)) {
                return StCheckMethod(pm, (string)level.Args[0].data);
            }

            // check(string file, string pwd)
            if(level.Is(ArgumentType.StringDouble, ArgumentType.StringDouble)) {
                return StCheckMethod(pm, (string)level.Args[0].data, (string)level.Args[1].data);
            }

            throw new PMLevelException(level, "`check(string file [, string pwd])`");
        }

        /// <summary>
        /// Checking archive for signature:
        ///     check(string file [, string pwd])
        /// </summary>
        /// <param name="pm"></param>
        /// <param name="file">Archive for unpacking.</param>
        /// <param name="pwd">password of archive if used.</param>
        /// <returns></returns>
        protected string StCheckMethod(IPM pm, string file, string pwd = null)
        {
            LSender.Send(this, $"StUnpackMethod: `{file}` pwd({((pwd == null) ? "none" : "***")})", MsgLevel.Trace);
            return CheckArchive(PathToFile(file), pwd);
        }

        /// <param name="file">Archive for unpacking.</param>
        /// <param name="pwd">password of archive if used.</param>
        /// <returns></returns>
        protected virtual string CheckArchive(string file, string pwd)
            => Value.From(archiver.Check(file, pwd));

        /// <summary>
        /// Gets path to item with Location for current context.
        /// </summary>
        /// <param name="item"></param>
        /// <returns>Combined path to file or directory.</returns>
        protected virtual string Location(string item)
            => Path.Combine(BasePath, item);

        /// <param name="file"></param>
        /// <returns>Path to directory</returns>
        protected virtual string GetDirectoryFromFile(string file)
            => Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file));

        /// <param name="file"></param>
        /// <param name="idx">Position from list if used (is not -1)</param>
        /// <returns></returns>
        private string PathToFile(string file, int idx = -1)
        {
            if(string.IsNullOrWhiteSpace(file))
            {
                throw new ArgumentException(
                    $"File name is empty. {GetFailedInMsg(idx)}"
                );
            }

            string fullpath = Location(file);

            // check existence of file if it's non-mask
            if(fullpath.IndexOf('*') == -1 && !File.Exists(fullpath))
            {
                throw new NotFoundException
                (
                    file,
                    $"File inside `{fullpath}`; {GetFailedInMsg(idx)}",
                    fullpath, idx
                );
            }

            return fullpath;
        }

        private string GetFailedInMsg(int idx)
        {
            if(idx == -1) {
                return string.Empty;
            }
            return $"Failed in '{idx}' position.";
        }
    }
}