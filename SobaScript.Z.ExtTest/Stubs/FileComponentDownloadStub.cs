using net.r_eg.SobaScript;
using net.r_eg.SobaScript.Z.Ext;

namespace SobaScript.Z.ExtTest.Stubs
{
    internal class FileComponentDownloadStub: FileComponent
    {
        public string addr, output, user, pwd;

        public FileComponentDownloadStub()
            : base(new Soba(), "")
        {

        }

        protected override string Download(string addr, string output, string user = null, string pwd = null)
        {
            this.addr   = addr;
            this.output = output;
            this.user   = user;
            this.pwd    = pwd;
            //return base.download(addr, output, user, pwd);
            return Value.Empty;
        }
    }
}
