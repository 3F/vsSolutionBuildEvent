using net.r_eg.SobaScript;

namespace SobaScript.Z.ExtTest.Stubs
{
    internal class SevenZipComponentCheckArchiveStub: SevenZipComponentExtractArchiveStub
    {
        protected override string CheckArchive(string file, string pwd)
        {
            this.file = file;
            this.pwd = pwd;
            return Value.Empty;
            //return base.checkArchive(file, pwd);
        }
    }
}
