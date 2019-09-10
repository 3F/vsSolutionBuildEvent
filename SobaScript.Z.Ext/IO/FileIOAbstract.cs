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
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using net.r_eg.Components;
using net.r_eg.SobaScript.Components;
using net.r_eg.SobaScript.Exceptions;
using net.r_eg.SobaScript.SNode;
using net.r_eg.SobaScript.Z.Ext.Extensions;

namespace net.r_eg.SobaScript.Z.Ext.IO
{
    public abstract class FileIOAbstract: ComponentAbstract
    {
        /// <summary>
        /// I/O Default encoding.
        /// </summary>
        protected Encoding defaultEncoding = new UTF8Encoding(false);

        public abstract IEnumerable<string> EnvPath { get; }

        public abstract IExer Exer { get; protected set; }

        protected abstract IEncDetector EncDetector { get; set; }

        protected FileIOAbstract(ISobaScript soba)
            : base(soba)
        {

        }

        /// <summary>
        /// Get all directories from Environment PATH and other.
        /// </summary>
        protected IEnumerable<string> GetEnvPath(EnvironmentVariableTarget target = EnvironmentVariableTarget.Process)
            => string.Format
        (
            "{0};{1};{2};{3};{4}",
            Environment.SystemDirectory,
            Environment.GetEnvironmentVariable("SystemRoot"),
            Environment.GetEnvironmentVariable("SystemRoot") + @"\System32\Wbem",
            Environment.GetEnvironmentVariable("SystemRoot") + @"\System32\WindowsPowerShell\v1.0\",
            Environment.GetEnvironmentVariable("PATH", target)
        )
        .Split(';');

        protected string CopyFile(IPM pm, RArgs files, string dest, bool overwrite, RArgs except = null)
        {
            dest = dest.DirectoryPathFormat();

            foreach(Argument src in files)
            {
                if(src.type != ArgumentType.StringDouble) {
                    throw new PMArgException(src, "Input files. Define as {\"f1\", \"f2\", ...}");
                }
                CopyFile(pm, src.data.ToString(), dest, overwrite, except);
            }

            return Value.Empty;
        }

        protected string CopyFile(IPM pm, string src, string dest, bool overwrite, RArgs except = null)
        {
            if(string.IsNullOrWhiteSpace(src) || string.IsNullOrWhiteSpace(dest)) {
                throw new ArgumentException("The source file or the destination path argument is empty.");
            }

            if(except != null && except.Any(p => p.type != ArgumentType.StringDouble)) {
                throw new PMArgException(except, "'except' argument. Define as {\"f1\", \"f2\", ...}");
            }

            dest = GetLocation(dest.TrimEnd());
            string destDir  = Path.GetDirectoryName(dest);
            string destFile = Path.GetFileName(dest);

            src = GetLocation(src);
            string[] input = new[] { src }.ExtractFiles(Exer.BasePath);
#if DEBUG
            LSender.Send(this, $"Found files to copy `{string.Join(", ", input)}`", MsgLevel.Trace);
#endif
            if(except != null)
            {
                string path = Path.GetDirectoryName(src);
                input = input.Except
                (
                    except
                    .Where(f => !string.IsNullOrWhiteSpace((string)f.data))
                    .Select(f => GetLocation((string)f.data, path))
                    .ToArray()
                    .ExtractFiles(Exer.BasePath)
                )
                .ToArray();
            }

            if(input.Length < 1) {
                throw new ArgumentException("The input files was not found. Check your mask and the exception list if used.");
            }

            CopyFile(destDir, destFile, overwrite, input);
            return Value.Empty;
        }

        protected virtual void CopyFile(string destDir, string destFile, bool overwrite, params string[] files)
        {
            if(!Directory.Exists(destDir))
            {
                LSender.Send(this, $"Trying to create directory `{destDir}`", MsgLevel.Trace);
                Directory.CreateDirectory(destDir);
            }

            bool isDestFile = !string.IsNullOrWhiteSpace(destFile);
            if(isDestFile && files.Length > 1)
            {
                throw new ArgumentException(
                    string.Format(
                        "The destination path `{0}` cannot contain file name `{1}` if the source has 2 or more files for used mask. End with `{1}\\` or `{1}/` if it directory.", 
                        destDir, 
                        destFile
                    )
                );
            }

            foreach(string file in files)
            {
                string dest = Path.Combine(destDir, isDestFile ? destFile : Path.GetFileName(file));
                LSender.Send(this, $"Copy file `{file}` to `{dest}` overwrite({overwrite})", MsgLevel.Trace);
                File.Copy(file, dest, overwrite);
            }
        }

