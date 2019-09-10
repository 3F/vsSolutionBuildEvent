using System.Collections.Generic;

namespace SobaScript.Z.ExtTest.Stubs
{
    internal class FileComponentCopyDirectoryStub: FileComponentPathStub
    {
        public IEnumerable<string[]> files;
        public string dest;
        public bool force, overwrite;

        protected override void CopyDirectory(IEnumerable<string[]> files, string dest, bool force, bool overwrite)
        {
            this.files      = files;
            this.dest       = dest;
            this.force      = force;
            this.overwrite  = overwrite;
            //base.copyDirectory(files, dest, force, overwrite);
        }

        protected override void Mkdir(string path)
        {
            this.dest = path;
            //base.mkdir(path);
        }
    }
}
