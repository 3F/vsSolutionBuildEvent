using System;
using System.IO;

namespace SobaScript.Z.ExtTest.Stubs
{
    internal class FileComponentCopyFileStub: FileComponentPathStub
    {
        public string destDir, destFile;
        public bool overwrite;
        public string[] files;

        protected override void CopyFile(string destDir, string destFile, bool overwrite, params string[] files)
        {
            this.destDir    = destDir.TrimStart(Path.PathSeparator);
            this.destFile   = destFile.TrimStart(Path.PathSeparator);
            this.overwrite  = overwrite;
            this.files      = files;
            //base.copyFile(destDir, destFile, overwrite, files);
        }
    }
}