        protected string CopyDirectory(IPM pm, string src, string dest, bool force, bool overwrite = false)
        {
            if(string.IsNullOrWhiteSpace(dest)) {
                throw new ArgumentException("The destination directory argument is empty.");
            }

            dest = Path.GetDirectoryName(GetLocation(dest.DirectoryPathFormat()));

            if(string.IsNullOrWhiteSpace(src))
            {
                if(force) {
                    Mkdir(dest);
                    return Value.Empty;
                }
                throw new ArgumentException($"Use `force` flag if you want to create directory `{dest}`");
            }

            src = GetLocation(src.DirectoryPathFormat());

            var files = Directory.EnumerateFiles(src, "*.*", SearchOption.AllDirectories)
                                    .Select(f => new[] { f, Path.Combine(dest, f.Substring(src.Length)) });

            CopyDirectory(files, dest, force, overwrite);
            return Value.Empty;
        }

        protected virtual void CopyDirectory(IEnumerable<string[]> files, string dest, bool force, bool overwrite)
        {
            if(!Directory.Exists(dest))
            {
                if(!force) {
                    throw new NotFoundException(dest, "Check path or use `force` flag");
                }

                LSender.Send(this, $"Trying to create directory `{dest}`", MsgLevel.Trace);
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

                LSender.Send(this, $"Copy directory: file `{from}` to `{to}` overwrite({overwrite})", MsgLevel.Trace);
                File.Copy(from, to, overwrite);
            }
        }

        protected string DeleteFiles(IPM pm, RArgs files, RArgs except = null)
        {
            if(files.Any(p => p.type != ArgumentType.StringDouble)) {
                throw new PMArgException(files, "Input files. Define as {\"f1\", \"f2\", ...}");
            }

            if(except != null && except.Any(p => p.type != ArgumentType.StringDouble)) {
                throw new PMArgException(except, "'except' argument. Define as {\"f1\", \"f2\", ...}");
            }

            string exs(string file, int idx)
            {
                if(!string.IsNullOrWhiteSpace(file)) {
                    return GetLocation(file);
                }
                throw new ArgumentException($"File name is empty. Fail in '{idx}' position.");
            }

            string[] input = files.Select((f, i) => exs((string)f.data, i)).ToArray().ExtractFiles(Exer.BasePath);
#if DEBUG
            LSender.Send(this, $"DeleteFiles: Found files `{string.Join(", ", input)}`", MsgLevel.Trace);
#endif
            if(except != null)
            {
                input = input.Except
                (
                    except
                    .Where(f => !string.IsNullOrWhiteSpace((string)f.data))
                    .Select(f => GetLocation((string)f.data))
                    .ToArray()
                    .ExtractFiles(Exer.BasePath)
                )
                .ToArray();
            }

            if(input.Length < 1) {
                throw new PMArgException(files, "Input files was not found. Check your mask and the exception list if used.");
            }

            DeleteFiles(input);
            return Value.Empty;
        }

        protected virtual void DeleteFiles(string[] files)
        {
            foreach(string file in files)
            {
                LSender.Send(this, $"Delete file `{file}`", MsgLevel.Trace);
                File.Delete(file);
            }
        }

        protected string DeleteDirectory(IPM pm, string src, bool force)
        {
            if(string.IsNullOrWhiteSpace(src)) {
                throw new ArgumentException("The source directory is empty.");
            }

            DeleteDirectory(GetLocation(src), force);
            return Value.Empty;
        }

        protected virtual void DeleteDirectory(string src, bool force)
        {
            LSender.Send(this, $"Delete directory `{src}` /force: {force}", MsgLevel.Trace);

            if(Directory.Exists(src)) { // to avoid errors like `File.Delete`
                Directory.Delete(src, force);
            }
        }

        protected virtual void Mkdir(string path)
        {
            if(!Directory.Exists(path))
            {
                LSender.Send(this, $"Create empty directory `{path}`", MsgLevel.Trace);
                Directory.CreateDirectory(path);
            }
        }

        /// <param name="file">The file to be read</param>
        /// <param name="enc">The character encoding</param>
        /// <param name="detectEncoding">Indicates whether to look for byte order marks at the beginning of the file</param>
        protected virtual string ReadToEnd(string file, Encoding enc, bool detectEncoding = false)
        {
            using(StreamReader stream = new StreamReader(file, enc, detectEncoding)) {
                return stream.ReadToEnd();
            }
        }

        protected string ReadToEnd(string file)
        {
            return ReadToEnd(file, defaultEncoding, true);
        }

