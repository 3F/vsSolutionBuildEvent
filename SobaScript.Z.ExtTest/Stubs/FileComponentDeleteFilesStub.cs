namespace SobaScript.Z.ExtTest.Stubs
{
    internal class FileComponentDeleteFilesStub: FileComponentPathStub
    {
        public string[] files;

        protected override void DeleteFiles(string[] files)
        {
            this.files = files;
            //base.deleteFiles(files);
        }
    }
}
