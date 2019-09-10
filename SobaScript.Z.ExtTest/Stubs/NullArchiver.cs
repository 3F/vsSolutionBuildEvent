using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using net.r_eg.SobaScript.Z.Ext.SevenZip;

namespace SobaScript.Z.ExtTest.Stubs
{
    internal sealed class NullArchiver: IArchiver
    {
        public string ArchiveName { get; set; }
        public ReadOnlyCollection<string> FilesInput { get; set; }
        public string DirPath { get; set; }

        public bool Check(string file, string pwd = null)
        {
            return true;
        }

        public bool Compress(IEnumerable<string> files, string output, MethodType method, RateType rate, FormatType format)
        {
            ArchiveName = output;
            FilesInput = new ReadOnlyCollection<string>(files.ToArray());
            //base.compressFiles(zip, name, input);
            return true;
        }

        public bool Compress(string dir, string output, MethodType method, RateType rate, FormatType format)
        {
            ArchiveName = output;
            DirPath = dir;
            //base.compressDirectory(zip, path, name);
            return true;
        }

        public bool Extract(string file, string output, string pwd = null)
        {
            return true;
        }
    }
}
