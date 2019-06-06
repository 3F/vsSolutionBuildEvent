/*
 * Copyright (c) 2013-2016,2019  Denis Kuzmin < entry.reg@gmail.com > GitHub/3F
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
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using AvalonEditorWPF = ICSharpCode.AvalonEdit.TextEditor;

namespace net.r_eg.vsSBE.UI.WForms.Controls.TextEditorElements
{
    /// <summary>
    /// Highlighting braces.
    /// </summary>
    public class HighlightBracesRenderer: IBackgroundRenderer
    {
        /// <summary>
        /// Describes how the area is painted.
        /// </summary>
        public Brush BrushStyle
        {
            get; 
            set;
        }

        /// <summary>
        /// Describes how a shape is outlined.
        /// </summary>
        public Pen PenStyle
        {
            get;
            set;
        }
        
        /// <summary>
        /// Gets the layer on which this background renderer should draw.
        /// </summary>
        public KnownLayer Layer
        {
            get {
                return KnownLayer.Selection;
            }
        }

        /// <summary>
        /// Reference to editor.
        /// </summary>
        protected AvalonEditorWPF editor;

        /// <summary>
        /// Causes the background renderer to draw.
        /// </summary>
        /// <param name="textView"></param>
        /// <param name="drawingContext"></param>
        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            if(editor.Document.TextLength < 2) {
                return;
            }
            int offset = alignOffset(editor.TextArea.Caret.Offset);

            if(highlightOffset('{', '}', offset, textView, drawingContext)) {
                return;
            }
            if(highlightOffset('[', ']', offset, textView, drawingContext)) {
                return;
            }
            highlightOffset('(', ')', offset, textView, drawingContext);
        }

        /// <param name="cL">Left bracket.</param>
        /// <param name="cR">Right bracket.</param>
        /// <param name="offset">Current offset of caret.</param>
        /// <param name="textView"></param>
        /// <param name="drawingContext"></param>
        /// <returns></returns>
        protected bool highlightOffset(char cL, char cR, int offset, TextView textView, DrawingContext drawingContext)
        {
            if(highlightOffset(true, cL, cR, offset, textView, drawingContext)) {
                return true;
            }
            return highlightOffset(false, cR, cL, offset, textView, drawingContext);
        }

        /// <param name="fromOpening">Movement type to find the pair.</param>
        /// <param name="cL">Left bracket.</param>
        /// <param name="cR">Right bracket.</param>
        /// <param name="offset">Current offset of caret.</param>
        /// <param name="textView"></param>
        /// <param name="drawingContext"></param>
        /// <returns></returns>
        protected bool highlightOffset(bool fromOpening, char cL, char cR, int offset, TextView textView, DrawingContext drawingContext)
        {
            int opening = checkOffset(cL, offset);
            if(opening == -1) {
                return false;
            }
            IEnumerable<int> iter = (fromOpening)? IterateFromOpening(opening) : IterateFromClosing(opening);
            
            int closing = -1;
            int count   = 0;
            foreach(int pos in iter)
            {
                char c = editor.Document.GetCharAt(pos);

                if(c == cL) {
                    ++count;
                }
                else if(c == cR)
                {
                    --count;
                    if(count == 0) {
                        closing = pos;
                        break;
                    }
                }
            }

            if(closing == -1) {
                return false;
            }

            highlight(new TextSegment() { StartOffset = opening, Length = 1 }, textView, drawingContext);
            highlight(new TextSegment() { StartOffset = closing, Length = 1 }, textView, drawingContext);
            return true;
        }

        /// <summary>
        /// Defines style of selection brace.
        /// </summary>
        /// <param name="brace"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        protected virtual int checkOffset(char brace, int offset)
        {
            // ->{
            if(checkBrace(brace, offset)) {
                return offset;
            }

            // {<-
            if(checkBrace(brace, --offset)) {
                return offset;
            }
            return -1;
        }

        /// <summary>
        /// Defines style of highlighting.
        /// </summary>
        /// <param name="segment"></param>
        /// <param name="textView"></param>
        /// <param name="drawingContext"></param>
        protected virtual void highlight(ISegment segment, TextView textView, DrawingContext drawingContext)
        {
            BackgroundGeometryBuilder geoBuilder = new BackgroundGeometryBuilder() {
                CornerRadius = 0
            };
            geoBuilder.AddSegment(textView, segment);

            Geometry geometry = geoBuilder.CreateGeometry();
            drawingContext.DrawGeometry(BrushStyle, PenStyle, geometry);
        }

        public HighlightBracesRenderer(AvalonEditorWPF editor)
        {
            this.editor = editor;
            BrushStyle  = brushColorFromString("#EAEDE0");
            PenStyle    = new Pen(brushColorFromString("#314463"), 0.2f);
        }

        protected Brush brushColorFromString(string color)
        {
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
        }

        protected int alignOffset(int offset)
        {
            return Math.Max(0, Math.Min(offset, editor.Document.TextLength - 1));
        }

        private bool checkBrace(char brace, int offset)
        {
            if(offset < 0) {
                return false;
            }
            return (brace == editor.Document.GetCharAt(offset));
        }

        private IEnumerable<int> IterateFromOpening(int start)
        {
            for(int pos = start; pos < editor.Document.TextLength; ++pos) {
                yield return pos;
            }
        }

        private IEnumerable<int> IterateFromClosing(int start)
        {
            for(int pos = start; pos > 0; --pos) {
                yield return pos;
            }
        }
    }
}
