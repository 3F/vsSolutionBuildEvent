/*
 * The MIT License (MIT)
 * 
 * Copyright (c) 2013-2016,2019  Denis Kuzmin < entry.reg@gmail.com > GitHub/3F
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
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;

namespace ClientDemo
{
    internal class Log: ILog
    {
        /// <summary>
        /// Message for information level.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public void info(string message, params object[] args)
        {
            write(String.Format(message, args));
        }

        /// <summary>
        /// Show messages if it's possible
        /// </summary>
        public void show()
        {
            if(status == null) {
                return;
            }
            
            Task.Factory.StartNew(() => {
                status.show();
            });
        }

        /// <summary>
        /// Initialize with IStatus
        /// </summary>
        /// <param name="status"></param>
        /// <returns>self reference</returns>
        public ILog init(IStatus status)
        {
            this.status = status;
            return this;
        }

        /// <summary>
        /// Gets instance from Log
        /// </summary>
        public static Log _
        {
            get { return _lazy.Value; }
        }
        private static readonly Lazy<Log> _lazy = new Lazy<Log>(() => new Log());

        /// <summary>
        /// Additional reporter
        /// </summary>
        protected IStatus status;


        /// <summary>
        /// Where and how to write message
        /// </summary>
        /// <param name="msg"></param>
        protected void write(string msg)
        {
            msg = format(msg);

            if(status != null) {
                status.report(msg);
            }

            Console.Write(msg);
            Debug.Write(msg);
        }

        /// <summary>
        /// The format of messages
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        protected virtual string format(string msg)
        {
            string tFormat = CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern + " .fff";
            return String.Format("[{0}] {1}{2}", DateTime.Now.ToString(tFormat), msg, Environment.NewLine);
        }

        private Log() { }
    }
}