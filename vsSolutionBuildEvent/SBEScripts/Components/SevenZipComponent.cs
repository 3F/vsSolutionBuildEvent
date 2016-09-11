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
using System.IO;
using System.Linq;
using System.Reflection;
using net.r_eg.vsSBE.Exceptions;
using net.r_eg.vsSBE.Extensions;
using net.r_eg.vsSBE.SBEScripts.Dom;
using net.r_eg.vsSBE.SBEScripts.Exceptions;
using net.r_eg.vsSBE.SBEScripts.SNode;
using SevenZip;

namespace net.r_eg.vsSBE.SBEScripts.Components
{
    /// <summary>
    /// Support of archives via 7-Zip engine
    /// </summary>
    [Component("7z", "7-Zip.\nFile archiver with high compression ratio.\nwww.7-zip.org")]
    public class SevenZipComponent: Component, IComponent
    {
        /// <summary>
        /// Name of library with full 7-Zip engine for work with all available formats.
        /// </summary>
        public const string LIB_FULL = "7z.dll";

        /// <summary>
        /// Name of library with compact version of 7-Zip engine for 7z archives.
        /// </summary>
        public const string LIB_ZA = "7za.dll";

        /// <summary>
        /// Name of library with compact version of 7-Zip engine for extracting from 7z archives.
        /// </summary>
        public const string LIB_ZXA = "7zxa.dll";

        /// <summary>
        /// Is ready to work with library ?
        /// </summary>
        private bool isReady = false;

        /// <summary>
        /// Ability to work with data for current component
        /// </summary>
        public override string Condition
        {
            get { return "7z "; }
        }

        /// <param name="loader">Initialize with loader</param>
        public SevenZipComponent(IBootloader loader)
            : base(loader)
        {
            initLib();
        }

        public SevenZipComponent()
        {
            initLib();
        }

        /// <summary>
        /// Handler for current data
        /// </summary>
        /// <param name="data">mixed data</param>
        /// <returns>prepared and evaluated data</returns>
        public override string parse(string data)
        {
            if(!isReady) {
                throw new ComponentException("`{0}` is not ready for work with 7-zip engine. Details in log.", ToString());
            }

            var point       = entryPoint(data);
            string subtype  = point.Key;
            string request  = point.Value;

            Log.Trace("`{0}`: subtype - `{1}`, request - `{2}`", ToString(), subtype, request);

            IPM pm = new PM(request, msbuild);
            switch(subtype) {
                case "pack": {
                    return stPack(pm);
                }
                case "unpack": {
                    return stUnpack(pm);
                }
                case "check": {
                    return stCheck(pm);
                }
            }

            throw new SubtypeNotFoundException("Subtype `{0}` is not found", subtype);
        }

        protected void initLib()
        {
            string cPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).PathFormat();
            try {
                SevenZipBase.SetLibraryPath(Path.Combine(cPath, LIB_FULL));
                isReady = true;
            }
            catch(Exception ex) {
                Log.Error("Found problem with library {0} ({1}): `{2}`", LIB_FULL, cPath, ex.Message);
            }
        }

