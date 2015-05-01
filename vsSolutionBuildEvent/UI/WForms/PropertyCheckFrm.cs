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
using net.r_eg.vsSBE.MSBuild;

namespace net.r_eg.vsSBE.UI.WForms
{
    public partial class PropertyCheckFrm: Form
    {
        /// <summary>
        /// Work with MSBuild
        /// </summary>
        private MSBuild.Parser _parser;
        /// <summary>
        /// Flag of sample
        /// </summary>
        private bool _isHiddenSample = false;

        public PropertyCheckFrm(IEnvironment env)
        {
            _parser = new MSBuild.Parser(env);
            InitializeComponent();
        }

        private void btnEvaluate_Click(object sender, EventArgs e)
        {
            string evaluated;
            try {
                // for a specific project use like this: $($(var):project)
                evaluated = _parser.parse(textBoxUnevaluated.Text.Trim());
            }
            catch(Exception ex) {
                evaluated = String.Format("Fail: {0}", ex.Message);
            }
            richTextBoxEvaluated.Text = evaluated;
        }

        private void textBoxUnevaluated_Click(object sender, EventArgs e)
        {
            if(_isHiddenSample) {
                return;
            }
            _isHiddenSample = true;
            setUnevaluated("", Color.FromArgb(0, 0, 0));
        }

        private void PropertyCheckFrm_Load(object sender, EventArgs e)
        {
            setUnevaluated("$([System.Guid]::NewGuid())", Color.FromArgb(128, 128, 128));
        }

        private void setUnevaluated(string str, Color foreColor)
        {
            textBoxUnevaluated.Text         = str;
            textBoxUnevaluated.ForeColor    = foreColor;
        }

        private void btnDoc_Click(object sender, EventArgs e)
        {
            Util.openUrl("https://bitbucket.org/3F/vssolutionbuildevent/wiki/Scripts_&_Commands/MSBuild");
        }
    }
}
