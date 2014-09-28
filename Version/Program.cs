using System;

namespace net.r_eg.vsSBE.Version
{
    class Program
    {
        static void Main(string[] args)
        {
            //TODO:
            if(args.Length < 5) {
                Console.WriteLine("arguments required: 'Configuration' 'SolutionDir' 'Version.tpl' 'Version.cs' 'vsixmanifest'");
                Console.ReadLine();
                return;
            }

            try {
                string cfg = args[0].Trim();
                string sln = args[1].Trim();

                Update.Data data    = new Update.Data();
                data.version        = sln + "_version";
                data.git            = sln + ".git";
                data.tpl            = sln + "Version/Version.tpl";
                data.cs             = sln + "vsSolutionBuildEvent/Version.cs";
                data.manifest       = sln + "vsSolutionBuildEvent/source.extension.vsixmanifest";

                Update upd = new Update(data, cfg.EndsWith("_with_revision"));

                Console.WriteLine("'{0}' successfully updated", upd.Version.ToString());
            }
            catch(Exception ex) {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}