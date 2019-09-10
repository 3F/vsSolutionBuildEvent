/*
 * Copyright (c) 2013-2016,2019  Denis Kuzmin < entry.reg@gmail.com > GitHub/3F
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
using net.r_eg.SobaScript.Z.Ext.Extensions;
using net.r_eg.SobaScript.Z.Ext.SevenZip;
using SevenZip;

namespace net.r_eg.vsSBE
{
    internal sealed class SzArchiver: IArchiver
    {
        /// <summary>
        /// The name of library with an complete 7-Zip engine (not a 7za/7zxa).
        /// </summary>
        internal const string LIB_7Z = "7z.dll";

        private bool Is32bit => IntPtr.Size == 4;

        public bool Compress(IEnumerable<string> files, string output, MethodType method, RateType rate, FormatType format)
        {
            var z = GetCompressor(method, rate, format);
            z.DirectoryStructure = true;

            z.CompressFiles(output, files.ToArray());
            return true;
        }

        public bool Compress(string dir, string output, MethodType method, RateType rate, FormatType format)
        {
            var z = GetCompressor(method, rate, format);
            z.IncludeEmptyDirectories = true;

            z.CompressDirectory(dir, output);
            return true;
        }

        public bool Extract(string file, string output, string pwd = null)
        {
            using(var zip = GetExtractor(file, pwd))
            {
                try
                {
                    zip.ExtractArchive(output);
                    return true;
                }
                catch(SevenZipArchiveException ex)
                {
                    Log.Debug($"Failed {nameof(Extract)}: {ex.Message}");
                    return false;
                }
            }
        }

        public bool Check(string file, string pwd = null)
        {
            using(var zip = GetExtractor(file, pwd))
            {
                try
                {
                    return zip.Check();
                }
                catch(SevenZipArchiveException ex)
                {
                    Log.Debug($"Failed {nameof(Check)}: {ex.Message}");
                    return false;
                }
            }
        }

        public SzArchiver()
        {
            string cPath = "".GetExecDir();
            string zPath = Is32bit ? cPath : Path.Combine(cPath, "x64");

            Log.Debug($"7z lib path '{zPath}'");
            try
            {
                SevenZipBase.SetLibraryPath(Path.Combine(zPath, LIB_7Z));
            }
            catch(Exception ex)
            {
                Log.Warn( $"Found problem with library {LIB_7Z} ({zPath}): `{ex.Message}`");
                throw ex;
            }
        }

        private SevenZipCompressor GetCompressor(MethodType method, RateType rate, FormatType format)
        {
            return new SevenZipCompressor()
            {
                CompressionMethod   = Convert(method),
                CompressionLevel    = Convert(rate),
                ArchiveFormat       = Convert(format),
                CompressionMode     = CompressionMode.Create,
                FastCompression     = true, // disables some events inside SevenZip
            };
        }

        private SevenZipExtractor GetExtractor(string file, string pwd = null)
            => string.IsNullOrEmpty(pwd) ? 
                new SevenZipExtractor(file) : new SevenZipExtractor(file, pwd);

        private CompressionMethod Convert(MethodType method)
        {
            switch(method)
            {
                case MethodType.BZip2: return CompressionMethod.BZip2;
                case MethodType.Copy: return CompressionMethod.Copy;
                case MethodType.Deflate: return CompressionMethod.Deflate;
                case MethodType.Deflate64: return CompressionMethod.Deflate64;
                case MethodType.Lzma: return CompressionMethod.Lzma;
                case MethodType.Lzma2: return CompressionMethod.Lzma2;
                case MethodType.Ppmd: return CompressionMethod.Ppmd;
            }

            throw new NotSupportedException(nameof(method));
        }

        private CompressionLevel Convert(RateType rate)
        {
            switch(rate)
            {
                case RateType.Fast: return CompressionLevel.Fast;
                case RateType.High: return CompressionLevel.High;
                case RateType.Low: return CompressionLevel.Low;
                case RateType.None: return CompressionLevel.None;
                case RateType.Normal: return CompressionLevel.Normal;
                case RateType.Ultra: return CompressionLevel.Ultra;
            }

            throw new NotSupportedException(nameof(rate));
        }

        private OutArchiveFormat Convert(FormatType format)
        {
            switch(format)
            {
                case FormatType.BZip2: return OutArchiveFormat.BZip2;
                case FormatType.GZip: return OutArchiveFormat.GZip;
                case FormatType.SevenZip: return OutArchiveFormat.SevenZip;
                case FormatType.Tar: return OutArchiveFormat.Tar;
                case FormatType.XZ: return OutArchiveFormat.XZ;
                case FormatType.Zip: return OutArchiveFormat.Zip;
            }

            throw new NotSupportedException(nameof(format));
        }
    }
}
