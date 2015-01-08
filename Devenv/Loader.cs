/*
 * The MIT License (MIT)
 * 
 * Copyright (c) 2013-2015  Denis Kuzmin (reg) <entry.reg@gmail.com>
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
*/

using System;
using System.IO;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Microsoft.Win32;

namespace net.r_eg.vsSBE.Devenv
{
    public class Loader
    {
        public const string GUID = "94ecd13f-15f3-4f51-9afd-17f0275c6266";

        public Library Library
        {
            get;
            protected set;
        }

        public Loader(DTE2 dte2, AddIn addIn)
        {
            string path = extractPath(addIn.SatelliteDllPath);
            if(!Library.existsIn(path)) {
                path = findWithRegistry(dte2.RegistryRoot);
            }
            this.Library = new Library(dte2, path);
        }

        protected string findWithRegistry(string root)
        {
            string keypath = String.Format(@"{0}\ExtensionManager\EnabledExtensions", root);
            using(RegistryKey rk = Registry.CurrentUser.OpenSubKey(keypath))
            {
                string name = rk.GetValueNames().FirstOrDefault(x => x.Contains(GUID));
                if(!String.IsNullOrEmpty(name)) {
                    return extractPath(rk.GetValue(name).ToString());
                }
            }
            throw new DllNotFoundException(String.Format("Not found '{0}' with Registry", GUID));
        }

        protected string extractPath(string file)
        {
            string dir = Path.GetDirectoryName(file);

            if(dir.ElementAt(dir.Length - 1) != Path.DirectorySeparatorChar) {
                dir += Path.DirectorySeparatorChar;
            }
            return dir;
        }
    }
}
