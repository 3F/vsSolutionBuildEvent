/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
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