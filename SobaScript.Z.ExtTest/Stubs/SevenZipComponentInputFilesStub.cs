using net.r_eg.SobaScript;
using net.r_eg.SobaScript.Z.Ext;

namespace SobaScript.Z.ExtTest.Stubs
{
    internal class SevenZipComponentInputFilesStub: SevenZipComponent
    {
        public NullArchiver Archiver => (NullArchiver)archiver;

        public SevenZipComponentInputFilesStub()
            : base(new Soba(), new NullArchiver(), "")
        {

        }
    }
}
