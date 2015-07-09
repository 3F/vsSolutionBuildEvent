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

namespace net.r_eg.vsSBE.UI.WForms.Components
{
    public class DataGridViewExt: DataGridView
    {
        /// <summary>
        /// After drag & drop sorting
        /// </summary>
        public event EventHandler<MovingRow> DragDropSortedRow = delegate(object sender, MovingRow index) { };

        /// <summary>
        /// The old / new index in sorted row
        /// </summary>
        public sealed class MovingRow: EventArgs
        {
            public int from;
            public int to;
        }

        /// <summary>
        /// Custom column: for work with numeric formats with standard TextBoxCell 
        /// </summary>
        public class NumericColumn: DataGridViewColumn
        {
            public bool Decimal { get; set; }
            public bool Negative { get; set; }

            public NumericColumn()
                : base(new DataGridViewTextBoxCell())
            {

            }
        }

        /// <summary>
        /// Shows total count of rows and current position for each row
        /// </summary>
        public bool NumberingForRowsHeader
        {
            get { return numberingForRowsHeader; }
            set { numberingForRowsHeader = value; }
        }
        protected bool numberingForRowsHeader = false;

        /// <summary>
        /// Allows sorting with Drag & Drop
        /// </summary>
        public bool DragDropSortable
        {
            get { return dragDropSortable; }
            set { 
                dragDropSortable = value;
                setupSortable(value);
            }
        }
        protected bool dragDropSortable = false;

        /// <summary>
        /// Always one row selected
        /// </summary>
        public bool AlwaysSelected
        {
            get { return alwaysSelected; }
            set { alwaysSelected = value; }
        }
        protected bool alwaysSelected = false;

        /// <summary>
        /// Support AlwaysSelected
        /// </summary>
        protected int lastSelectedRowIndex = 0;

        /// <summary>
        /// Support drag & drop for sortable rows
        /// </summary>
        protected MovingRow ddSort = new MovingRow();

        /// <summary>
        /// object synch.
        /// </summary>
        private Object _eLock = new Object();


        public DataGridViewExt()
        {
            this.CellPainting       += new DataGridViewCellPaintingEventHandler(onNumberingCellPainting);
            this.SelectionChanged   += new EventHandler(onAlwaysSelected);
            EditingControlShowing   += (object sender, DataGridViewEditingControlShowingEventArgs e) =>
                                        {
                                            if(e.Control == null) {
                                                return;
                                            }
                                            lock(_eLock) {
                                                e.Control.KeyPress -= onControlKeyPress;
                                                e.Control.KeyPress += onControlKeyPress;
                                            }
                                        };
        }

        protected void onControlKeyPress(object sender, KeyPressEventArgs e)
        {
            if(sender == null || sender.GetType() != typeof(DataGridViewTextBoxEditingControl)) {
                return;
            }
            DataGridView dgv = ((DataGridViewTextBoxEditingControl)sender).EditingControlDataGridView;

            if(dgv.CurrentCell.OwningColumn.GetType() != typeof(NumericColumn)) {
                return;
            }
            //(NumericColumn)dgv.CurrentCell.OwningColumn;

            if(!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar)) {
                e.Handled = true;
            }
        }

        protected void onNumberingCellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if(NumberingForRowsHeader) {
                numberingRowsHeader(e);
            }
        }

        protected void onSortableDragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        protected void onSortableDragDrop(object sender, DragEventArgs e)
        {
            Point point = this.PointToClient(new Point(e.X, e.Y));
            ddSort.to = this.HitTest(point.X, point.Y).RowIndex;

            if(e.Effect != DragDropEffects.Move || ddSort.to == -1 || this.Rows[ddSort.to].IsNewRow) {
                return;
            }
            e.Effect = DragDropEffects.None;

            this.Rows.RemoveAt(ddSort.from);
            this.Rows.Insert(ddSort.to, (DataGridViewRow)e.Data.GetData(typeof(DataGridViewRow)));
            this.ClearSelection();
            this.Rows[ddSort.to].Selected = true;

            DragDropSortedRow(this, ddSort);
        }

        protected void onSortableMouseMove(object sender, MouseEventArgs e)
        {
            if((e.Button & MouseButtons.Left) == MouseButtons.Left) {
                if(ddSort.from == -1 || this.Rows[ddSort.from].IsNewRow) {
                    return;
                }
                this.DoDragDrop(this.Rows[ddSort.from], DragDropEffects.Move);
            }
        }

        protected void onSortableMouseDown(object sender, MouseEventArgs e)
        {
            ddSort.from = this.HitTest(e.X, e.Y).RowIndex;
        }

        protected void setupSortable(bool enabled)
        {
            if(enabled) {
                this.AllowDrop = true;
                enableSortEvents();
                return;
            }
            disableSortEvents();
        }

        protected void enableSortEvents()
        {
            lock(_eLock) {
                disableSortEvents();
                this.DragOver   += new DragEventHandler(onSortableDragOver);
                this.DragDrop   += new DragEventHandler(onSortableDragDrop);
                this.MouseMove  += new MouseEventHandler(onSortableMouseMove);
                this.MouseDown  += new MouseEventHandler(onSortableMouseDown);
            }
        }

        protected void disableSortEvents()
        {
            lock(_eLock) {
                this.DragOver   -= new DragEventHandler(onSortableDragOver);
                this.DragDrop   -= new DragEventHandler(onSortableDragDrop);
                this.MouseMove  -= new MouseEventHandler(onSortableMouseMove);
                this.MouseDown  -= new MouseEventHandler(onSortableMouseDown);
            }
        }

        protected void onAlwaysSelected(object sender, EventArgs e)
        {
            if(!AlwaysSelected || Rows.Count < 1) {
                return;
            }

            if(SelectedRows.Count < 1) {
                lastSelectedRowIndex = Math.Max(0, Math.Min(lastSelectedRowIndex, Rows.Count - 1));
                Rows[lastSelectedRowIndex].Selected = true;
                return;
            }
            lastSelectedRowIndex = SelectedRows[0].Index;
        }

        protected virtual void numberingRowsHeader(DataGridViewCellPaintingEventArgs e)
        {
            if(e.ColumnIndex != -1) {
                return;
            }

            string str;
            if(e.RowIndex >= 0) {
                str = String.Format(" {0}", e.RowIndex + 1);
            }
            else {
                str = String.Format("({0})", this.Rows.Count);
            }

            e.PaintBackground(e.CellBounds, true);
            e.Graphics.DrawString(str, e.CellStyle.Font, new SolidBrush(Color.Black), e.CellBounds);
            e.Handled = true;
        }
    }
}
