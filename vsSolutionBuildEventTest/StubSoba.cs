using net.r_eg.SobaScript;
using net.r_eg.Varhead;

namespace net.r_eg.vsSBE.Test
{
    internal static class StubSoba
    {
        public static ISobaScript MakeNew() 
            => MakeNew(new StubEnv());

        public static ISobaScript MakeNew(IEnvironment env) 
            => Bootloader.Configure(new Soba(), env);

        public static ISobaScript MakeNew(IUVars uvars) 
            => MakeNew(uvars, new StubEnv());

        public static ISobaScript MakeNew(IUVars uvars, IEnvironment env)
            => Bootloader.Configure(new Soba(uvars), env);
    }
}
