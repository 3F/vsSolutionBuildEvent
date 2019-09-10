using System;
using System.IO;
using net.r_eg.SobaScript;
using net.r_eg.SobaScript.Z.Ext;

namespace SobaScript.Z.ExtTest.Stubs
{
    internal class SevenZipComponentExtractArchiveStub: SevenZipComponent
    {
        public string file { get; set; }
        public string output { get; set; }
        public bool delete { get; set; }
        public string pwd { get; set; }

        public new string Location(string file)
        {
            return base.Location(file);
        }

        public new string GetDirectoryFromFile(string output)
        {
            return base.GetDirectoryFromFile(output);
        }

        public SevenZipComponentExtractArchiveStub()
            : base(new Soba(), new NullArchiver(), "")
        {

        }

        protected override void ExtractArchive(string file, string output, bool delete, string pwd)
        {
            this.file = file;
            this.output = output;
            this.delete = delete;
            this.pwd = pwd;
            //base.extractArchive(file, output, delete, pwd);
        }
    }
}
