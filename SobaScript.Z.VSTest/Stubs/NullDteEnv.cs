using net.r_eg.SobaScript.Z.VS.Dte;

namespace SobaScript.Z.VSTest.Stubs
{
    internal sealed class NullDteEnv: IDteEnv
    {
        private readonly object sync = new object();

        public bool IsAvaialbleDteCmd
        {
            get;
            set;
        } = true;

        public IDteCommand LastCmd
        {
            get;
            private set;
        } = new _DteCommand();

        public void Execute(string cmd) { }

        internal void EmulateBeforeExecute(string guid, int id, object customIn, object customOut, bool cancelDefault)
            => CommandEvent(true, guid, id, customIn, customOut);

        internal void EmulateAfterExecute(string guid, int id, object customIn, object customOut)
            => CommandEvent(false, guid, id, customIn, customOut);

        public NullDteEnv()
        {

        }

        private void CommandEvent(bool pre, string guid, int id, object customIn, object customOut)
        {
            LastCmd = new _DteCommand()
            {
                Guid        = guid,
                Id          = id,
                CustomIn    = customIn,
                CustomOut   = customOut,
                Pre         = pre
            };
        }

        private sealed class _DteCommand: IDteCommand
        {
            public string Guid { get; set; }

            public int Id { get; set; }

            public object CustomIn { get; set; }

            public object CustomOut { get; set; }

            public bool Cancel { get; set; }

            public bool Pre { get; set; }
        }
    }
}
