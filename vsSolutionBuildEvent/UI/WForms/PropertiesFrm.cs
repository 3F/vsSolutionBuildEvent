/*
 * Copyright (c) 2013-2016,2019-2020  Denis Kuzmin < x-3F@outlook.com > GitHub/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using net.r_eg.EvMSBuild;

namespace net.r_eg.vsSBE.UI.WForms
{
    public partial class PropertiesFrm: Form
    {
        /// <summary>
        /// Transport support
        /// </summary>
        private ITransfer _pin;

        /// <summary>
        /// Work with properties
        /// </summary>
        private IEvMSBuild _msbuild;

        /// <summary>
        /// 
        /// </summary>
        private IEnvironment _env;

        /// <summary>
        /// Caching of retrieved properties
        /// </summary>
        private ConcurrentDictionary<string, IEnumerable<PropertyItem>> _cacheProperties;

        public PropertiesFrm(IEnvironment env, ITransfer pin)
        {
            InitializeComponent();
            Icon = Resource.Package_32;

            _env                = env;
            _msbuild            = MSBuild.MakeEvaluator(_env);
            _pin                = pin;
            _cacheProperties    = new ConcurrentDictionary<string, IEnumerable<PropertyItem>>();
        }

        protected void fillProjects()
        {
            comboBoxProjects.Items.Clear();
            comboBoxProjects.Items.Add("<default>");

            try {
                comboBoxProjects.Items.AddRange(_env.ProjectsList.ToArray());
            }
            catch(Exception ex) {
                Log.Error($"Error when getting projects: {ex.Message}");
                Log.Debug(ex.StackTrace);
            }

            comboBoxProjects.SelectedIndex = 0;
        }

        protected void fillProperties(string project, string filterName = null, string filterValue = null)
        {
            dataGridViewVariables.Rows.Clear();
            try
            {
                foreach(PropertyItem prop in _getProperties(project))
                {
                    if(!String.IsNullOrEmpty(filterName) && !cmp(prop.name, filterName, mFilterRegexp.Checked)) {
                        continue;
                    }
                    if(!String.IsNullOrEmpty(filterValue) && !cmp(prop.value, filterValue, mFilterRegexp.Checked)) {
                        continue;
                    }
                    dataGridViewVariables.Rows.Add(prop.name, prop.value);
                }
            }
            catch(Exception ex) {
                Log.Error($"Error when getting properties from '{project}' project: {ex.Message}");
                Log.Debug(ex.StackTrace);
            }
        }

        protected virtual bool cmp(string data, string filter, bool regexp = false)
        {
            if(!regexp) {
                return data.ToLower().Contains(filter.ToLower());
            }

            try {
                return System.Text.RegularExpressions.Regex.IsMatch(data, filter);
            }
            catch(Exception) {
                return false;
            }
        }

        protected void listRender()
        {
            fillProperties(getSelectedProject(), textBoxFilter.Text.Trim(), textBoxFilterVal.Text.Trim());
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

        protected void keyDownToDGV(DataGridView grid, object sender, KeyEventArgs e)
        {
            if(grid == null || grid.CurrentCell == null) {
                e.SuppressKeyPress = (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down);
                return;
            }

            if(e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
            {
                int newpos          = grid.CurrentCell.RowIndex + (e.KeyCode == Keys.Up ? -1 : 1);
                grid.CurrentCell    = grid[0, Math.Max(0, Math.Min(newpos, grid.Rows.Count - 1))];
                e.SuppressKeyPress  = true;
                return;
            }
        }

        private IEnumerable<PropertyItem> _getProperties(string project)
        {
            string key = project;
            if(String.IsNullOrEmpty(key)) {
                key = "default";
            }

            if(!_cacheProperties.ContainsKey(key))
            {
                _cacheProperties[key] = _msbuild.ListProperties(project);
                Log.Debug("Properties has been saved in the cache. ['{0}']", key);
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

        private void _cellColor(Color backColor, Color foreColor, DataGridViewRow row)
        {
            row.DefaultCellStyle.SelectionBackColor = backColor;
            row.DefaultCellStyle.SelectionForeColor = foreColor;
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

        private void textBoxFilterVal_TextChanged(object sender, EventArgs e)
        {
            listRender();
        }

        private void textBoxFilterVal_KeyUp(object sender, KeyEventArgs e)
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

        private void dataGridViewVariables_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex < 0) {
                return;
            }
            _cellColor(Color.FromArgb(225, 241, 253), Color.FromArgb(23, 36, 47), dataGridViewVariables.Rows[e.RowIndex]);
        }

        private void dataGridViewVariables_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex < 0) {
                return;
            }
            _pin.property(dataGridViewVariables[0, e.RowIndex].Value.ToString(), getSelectedProject());
            _cellColor(Color.FromArgb(245, 242, 203), Color.FromArgb(23, 36, 47), dataGridViewVariables.Rows[e.RowIndex]);
        }

        private void menuItemExportList_Click(object sender, EventArgs e)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(String.Format("MSBuild Properties for project '{0}'\n", getSelectedProject()));
            sb.Append(String.Format("* Variables Filter: '{0}'\n", textBoxFilter.Text.Trim()));
            sb.Append(String.Format("* Values Filter: '{0}'\n", textBoxFilterVal.Text.Trim()));
            sb.Append(String.Format("* Regexp: {0}\n", mFilterRegexp.Checked));
            sb.Append(String.Format("* Found: {0}\n", dataGridViewVariables.Rows.Count));
            sb.Append(new String('_', 50) + "\n");
            sb.Append(new String('=', 50) + "\n\n\n");


            foreach(DataGridViewRow row in dataGridViewVariables.Rows) {
                sb.Append(String.Format("$({0}) = '{1}'\n{2}\n",
                                        row.Cells[colName.Name].Value,
                                        row.Cells[colValue.Name].Value,
                                        new String('_', 110)));
            }

            Clipboard.SetText(sb.ToString());
        }

        private void menuItemFilterRegexp_Click(object sender, EventArgs e)
        {
            mFilterRegexp.Checked       = !mFilterRegexp.Checked;
            textBoxFilter.BackColor     = (mFilterRegexp.Checked)? Color.FromArgb(232, 237, 247) : Color.White;
            textBoxFilterVal.BackColor  = textBoxFilter.BackColor;
            listRender();
        }

        private void textBoxFilter_KeyDown(object sender, KeyEventArgs e)
        {
            keyDownToDGV(dataGridViewVariables, sender, e);
        }

        private void textBoxFilterVal_KeyDown(object sender, KeyEventArgs e)
        {
            keyDownToDGV(dataGridViewVariables, sender, e);
        }
    }
}
