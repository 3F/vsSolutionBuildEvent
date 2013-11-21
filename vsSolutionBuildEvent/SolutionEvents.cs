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

namespace reg.ext.vsSolutionBuildEvent
{
    interface ISolutionEvent
    {
        /// <summary>
        /// execution of shell command
        /// </summary>
        string command { get; set; }

        /// <summary>
        /// output information to "Output" window or something else...
        /// </summary>
        string caption { get; set; }

        /// <summary>
        /// status of activate
        /// </summary>
        bool enabled { get; set; }

        /// <summary>
        /// Hide Process
        /// </summary>
        bool processHide { get; set; }

        /// <summary>
        /// not close after completion
        /// </summary>
        bool processKeep { get; set; }

        /// <summary>
        /// script or files mode
        /// </summary>
        bool modeScript { get; set; }

        /// <summary>
        /// stream processor
        /// </summary>
        string interpreter { get; set; }

        /// <summary>
        /// treat newline as
        /// </summary>
        string newline { get; set; }

        /// <summary>
        /// symbol wrapper for commands or script
        /// </summary>
        string wrapper { get; set; }

        /// <summary>
        /// Wait until terminates script handling
        /// </summary>
        bool waitForExit { get; set; }
    }


    [Serializable]
    public class SolutionEvents
    {
        private Event _preBuild = new Event();
        /// <summary>
        /// Before building solution
        /// </summary>
        public Event preBuild
        {
            get { return _preBuild; }
            set { _preBuild = value; }
        }

        private Event _postBuild = new Event();
        /// <summary>
        /// After building solution
        /// </summary>
        public Event postBuild
        {
            get { return _postBuild; }
            set { _postBuild = value; }
        }

        private Event _cancelBuild = new Event();
        /// <summary>
        /// When cancel building solution
        /// e.g. fatal error of compilation or cancel of user
        /// </summary>
        public Event cancelBuild
        {
            get { return _cancelBuild; }
            set { _cancelBuild = value; }
        }
    }

    public class Event : ISolutionEvent
    {
        private string _command = "";
        /// <summary>
        /// execution of shell command
        /// </summary>
        public string command
        {
            get { return _command; }
            set { _command = value; }
        }

        private string _caption = "";
        /// <summary>
        /// output information to "Output" window or something else...
        /// </summary>
        public string caption
        {
            get { return _caption; }
            set { _caption = value; }
        }

        private bool _enabled = false;
        /// <summary>
        /// status of activate
        /// </summary>
        public bool enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        private bool _processHide = true;
        public bool processHide
        {
            get { return _processHide; }
            set { _processHide = value; }
        }

        private bool _modeScript = true;
        /// <summary>
        /// not close after completion
        /// </summary>
        public bool modeScript
        {
            get { return _modeScript; }
            set { _modeScript = value; }
        }

        private bool _processKeep = false;
        /// <summary>
        /// not close after completion
        /// </summary>
        public bool processKeep
        {
            get { return _processKeep; }
            set { _processKeep = value; }
        }

        private string _interpreter = "";
        /// <summary>
        /// stream processor
        /// </summary>
        public string interpreter
        {
            get { return _interpreter; }
            set { _interpreter = value; }
        }

        private string _newline = "";
        /// <summary>
        /// treat newline as
        /// </summary>
        public string newline
        {
            get { return _newline; }
            set { _newline = value; }
        }

        private string _wrapper = "";
        /// <summary>
        /// symbol wrapper for commands or script
        /// </summary>
        public string wrapper
        {
            get { return _wrapper; }
            set { _wrapper = value; }
        }

        private bool _waitForExit = true;
        /// <summary>
        /// Wait until terminates script handling
        /// </summary>
        public bool waitForExit
        {
            get { return _waitForExit; }
            set { _waitForExit = value; }
        }
    }
}