        /// <summary>
        /// Work with packing node.
        /// Sample: #[7z pack]
        /// </summary>
        /// <param name="pm"></param>
        /// <returns></returns>
        [Property("pack", "Packing with 7-zip engine.")]
        protected string stPack(IPM pm)
        {
            if(!pm.It(LevelType.Property, "pack")) {
                throw new IncorrectNodeException(pm);
            }
            ILevel level = pm.FirstLevel; // level of the pack property

            if(pm.FinalEmptyIs(LevelType.Method, "files")) {
                return packFiles(level, pm);
            }

            if(pm.FinalEmptyIs(LevelType.Method, "directory")) {
                return packDirectory(level, pm);
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
        protected string packFiles(ILevel level, IPM pm)
        {
            if(level.Is(ArgumentType.Object, ArgumentType.StringDouble)) {
                return stPackFiles((Argument[])level.Args[0].data, (string)level.Args[1].data, pm.pinTo(1));
            }

            if(level.Is(ArgumentType.Object, ArgumentType.StringDouble, ArgumentType.EnumOrConst, ArgumentType.EnumOrConst, ArgumentType.Integer))
            {
                return stPackFiles(
                            (Argument[])level.Args[0].data, 
                            (string)level.Args[1].data,
                            (OutArchiveFormat)Enum.Parse(typeof(OutArchiveFormat), (string)level.Args[2].data),
                            (CompressionMethod)Enum.Parse(typeof(CompressionMethod), (string)level.Args[3].data),
                            (CompressionLevel)(int)level.Args[4].data,
                            pm.pinTo(1));
            }

            if(level.Is(ArgumentType.Object, ArgumentType.StringDouble, ArgumentType.Object)) {
                return stPackFiles((Argument[])level.Args[0].data, (string)level.Args[1].data, (Argument[])level.Args[2].data, pm.pinTo(1));
            }

            if(level.Is(ArgumentType.Object, ArgumentType.StringDouble, ArgumentType.Object, ArgumentType.EnumOrConst, ArgumentType.EnumOrConst, ArgumentType.Integer))
            {
                return stPackFiles(
                            (Argument[])level.Args[0].data,
                            (string)level.Args[1].data,
                            (Argument[])level.Args[2].data,
                            (OutArchiveFormat)Enum.Parse(typeof(OutArchiveFormat), (string)level.Args[3].data),
                            (CompressionMethod)Enum.Parse(typeof(CompressionMethod), (string)level.Args[4].data),
                            (CompressionLevel)(int)level.Args[5].data,
                            pm.pinTo(1));
            }

            throw new ArgumentPMException(level, "pack.files(object files, string output [, object except][, enum format, enum method, integer level])");
        }

        /// <summary>
        /// Packing files for signature:
        ///     `pack.files(object files, string output)`
        /// </summary>
        /// <param name="files">Input files.</param>
        /// <param name="name">Output archive.</param>
        /// <param name="pm"></param>
        /// <returns></returns>
        protected string stPackFiles(Argument[] files, string name, IPM pm)
        {
            return stPackFiles(files, name, null, pm);
        }

        /// <summary>
        /// Packing files for signature:
        ///     `pack.files(object files, string output, object except)`
        /// </summary>
        /// <param name="files">Input files.</param>
        /// <param name="name">Output archive.</param>
        /// <param name="except">List of files to exclude from input list.</param>
        /// <param name="pm"></param>
        /// <returns></returns>
        protected string stPackFiles(Argument[] files, string name, Argument[] except, IPM pm)
        {
            return stPackFiles(files, name, except, OutArchiveFormat.Zip, CompressionMethod.Deflate, CompressionLevel.Normal, pm);
        }

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
        protected string stPackFiles(Argument[] files, string name, OutArchiveFormat type, CompressionMethod method, CompressionLevel rate, IPM pm)
        {
            return stPackFiles(files, name, null, type, method, rate, pm);
        }

        /// <summary>
        /// Packing files for signature:
        ///     `pack.files(object files, string output, object except, enum format, enum method, integer level)`
        /// </summary>
        /// <param name="files">Input files.</param>
        /// <param name="name">Output archive.</param>
        /// <param name="except">List of files to exclude from input list.</param>
        /// <param name="type"></param>
        /// <param name="method"></param>
        /// <param name="rate"></param>
        /// <param name="pm"></param>
        /// <returns></returns>
        [Method("files",
                "To compress selected files with default settings.",
                "pack",
                "stPack",
                new string[] { "files", "output" },
                new string[] { "List of files as {\"f1\", \"path\\*.dll\", ..}", "Archive name" },
                CValueType.Void,
                CValueType.Object, CValueType.String)]
        [Method("files",
                "To compress selected files with default settings.",
                "pack",
                "stPack",
                new string[] { "files", "output", "except" },
                new string[] {
                    "List of files as {\"f1\", \"path\\*.dll\", ..}",
                    "Archive name",
                    "List of files to exclude from input list"
                },
                CValueType.Void,
                CValueType.Object, CValueType.String, CValueType.Object)]
        [Method("files",
                "To compress selected files with custom settings.",
                "pack",
                "stPack",
                new string[] { "files", "output", "format", "method", "level" },
                new string[] {
                    "List of files as {\"f1\", \"path\\*.dll\", ..}",
                    "Archive name",
                    "Type of archive: SevenZip, Zip, GZip, BZip2, Tar, XZ",
                    "Compression method: Copy, Deflate, Deflate64, BZip2, Lzma, Lzma2, Ppmd",
                    "Compression level: 0 (None) to 5 (Maximum)"
                },
                CValueType.Void,
                CValueType.Object, CValueType.String, CValueType.Enum, CValueType.Enum, CValueType.UInteger)]
        [Method("files",
                "To compress selected files with custom settings.",
                "pack",
                "stPack",
                new string[] { "files", "output", "except", "format", "method", "level" },
                new string[] {
                    "List of files as {\"f1\", \"path\\*.dll\", ..}",
                    "Archive name",
                    "List of files to exclude from input list",
                    "Type of archive: SevenZip, Zip, GZip, BZip2, Tar, XZ",
                    "Compression method: Copy, Deflate, Deflate64, BZip2, Lzma, Lzma2, Ppmd",
                    "Compression level: 0 (None) to 5 (Maximum)"
                },
                CValueType.Void,
                CValueType.Object, CValueType.String, CValueType.Object, CValueType.Enum, CValueType.Enum, CValueType.UInteger)]
        protected string stPackFiles(Argument[] files, string name, Argument[] except, OutArchiveFormat type, CompressionMethod method, CompressionLevel rate, IPM pm)
        {
            Log.Trace("stPackFiles: `{0}` : type({1}), method({2}), level({3})", name, type, method, rate);

            if(String.IsNullOrWhiteSpace(name)) {
                throw new InvalidArgumentException("The output name of archive is empty.");
            }

            if(files.Length < 1) {
                throw new InvalidArgumentException("List of files is empty.");
            }

            if(files.Any(p => p.type != ArgumentType.StringDouble)) {
                throw new InvalidArgumentException("Incorrect data from input files. Define as {\"f1\", \"f2\", ...}");
            }

            if(except != null && except.Any(p => p.type != ArgumentType.StringDouble)) {
                throw new InvalidArgumentException("Incorrect data from the 'except' argument. Define as {\"f1\", \"f2\", ...}");
            }

            // additional checking of input files. 
            // The SevenZipSharp creates directories if input file is not exist o_O
            string[] input = files.Select((f, i) => pathToFile((string)f.data, i)).ToArray().ExtractFiles();
#if DEBUG
            Log.Trace("stPackFiles: Found files `{0}`", String.Join(", ", input));
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

            SevenZipCompressor zip = new SevenZipCompressor()
            {
                ArchiveFormat       = type,
                CompressionMethod   = method,
                CompressionLevel    = rate,
                CompressionMode     = CompressionMode.Create,
                FastCompression     = true, // to disable some events inside SevenZip
                DirectoryStructure  = true,
            };

            compressFiles(zip, location(name), input);
            return Value.Empty;
        }

        /// <param name="zip">Compressor.</param>
        /// <param name="name">Archive name.</param>
        /// <param name="input">Input files.</param>
        protected virtual void compressFiles(SevenZipCompressor zip, string name, params string[] input)
        {
            zip.CompressFiles(name, input); // -_-
        }

        /// <summary>
        /// Prepares signatures:
        ///     pack.directory(string dir, string output [, enum format, enum method, integer level])
        /// </summary>
        /// <param name="level"></param>
        /// <param name="pm"></param>
        /// <returns></returns>
        protected string packDirectory(ILevel level, IPM pm)
        {
            if(level.Is(ArgumentType.StringDouble, ArgumentType.StringDouble)) {
                return stPackDirectory((string)level.Args[0].data, (string)level.Args[1].data, pm.pinTo(1));
            }

            if(level.Is(ArgumentType.StringDouble, ArgumentType.StringDouble, ArgumentType.EnumOrConst, ArgumentType.EnumOrConst, ArgumentType.Integer))
            {
                return stPackDirectory(
                            (string)level.Args[0].data, 
                            (string)level.Args[1].data,
                            (OutArchiveFormat)Enum.Parse(typeof(OutArchiveFormat), (string)level.Args[2].data),
                            (CompressionMethod)Enum.Parse(typeof(CompressionMethod), (string)level.Args[3].data),
                            (CompressionLevel)(int)level.Args[4].data,
                            pm.pinTo(1));
            }

            throw new ArgumentPMException(level, "pack.directory(string dir, string output [, enum format, enum method, integer level])");
        }

        /// <summary>
        /// Packing directory for signature:
        ///     pack.directory(string dir, string output)
        /// </summary>
        /// <param name="dir">Input directory.</param>
        /// <param name="name">Output archive.</param>
        /// <param name="pm"></param>
        /// <returns></returns>
        protected string stPackDirectory(string dir, string name, IPM pm)
        {
            return stPackDirectory(dir, name, OutArchiveFormat.Zip, CompressionMethod.Deflate, CompressionLevel.Normal, pm);
        }

        /// <summary>
        /// Packing directory for signature:
        ///     pack.directory(string dir, string output, enum format, enum method, integer level)
        /// </summary>
        /// <param name="dir">Input directory.</param>
        /// <param name="name">Output archive.</param>
        /// <param name="type"></param>
        /// <param name="method"></param>
        /// <param name="rate"></param>
        /// <param name="pm"></param>
        /// <returns></returns>
        [Method("directory",
                "To compress selected directory with default settings.",
                "pack",
                "stPack",
                new string[] { "path", "output" },
                new string[] { "Path to directory for packing", "Archive name" },
                CValueType.Void,
                CValueType.String, CValueType.String)]
        [Method("directory",
                "To compress selected directory with custom settings.",
                "pack",
                "stPack",
                new string[] { "path", "output", "format", "method", "level" },
                new string[] {
                    "Path to directory for packing",
                    "Archive name",
                    "Type of archive: SevenZip, Zip, GZip, BZip2, Tar, XZ",
                    "Compression method: Copy, Deflate, Deflate64, BZip2, Lzma, Lzma2, Ppmd",
                    "Compression level: 0 (None) to 5 (Maximum)"
                },
                CValueType.Void,
                CValueType.String, CValueType.String, CValueType.Enum, CValueType.Enum, CValueType.UInteger)]
        protected string stPackDirectory(string dir, string name, OutArchiveFormat type, CompressionMethod method, CompressionLevel rate, IPM pm)
        {
            Log.Trace("stPackDirectory: `{0}` -> `{1}` : type({2}), method({3}), level({4})", dir, name, type, method, rate);

            if(String.IsNullOrWhiteSpace(dir)) {
                throw new InvalidArgumentException("The path to directory is empty.");
            }

            if(String.IsNullOrWhiteSpace(name)) {
                throw new InvalidArgumentException("The output name of archive is empty.");
            }

            string fullpath = location(dir);

            // additional checking of input directory. 
            // The SevenZipSharp creates empty file of archive even if the input directory is not exist o_O
            if(!Directory.Exists(fullpath)) {
                throw new NotFoundException("Directory `{0}` is not found. Looked as `{1}`", dir, fullpath);
            }

            SevenZipCompressor zip = new SevenZipCompressor()
            {
                ArchiveFormat           = type,
                CompressionMethod       = method,
                CompressionLevel        = rate,
                CompressionMode         = CompressionMode.Create,
                FastCompression         = true, // to disable some events inside SevenZip
                IncludeEmptyDirectories = true
            };

            compressDirectory(zip, fullpath, location(name));
            return Value.Empty;
        }

        /// <param name="zip">Compressor.</param>
        /// <param name="path">Input directory.</param>
        /// <param name="name">Archive name.</param>
        protected virtual void compressDirectory(SevenZipCompressor zip, string path, string name)
        {
            zip.CompressDirectory(path, name);
        }

        /// <summary>
        /// Work with unpacking node.
        /// Sample: #[7z unpack]
        /// </summary>
        /// <param name="pm"></param>
        /// <returns></returns>
        [Method("unpack",
                "Extract archive data into same directory.",
                new string[] { "file" },
                new string[] { "Archive for unpacking" },
                CValueType.Void,
                CValueType.String)]
        [Method("unpack",
                "Extract archive data into selected directory.",
                new string[] { "file", "output" },
                new string[] { "Archive for unpacking", "Output path to unpacking archive data" },
                CValueType.Void,
                CValueType.String, CValueType.String)]
        [Method("unpack",
                "Extract data from protected archive into selected directory.",
                new string[] { "file", "output", "pwd" },
                new string[] { "Archive for unpacking", "Output path to unpacking archive data", "Password of archive" },
                CValueType.Void,
                CValueType.String, CValueType.String, CValueType.String)]
        [Method("unpack",
                "Extract archive data into selected directory and delete it after extraction if needed.",
                new string[] { "file", "output", "delete" },
                new string[] { "Archive for unpacking", "Output path to unpacking archive data", "To delete archive after extraction if true" },
                CValueType.Void,
                CValueType.String, CValueType.String, CValueType.Boolean)]
        [Method("unpack",
                "Extract data from protected archive into selected directory and delete it after extraction if needed.",
                new string[] { "file", "output", "delete", "pwd" },
                new string[] { "Archive for unpacking", "Output path to unpacking archive data", "To delete archive after extraction if true", "Password of archive" },
                CValueType.Void,
                CValueType.String, CValueType.String, CValueType.Boolean, CValueType.String)]
        [Method("unpack",
                "Extract archive data into same directory and delete it after extraction if needed.",
                new string[] { "file", "delete" },
                new string[] { "Archive for unpacking", "To delete archive after extraction if true" },
                CValueType.Void,
                CValueType.String, CValueType.Boolean)]
        [Method("unpack",
                "Extract data from protected archive into same directory and delete it after extraction if needed.",
                new string[] { "file", "delete", "pwd" },
                new string[] { "Archive for unpacking", "To delete archive after extraction if true", "Password of archive" },
                CValueType.Void,
                CValueType.String, CValueType.Boolean, CValueType.String)]
        protected string stUnpack(IPM pm)
        {
            if(pm.FinalEmptyIs(LevelType.Method, "unpack")) {
                return unpack(pm.FirstLevel, pm);
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
        protected string unpack(ILevel level, IPM pm)
        {
            // unpack(string file)
            if(level.Is(ArgumentType.StringDouble)) {
                return stUnpackMethod(pm, (string)level.Args[0].data);
            }

            // unpack(string file, string output)
            if(level.Is(ArgumentType.StringDouble, ArgumentType.StringDouble)) {
                return stUnpackMethod(pm, (string)level.Args[0].data, (string)level.Args[1].data);
            }

            // unpack(string file, string output, string pwd)
            if(level.Is(ArgumentType.StringDouble, ArgumentType.StringDouble, ArgumentType.StringDouble)) {
                return stUnpackMethod(pm, (string)level.Args[0].data, (string)level.Args[1].data, false, (string)level.Args[2].data);
            }

            // unpack(string file, string output, boolean delete)
            if(level.Is(ArgumentType.StringDouble, ArgumentType.StringDouble, ArgumentType.Boolean)) {
                return stUnpackMethod(pm, (string)level.Args[0].data, (string)level.Args[1].data, (bool)level.Args[2].data);
            }

            // unpack(string file, string output, boolean delete, string pwd)
            if(level.Is(ArgumentType.StringDouble, ArgumentType.StringDouble, ArgumentType.Boolean, ArgumentType.StringDouble)) {
                return stUnpackMethod(pm, (string)level.Args[0].data, (string)level.Args[1].data, (bool)level.Args[2].data, (string)level.Args[3].data);
            }

            // unpack(string file, boolean delete)
            if(level.Is(ArgumentType.StringDouble, ArgumentType.Boolean)) {
                return stUnpackMethod(pm, (string)level.Args[0].data, null, (bool)level.Args[1].data);
            }

            // unpack(string file, boolean delete, string pwd)
            if(level.Is(ArgumentType.StringDouble, ArgumentType.Boolean, ArgumentType.StringDouble)) {
                return stUnpackMethod(pm, (string)level.Args[0].data, null, (bool)level.Args[1].data, (string)level.Args[2].data);
            }

            throw new InvalidArgumentException("Incorrect arguments to `unpack(string file [, string output][, boolean delete][, string pwd])`");
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
        protected string stUnpackMethod(IPM pm, string file, string output = null, bool delete = false, string pwd = null)
        {
            Log.Trace("stUnpackMethod: `{0}` -> `{1}` : delete({2}), pwd({3})", file, output, delete, (pwd == null)? "none" : "***");

            file = pathToFile(file);

            if(output == null) {
                output = getDirectoryFromFile(file); // same path
            }
            else if(String.IsNullOrWhiteSpace(output)) {
                throw new InvalidArgumentException("The `output` argument can't be empty.");
            }
            else {
                output = location(output);
            }

            extractArchive(file, output, delete, pwd);
            return Value.Empty;
        }

        /// <param name="file">Archive for unpacking.</param>
        /// <param name="output">Output path to unpacking archive data.</param>
        /// <param name="delete">To delete archive after extracting data if true.</param>
        /// <param name="pwd">password of archive if used.</param>
        protected virtual void extractArchive(string file, string output, bool delete, string pwd)
        {
            using(var zip = String.IsNullOrEmpty(pwd) ? new SevenZipExtractor(file) : new SevenZipExtractor(file, pwd))
            {
                try {
                    zip.ExtractArchive(output);
                }
                catch(SevenZipArchiveException ex) {
                    throw new SBEException("Failed extraction data from archive. Check also your password if it's required.", ex);
                }
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
                new string[] { "file" },
                new string[] { "Archive for testing" },
                CValueType.Boolean,
                CValueType.String)]
        [Method("check",
                "Verification data of archive.\nReturns true if no any errors for archive.",
                new string[] { "file", "pwd" },
                new string[] { "Archive for testing", "Password of archive" },
                CValueType.Boolean,
                CValueType.String, CValueType.String)]
        protected string stCheck(IPM pm)
        {
            if(pm.FinalEmptyIs(LevelType.Method, "check")) {
                return check(pm.FirstLevel, pm);
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
        protected string check(ILevel level, IPM pm)
        {
            // check(string file)
            if(level.Is(ArgumentType.StringDouble)) {
                return stCheckMethod(pm, (string)level.Args[0].data);
            }

            // check(string file, string pwd)
            if(level.Is(ArgumentType.StringDouble, ArgumentType.StringDouble)) {
                return stCheckMethod(pm, (string)level.Args[0].data, (string)level.Args[1].data);
            }

            throw new InvalidArgumentException("Incorrect arguments to `check(string file [, string pwd])`");
        }

        /// <summary>
        /// Checking archive for signature:
        ///     check(string file [, string pwd])
        /// </summary>
        /// <param name="pm"></param>
        /// <param name="file">Archive for unpacking.</param>
        /// <param name="pwd">password of archive if used.</param>
        /// <returns></returns>
        protected string stCheckMethod(IPM pm, string file, string pwd = null)
        {
            Log.Trace("stUnpackMethod: `{0}` pwd({1})", file, (pwd == null)? "none" : "***");
            return checkArchive(pathToFile(file), pwd);
        }

        /// <param name="file">Archive for unpacking.</param>
        /// <param name="pwd">password of archive if used.</param>
        /// <returns></returns>
        protected virtual string checkArchive(string file, string pwd)
        {
            using(var zip = String.IsNullOrEmpty(pwd) ? new SevenZipExtractor(file) : new SevenZipExtractor(file, pwd))
            {
                try {
                    return Value.from(zip.Check());
                }
                catch(SevenZipArchiveException ex) {
                    throw new SBEException("Failed extraction data from archive. Check also your password if it's required.", ex);
                }
            }
        }

        /// <summary>
        /// Gets path to item with location for current context.
        /// </summary>
        /// <param name="item"></param>
        /// <returns>Combined path to file or directory.</returns>
        protected virtual string location(string item)
        {
            return Path.Combine(Settings.WPath, item);
        }

        /// <param name="file"></param>
        /// <returns>Path to directory</returns>
        protected virtual string getDirectoryFromFile(string file)
        {
            return Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file));
        }

        /// <param name="file"></param>
        /// <param name="idx">Position from list if used (is not -1)</param>
        /// <returns></returns>
        private string pathToFile(string file, int idx = -1)
        {
            if(String.IsNullOrWhiteSpace(file)) {
                throw new InvalidArgumentException("File name is empty. {0}", 
                                                    (idx != -1)? String.Format("Fail in '{0}' position.", idx) : "");
            }

            string fullpath = location(file);

            if(fullpath.IndexOf('*') == -1 && !File.Exists(fullpath)) { // check existence of file if it's non-mask
                throw new NotFoundException("File `{0}` is not found. Looked as `{1}`. {2}", 
                                                file, 
                                                fullpath, 
                                                (idx != -1)? String.Format("Fail in '{0}' position.", idx) : "");
            }

            return fullpath;
        }
    }
}