/*
 * Copyright (c) 2013-2014  Denis Kuzmin (reg) <entry.reg@gmail.com>
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
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

        public DTECheckFrm(IEnvironment env)
        {
            _dteo = new DTEOperation(env, vsSBE.Events.SolutionEventType.General);
            InitializeComponent();
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            richTextBoxExecuted.Text = String.Empty;

            Log.MessageEvent hlog = new Log.MessageEvent(delegate(string msg) {
                richTextBoxExecuted.Text += msg;
            });
            Log.Message += hlog;

            try {
                _dteo.exec(richTextBoxCommand.Text.Split('\n'), false);
            }
            catch(Exception ex) {
                richTextBoxExecuted.Text += ex.Message;
            }
            Log.Message -= hlog;
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
            Util.openUrl("https://bitbucket.org/3F/vssolutionbuildevent/wiki/Scripts_&_Commands/DTE-Commands");
        }
    }
}
