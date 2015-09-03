/*
 * Copyright (c) 2013-2015  Denis Kuzmin (reg) <entry.reg@gmail.com>
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
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
                Log._.Receiving -= hlog;
                Log._.Receiving += hlog;

                try {
                    _dteo.exec(richTextBoxCommand.Text.Split('\n'), false);
                }
                catch(Exception ex) {
                    richTextBoxExecuted.Text += ex.Message;
                }
                Log._.Receiving -= hlog;
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
            Util.openUrl("http://vssbe.r-eg.net/doc/Scripts/DTE-Commands/");
        }
    }
}