        /// <param name="file"></param>
        /// <param name="data"></param>
        /// <param name="append">Flag to append the content to the end of the file</param>
        /// <param name="newline">To write with newline if true</param>
        /// <param name="enc">The character encoding</param>
        protected virtual void WriteToFile(string file, string data, bool append, bool newline, Encoding enc)
        {
#if DEBUG
            LSender.Send(this, $"File `{file}` write with Encoding '{enc.EncodingName}'", MsgLevel.Trace);
#endif
            using(TextWriter stream = new StreamWriter(file, append, enc))
            {
                if(newline) {
                    stream.WriteLine(data);
                }
                else {
                    stream.Write(data);
                }
            }
        }

        protected void WriteToFile(string file, string data, bool append, bool newline)
        {
            WriteToFile(file, data, append, newline, defaultEncoding);
        }

        protected void WriteToFile(string file, string data, bool append, Encoding enc)
        {
            WriteToFile(file, data, append, false, enc);
        }

        /// <summary>
        /// Write into standard output stream.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="newline">To write with newline if true</param>
        protected void WriteToStdOut(string data, bool newline)
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
        protected void WriteToStdErr(string data, bool newline)
        {
            if(newline) {
                Console.Error.WriteLine(data);
            }
            else {
                Console.Error.Write(data);
            }
        }

        /// <param name="type">search type</param>
        /// <param name="content"></param>
        /// <param name="pattern"></param>
        /// <param name="replacement"></param>
        private protected string StReplaceEngine(SearchType type, ref string content, string pattern, string replacement)
        {
            switch(type)
            {
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

        /// <summary>
        /// Auto detecting encoding from file.
        /// </summary>
        protected virtual Encoding DetectEncodingFromFile(string file)
        {
            using(FileStream fs = File.OpenRead(file))
            {
                var enc = EncDetector?.Detect(fs);

                if(enc == null) {
                    LSender.Send(this, $"Problem with detection of encoding for '{file}'", MsgLevel.Debug);
                    return defaultEncoding; // good luck
                }

                if(enc == Encoding.UTF8)
                {
                    fs.Seek(0, SeekOrigin.Begin);

                    return (fs.ReadByte() == 0xEF && 
                            fs.ReadByte() == 0xBB && 
                            fs.ReadByte() == 0xBF) ? new UTF8Encoding(true) : new UTF8Encoding(false);
                }

                return enc;
            }
        }

        protected virtual string Download(string addr, string output, string user = null, string pwd = null)
        {
            var wc = new WebClient();
            if(user != null) {
                wc.Credentials = new NetworkCredential(user, pwd ?? "");
            }

            wc.DownloadFile(addr, GetLocation(output));
            return Value.Empty;
        }

        /// <summary>
        /// Execute file with arguments
        /// </summary>
        /// <param name="file"></param>
        /// <param name="args"></param>
        /// <param name="silent">Hide process if true</param>
        /// <param name="stdOut">Reads from StandardOutput if true</param>
        /// <param name="timeout">How long to wait the execution, in seconds. 0 value - infinitely</param>
        protected virtual string Run(string file, string args, bool silent, bool stdOut, int timeout = 0)
        {
            string ret = Exer.Run(file, args, silent, timeout);
            return stdOut ? ret : string.Empty;
        }

        /// <summary>
        /// Gets full path to file with environment PATH.
        /// </summary>
        /// <returns>null value if file is not found</returns>
        protected virtual string FindFile(string file)
        {
            string lfile = GetLocation(file);
            if(File.Exists(lfile)) {
                return lfile;
            }

            LSender.Send(this, $"finding file with environment PATH :: `{file}`({lfile})", MsgLevel.Trace);

            string[] exts = Environment.GetEnvironmentVariable("PATHEXT").Split(';');

            foreach(string dir in EnvPath)
            {
                lfile = GetLocation(file, dir);
                if(File.Exists(lfile) || exts.Any(ext => File.Exists(lfile + ext))) {
                    LSender.Send(this, $"found in: '{dir}' :: '{lfile}'", MsgLevel.Trace);
                    return lfile;
                }
            }

            return null;
        }

        /// <summary>
        /// Location of file for specific path.
        /// </summary>
        protected string GetLocation(string file, string path) => Path.Combine(path, file);

        /// <summary>
        /// Location of file for current context.
        /// </summary>
        /// <returns>Full path to file</returns>
        protected string GetLocation(string file) => Path.Combine(Exer.BasePath, file);

        protected Encoding GetEncoding(string name)
        {
            if(string.IsNullOrWhiteSpace(name)) {
                throw new ArgumentException("Name of encoding is null or empty.");
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
