/*
 * Copyright (c) 2013 Developed by reg <entry.reg@gmail.com>
 * Distributed under the Boost Software License, Version 1.0
 * (See accompanying file LICENSE or copy at http://www.boost.org/LICENSE_1_0.txt)
 */

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace net.r_eg.vsSBE.UI
{
    public partial class PropertiesFrm: Form
    {
        /// <summary>
        /// Transport support
        /// </summary>
        private ITransferDataProperty _pin;

        /// <summary>
        /// Work with properties
        /// </summary>
        private MSBuildParser _msbuild;

        /// <summary>
        /// 
        /// </summary>
        private Environment _env;

        /// <summary>
        /// Caching of retrieved properties
        /// </summary>
        private ConcurrentDictionary<string, List<TMSBuildPropertyItem>> _cacheProperties;

        public PropertiesFrm(ITransferDataProperty pin)
        {
            InitializeComponent();

            _env                = new Environment(vsSolutionBuildEventPackage.Dte2);
            _msbuild            = new MSBuildParser(_env);
            this._pin           = pin;
            _cacheProperties    = new ConcurrentDictionary<string, List<TMSBuildPropertyItem>>();
        }

        protected void fillProjects()
        {
            comboBoxProjects.Items.Clear();
            comboBoxProjects.Items.Add("<default>");

            try {
                comboBoxProjects.Items.AddRange(_env.DTEProjectsList.ToArray());
            }
            catch(Exception ex) {
                Log.nlog.Error("Error with getting projects: " + ex.Message);
            }

            comboBoxProjects.SelectedIndex = 0;
        }

        protected void fillProperties(string project, string filter = null)
        {
            dataGridViewVariables.Rows.Clear();
            try
            {
                foreach(TMSBuildPropertyItem prop in _getProperties(project)) {
                    if(filter != null && !prop.name.ToLower().Contains(filter)) {
                        continue;
                    }
                    dataGridViewVariables.Rows.Add(prop.name, prop.value);
                }
            }
            catch(Exception ex) {
                Log.nlog.Error("Error with getting properties for '{0}': {1}", project, ex.Message);
            }
        }

        protected void listRender()
        {
            fillProperties(getSelectedProject(), textBoxFilter.Text.Trim().ToLower());
            labelPropCount.Text = dataGridViewVariables.Rows.Count.ToString();
        }

        protected void keyUp(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Escape) {
                this.Dispose();
                return;
            }

            if(e.KeyCode != Keys.Enter) {
                return;
            }

            foreach(DataGridViewRow row in dataGridViewVariables.Rows) {
                if(row.Selected) {
                    _pin.property(row.Cells[0].Value.ToString(), getSelectedProject());
                    this.Dispose();
                    return;
                }
            }
        }

        /// <exception cref="MSBuildParserProjectNotFoundException">if not found the specific project</exception>
        private List<TMSBuildPropertyItem> _getProperties(string project)
        {
            string key = project;
            if(String.IsNullOrEmpty(key)) {
                key = "default";
            }

            if(!_cacheProperties.ContainsKey(key))
            {
                _cacheProperties[key] = _msbuild.listProperties(project);
                Log.nlog.Debug("Properties has been saved in the cache. ['{0}']", key);
            }
            return _cacheProperties[key];
        }

        private string getSelectedProject()
        {
            if(comboBoxProjects.SelectedIndex > 0) {
                return comboBoxProjects.Text;
            }
            return null;
        }

        private void EnvironmentVariablesFrm_Load(object sender, EventArgs e)
        {
            _cacheProperties.Clear();
            fillProjects();            
            textBoxFilter.Select();
        }

        private void comboBoxProjects_SelectedIndexChanged(object sender, EventArgs e)
        {
            listRender();
        }

        private void textBoxFilter_TextChanged(object sender, EventArgs e)
        {
            listRender();
        }

        private void textBoxFilter_KeyUp(object sender, KeyEventArgs e)
        {
            keyUp(sender, e);
        }

        private void dataGridViewVariables_KeyUp(object sender, KeyEventArgs e)
        {
            keyUp(sender, e);
        }

        private void dataGridViewVariables_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter) {
                e.SuppressKeyPress = true;
                return;
            }
        }

        private void comboBoxProjects_KeyUp(object sender, KeyEventArgs e)
        {
            keyUp(sender, e);
        }

        private void EnvironmentVariablesFrm_KeyUp(object sender, KeyEventArgs e)
        {
            keyUp(sender, e);
        }

        private void dataGridViewVariables_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex >= 0) {
                _pin.property(dataGridViewVariables[0, e.RowIndex].Value.ToString(), getSelectedProject());
                this.Dispose();
            }
        }
    }
}
