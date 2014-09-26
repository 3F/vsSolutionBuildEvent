using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EnvDTE80;

namespace net.r_eg.vsSBE.UI
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
            grid.SuspendLayout();
            grid.Rows.Clear();
            foreach(EnvDTE.Command cmd in _commands) {
                if(cmd.Name.Length < 1 || (filter != null && !cmd.Name.ToLower().Contains(filter))) {
                    continue;
                }
                grid.Rows.Add(cmd.Name);
            }
            grid.ResumeLayout();
            labelPropCount.Text = grid.Rows.Count.ToString();
        }

        private void dataGridViewDTE_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            _pin.command(String.Format("{0}{1}", dataGridViewDTE[0, e.RowIndex].Value, System.Environment.NewLine));

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
