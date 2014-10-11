/*
 * Copyright (c) 2013-2014 Developed by reg <entry.reg@gmail.com>
 * Distributed under the Boost Software License, Version 1.0
 * (See accompanying file LICENSE or copy at http://www.boost.org/LICENSE_1_0.txt)
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace net.r_eg.vsSBE.UI.WForms
{
    /// <summary>
    /// hooking up notifications for moved rows
    /// </summary>
    public delegate void DGVSortEventHandler(MovingRow index);

    /// <summary>
    /// The old / new index in sorted row
    /// </summary>
    public struct MovingRow
    {
        public int from;
        public int to;
    }

    public class DataGridViewExt: DataGridView
    {
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
        /// Call after drag & drop sorting
        /// </summary>
        public event DGVSortEventHandler DragDropSortedRow = delegate(MovingRow index) { };

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
        protected MovingRow ddSort;

        /// <summary>
        /// object synch.
        /// </summary>
        private Object _eLock = new Object();

        public DataGridViewExt()
        {
            this.CellPainting       += new DataGridViewCellPaintingEventHandler(onNumberingCellPainting);
            this.SelectionChanged   += new EventHandler(onAlwaysSelected);
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

            DragDropSortedRow(ddSort);
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
