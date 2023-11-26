/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.Drawing;
using System.Windows.Forms;

namespace net.r_eg.vsSBE.UI.WForms.Components
{
    public class DataGridViewExt: DataGridView
    {
        /// <summary>
        /// After drag 'n' drop sorting
        /// </summary>
        public event EventHandler<MovingRowArgs> DragDropSortedRow = delegate(object sender, MovingRowArgs e) { };

        /// <summary>
        /// Alias for amazing designer -_-
        /// </summary>
        public sealed class MovingRowArgs: DataArgs<MovingRow> { }

        /// <summary>
        /// The old / new index in sorted row
        /// </summary>
        public struct MovingRow
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
        /// Allows sorting with Drag 'n' Drop
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
        /// Support drag 'n' drop for sortable rows
        /// </summary>
        protected MovingRow ddSort = new MovingRow();

        /// <summary>
        /// object synch.
        /// </summary>
        private Object _eLock = new Object();


        public DataGridViewExt()
        {
            this.CellPainting       += onNumberingCellPainting;
            this.SelectionChanged   += onAlwaysSelected;
            EditingControlShowing   += (object sender, DataGridViewEditingControlShowingEventArgs e) =>
                                        {
                                            if(e.Control == null) {
                                                return;
                                            }
                                            lock(_eLock) {
                                                e.Control.KeyPress -= onControlKeyPress;
                                                e.Control.KeyPress += onControlKeyPress;
                                                e.Control.PreviewKeyDown -= onControlPreviewKeyDown;
                                                e.Control.PreviewKeyDown += onControlPreviewKeyDown;
                                                
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

        protected void onSortableDragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        protected void onSortableDragDrop(object sender, DragEventArgs e)
        {
            Point point = this.PointToClient(new Point(e.X, e.Y));
            ddSort.to = this.HitTest(point.X, point.Y).RowIndex;

            if(e.Effect != DragDropEffects.Move || ddSort.to == -1 || Rows.Count < 1 || Rows[ddSort.to].IsNewRow) {
                return;
            }
            e.Effect = DragDropEffects.None;

            this.Rows.RemoveAt(ddSort.from);
            this.Rows.Insert(ddSort.to, (DataGridViewRow)e.Data.GetData(typeof(DataGridViewRow)));
            this.ClearSelection();
            this.Rows[ddSort.to].Selected = true;

            DragDropSortedRow(this, new MovingRowArgs() { Data = ddSort });
        }

        protected void onSortableMouseMove(object sender, MouseEventArgs e)
        {
            if((e.Button & MouseButtons.Left) == MouseButtons.Left) {
                if(ddSort.from == -1 || Rows.Count < 1 || Rows[ddSort.from].IsNewRow) {
                    return;
                }
                DoDragDrop(Rows[ddSort.from], DragDropEffects.Move);
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
            lock(_eLock)
            {
                disableSortEvents();
                this.DragOver   += onSortableDragOver;
                this.DragDrop   += onSortableDragDrop;
                this.MouseMove  += onSortableMouseMove;
                this.MouseDown  += onSortableMouseDown;
            }
        }

        protected void disableSortEvents()
        {
            lock(_eLock)
            {
                this.DragOver   -= onSortableDragOver;
                this.DragDrop   -= onSortableDragDrop;
                this.MouseMove  -= onSortableMouseMove;
                this.MouseDown  -= onSortableMouseDown;
            }
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

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if(keyData == Keys.Enter) {
                EndEdit();
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }

        private void onNumberingCellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if(NumberingForRowsHeader) {
                numberingRowsHeader(e);
            }
        }

        private void onAlwaysSelected(object sender, EventArgs e)
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

        /// <summary>
        /// A trick with left/right keys in EditMode of text columns.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onControlPreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if(sender == null || sender.GetType() != typeof(DataGridViewTextBoxEditingControl)) {
                return;
            }
            var box = (DataGridViewTextBoxEditingControl)sender;
            int pos = box.SelectionStart;

            if(box.Text.Length < 1) {
                return;
            }

            if(pos == 0 && e.KeyData == Keys.Left) {
                BeginEdit(false);
                box.SelectionStart = Math.Min(1, box.Text.Length); // will decrease with std handler
                return;
            }

            if(pos == box.Text.Length && e.KeyData == Keys.Right) {
                BeginEdit(false);
                box.SelectionStart = box.Text.Length - 1; // also will with std handler later
                return;
            }
        }
    }
}
