using System.IO;
using net.r_eg.SobaScript;
using net.r_eg.SobaScript.Z.Ext;

namespace SobaScript.Z.ExtTest.Stubs
{
    internal class FileComponentPathStub: FileComponent
    {
        public bool CmpPaths(string p1, string p2)
        {
            return p1.TrimStart(Path.DirectorySeparatorChar) == p2.TrimStart(Path.DirectorySeparatorChar);
        }

        public FileComponentPathStub()
            : base(new Soba(), "")
        {

        }
    }
}
