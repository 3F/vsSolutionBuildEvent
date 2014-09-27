using System;

namespace net.r_eg.vsSBE.Version
{
    class Program
    {
        static void Main(string[] args)
        {
            //TODO:
            if(args.Length < 4) {
                Console.WriteLine("arguments required: 'SolutionDir' 'Version.tpl' 'Version.cs' 'vsixmanifest'");
                Console.ReadLine();
                return;
            }

            try {
                string sln = args[0].Trim();
                Update upd = new Update(sln + "_version", 
                                        sln + ".git", 
                                        sln + "Version/Version.tpl", 
                                        sln + "vsSolutionBuildEvent/Version.cs", 
                                        sln + "vsSolutionBuildEvent/source.extension.vsixmanifest");

                Console.WriteLine("'{0}' successfully updated", upd.Version.ToString());
            }
            catch(Exception ex) {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}