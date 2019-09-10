using System.Runtime.InteropServices;
using System.Text;
using net.r_eg.SobaScript;
using net.r_eg.SobaScript.Exceptions;
using net.r_eg.SobaScript.Z.Ext;

namespace SobaScript.Z.ExtTest.Stubs
{
    internal class FileComponentAcs: FileComponent
    {
        public bool throwError = false;
        protected string content = "content from file";

        public FileComponentAcs(bool throwError = false)
            : base(new Soba(), "")
        {
            this.throwError = throwError;
        }

        protected override string ReadToEnd(string file, Encoding enc, bool detectEncoding)
        {
            if(throwError) {
                throw new System.IO.FileNotFoundException(string.Format("Some error for '{0}'", file));
            }
            return content;
        }

        protected override Encoding DetectEncodingFromFile(string file)
        {
            return Encoding.UTF8;
        }

        protected override void WriteToFile(string file, string data, bool append, bool writeLine, Encoding enc)
        {
            if(throwError) {
                throw new System.IO.IOException(string.Format("Some error for '{0}'", file));
            }
            content = data;
        }

        protected override string FindFile(string file)
        {
            return file;
        }

        protected override string Run(string file, string args, bool silent, bool stdOut, int timeout = 0)
        {
            if(throwError) {
                throw new ExternalException(string.Format("Some error for '{0} {1}'", file, args));
            }
            return string.Format("{0}stdout", silent? "silent ": string.Empty);
        }
    }
}
