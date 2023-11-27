/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.Drawing;
using System.Windows.Forms;
using net.r_eg.EvMSBuild;

namespace net.r_eg.vsSBE.UI.WForms
{
    public partial class PropertyCheckFrm: Form
    {
        /// <summary>
        /// Work with MSBuild
        /// </summary>
        private IEvMSBuild _parser;

        /// <summary>
        /// Flag of sample
        /// </summary>
        private bool _isHiddenSample = false;

        public PropertyCheckFrm(IEnvironment env)
        {
            _parser = MSBuild.MakeEvaluator(env);

            InitializeComponent();
            Icon = Resource.Package_32;
        }

        private void btnEvaluate_Click(object sender, EventArgs e)
        {
            string evaluated;
            try {
                // for a specific project use like this: $($(var):project)
                evaluated = _parser.Eval(textBoxUnevaluated.Text.Trim());
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
            Util.openUrl("https://3F.github.io/web.vsSBE/doc/Scripts/MSBuild/");
        }
    }
}
