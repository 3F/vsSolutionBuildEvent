/*
    * The MIT License (MIT)
    * 
    * Copyright (c) 2013 Developed by reg <entry.reg@gmail.com>
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Windows.Forms;

namespace reg.ext.vsSolutionBuildEvent
{
    class Config
    {
        public static SolutionEvents data = null;
        private static string _path;

        public static void load(string path)
        {
            _path               = path;
            FileStream stream   = null;
            try
            {
                XmlSerializer xml   = new XmlSerializer(typeof(SolutionEvents));
                stream              = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                data                = (SolutionEvents)xml.Deserialize(stream);
            }
            catch (Exception)
            {
                //Debug.Assert(false);
                data = new SolutionEvents();
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }
        }

        /// <summary>
        /// with changing path
        /// </summary>
        public static void save(string path)
        {
            _path = path;
            save();
        }

        public static void save()
        {
            TextWriter stream = null;
            try
            {
                if (data == null)
                {
                    data = new SolutionEvents();
                }
                XmlSerializer xml   = new XmlSerializer(typeof(SolutionEvents));
                stream              = new StreamWriter(_path);

                xml.Serialize(stream, data);
            }
            catch (Exception e)
            {
                MessageBox.Show("Failed save settings:\n" + e.Message, "Solution BuildEvent", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }
        }

        private Config(){}
    }
}
