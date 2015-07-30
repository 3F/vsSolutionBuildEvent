using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace net.r_eg.vsSBE.UI.WForms
{
    public partial class DTECommandsFrm: Form
    {
        /// <summary>
        /// Transport support
        /// </summary>
        private ITransferDataCommand _pin;

        IEnumerable<EnvDTE.Command> _commands;

        public DTECommandsFrm(IEnumerable<EnvDTE.Command> commands, ITransferDataCommand pin)
        {
            _commands = commands;
            this._pin = pin;

            InitializeComponent();
            Icon = Resource.Package_32;
        }

        private void DTECommandsFrm_Load(object sender, EventArgs e)
        {
            fill(dataGridViewDTE);
        }

        private void textBoxFilter_TextChanged(object sender, EventArgs e)
        {
            fill(dataGridViewDTE, textBoxFilter.Text);
        }

        private void fill(DataGridView grid, string filter = null)
        {
            if(!String.IsNullOrWhiteSpace(filter)) {
                filter = filter.ToLower();
            }

            grid.SuspendLayout();
            grid.Rows.Clear();
            foreach(EnvDTE.Command cmd in _commands)
            {
                if(String.IsNullOrWhiteSpace(cmd.Name) || (filter != null && !cmd.Name.ToLower().Contains(filter))) {
                    continue;
                }
                grid.Rows.Add(cmd.Name);
            }
            grid.ResumeLayout();
            labelPropCount.Text = grid.Rows.Count.ToString();
        }

        private void dataGridViewDTE_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            _pin.command(String.Format("{0}", dataGridViewDTE[0, e.RowIndex].Value));

            dataGridViewDTE.Rows[e.RowIndex].DefaultCellStyle.SelectionBackColor = Color.FromArgb(245, 242, 203);
            dataGridViewDTE.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor = Color.FromArgb(23, 36, 47);
        }

        private void dataGridViewDTE_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridViewDTE.Rows[e.RowIndex].DefaultCellStyle.SelectionBackColor = Color.FromArgb(248, 243, 243);
            dataGridViewDTE.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor = Color.FromArgb(23, 36, 47);
        }

        private void dataGridViewDTE_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter) {
                e.SuppressKeyPress = true;
                return;
            }
        }
    }
}
