/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.Drawing;
using System.Windows.Forms;
using net.r_eg.vsSBE.Actions;

namespace net.r_eg.vsSBE.UI.WForms
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

        /// <summary>
        /// obj synch.
        /// </summary>
        private Object _lock = new Object();

        public DTECheckFrm(IEnvironment env)
        {
            _dteo = new DTEOperation(env, vsSBE.Events.SolutionEventType.General);

            InitializeComponent();
            Icon = Resource.Package_32;
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            richTextBoxExecuted.Text = String.Empty;

            var hlog = new EventHandler<Logger.MessageArgs>(delegate(object _sender, Logger.MessageArgs _e) {
                richTextBoxExecuted.Text += _e.Message;
            });

            lock(_lock)
            {
                Log._.Received -= hlog;
                Log._.Received += hlog;

                try {
                    _dteo.exec(richTextBoxCommand.Text.Split('\n'), false);
                }
                catch(Exception ex) {
                    richTextBoxExecuted.Text += ex.Message;
                }
                Log._.Received -= hlog;
            }
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

        private void btnDoc_Click(object sender, EventArgs e)
        {
            Util.openUrl("https://3F.github.io/web.vsSBE/doc/Scripts/DTE-Commands/");
        }
    }
}
