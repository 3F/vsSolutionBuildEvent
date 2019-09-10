using System;
using System.IO;

namespace SobaScript.Z.ExtTest.Stubs
{
    internal sealed class TempFile: IDisposable
    {
        public string Dir
        {
            get;
            private set;
        }

        public string File
        {
            get;
            private set;
        }

        public TempFile(bool insideDir = false, string ext = null)
        {
            string path = Path.GetTempPath();
            string name = Guid.NewGuid().ToString();

            if(ext != null) {
                name += ext;
            }

            File = Path.Combine(path, name);
            if(insideDir) {
                Dir = Directory.CreateDirectory(File).FullName;
                File = Path.Combine(Dir, name);
            }

            using(var f = System.IO.File.Create(File)) { }
        }

        public void Dispose()
        {
            try
            {
                System.IO.File.Delete(File);
                if(Dir != null) {
                    Directory.Delete(Dir);
                }
            }
            catch { /* we're working inside temp directory with unique name, so it's not important */ }
        }
    }
}
