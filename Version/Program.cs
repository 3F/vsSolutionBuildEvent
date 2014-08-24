using System;

namespace net.r_eg.vsSBE.Version
{
    class Program
    {
        static void Main(string[] args)
        {
            //TODO:
            if(args.Length < 5) {
                Console.WriteLine("arguments required: '_version' '.git' 'Version.tpl' 'Version.cs' 'vsixmanifest'");
                Console.ReadLine();
                return;
            }

            try {
                Update upd = new Update(args[0], args[1], args[2], args[3], args[4]);
                Console.WriteLine("'{0}' successfully updated", upd.Version.ToString());
            }
            catch(Exception ex) {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}