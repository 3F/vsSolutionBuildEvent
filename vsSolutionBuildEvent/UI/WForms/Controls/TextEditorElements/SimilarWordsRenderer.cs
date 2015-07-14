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
using System.Linq;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Search;
using AvalonEditorWPF = ICSharpCode.AvalonEdit.TextEditor;

namespace net.r_eg.vsSBE.UI.WForms.Controls.TextEditorElements
{
    public class SimilarWordsRenderer: IBackgroundRenderer
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
                return KnownLayer.Background;
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
            if(String.IsNullOrWhiteSpace(editor.SelectedText) 
                || !editor.SelectedText.All(Char.IsLetterOrDigit))
            {
                return;
            }
            ISearchStrategy strategy = SearchStrategyFactory.Create(editor.SelectedText, false, true, SearchMode.Normal);
            
            foreach(ISearchResult result in strategy.FindAll(textView.Document, 0, textView.Document.TextLength))
            {
                BackgroundGeometryBuilder builder = new BackgroundGeometryBuilder() {
                    CornerRadius = 1
                };
                builder.AddSegment(textView, result);
                drawingContext.DrawGeometry(BrushStyle, PenStyle, builder.CreateGeometry());
            }
        }

        public SimilarWordsRenderer(AvalonEditorWPF editor)
        {
            this.editor = editor;
            BrushStyle  = brushColorFromString("#DDE8CA");
            PenStyle    = new Pen(BrushStyle, 1);
        }

        protected Brush brushColorFromString(string color)
        {
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
        }
    }
}