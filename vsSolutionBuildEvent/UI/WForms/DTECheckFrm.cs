/*
 * Copyright (c) 2013-2014 Developed by reg <entry.reg@gmail.com>
 * Distributed under the Boost Software License, Version 1.0
 * (See accompanying file LICENSE or copy at http://www.boost.org/LICENSE_1_0.txt)
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using net.r_eg.vsSBE.Actions;

namespace net.r_eg.vsSBE.UI
{
    public partial class DTECheckFrm: Form
    {
        /// <summary>
        /// Work with DTE-Commands
        /// </summary>
        private DTEOperation _dteo;
        /// <summary>
        /// Flag of sample
        /// </summary>
        private bool _isHiddenSample = false;

        public DTECheckFrm(Environment env)
        {
            _dteo = new DTEOperation((EnvDTE.DTE)env.DTE2, vsSBE.Events.SolutionEventType.General);
            InitializeComponent();
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            richTextBoxExecuted.Text = String.Empty;

            LogMessageEvent hlog = new LogMessageEvent(delegate(string msg) {
                richTextBoxExecuted.Text += msg;
            });
            Log.ReceiveMessage += hlog;

            try {
                _dteo.exec(richTextBoxCommand.Text.Split('\n'), false);
            }
            catch(Exception ex) {
                richTextBoxExecuted.Text += ex.Message;
            }
            Log.ReceiveMessage -= hlog;
        }

        private void richTextBoxCommand_Click(object sender, EventArgs e)
        {
            if(_isHiddenSample) {
                return;
            }
            _isHiddenSample = true;
            setCommand("", Color.FromArgb(0, 0, 0));
        }

        private void DTECheckFrm_Load(object sender, EventArgs e)
        {
            setCommand("Build.SolutionConfigurations(Debug)\nBuild.SolutionPlatforms(x86)", Color.FromArgb(128, 128, 128));
        }

        private void setCommand(string str, Color foreColor)
        {
            richTextBoxCommand.Text = str;
            richTextBoxCommand.ForeColor = foreColor;
        }
    }
}
