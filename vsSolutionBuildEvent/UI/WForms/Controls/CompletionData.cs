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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using net.r_eg.SobaScript.Mapper;

namespace net.r_eg.vsSBE.UI.WForms.Controls
{
    internal sealed class CompletionData: ICompletionData
    {
        public ImageSource Image
        {
            get;
            private set;
        }

        public string Text
        {
            get;
            private set;
        }

        public object Content
        {
            get;
            private set;
        }

        public object Description
        {
            get;
            private set;
        }

        /// <summary>
        /// Used in the selection logic to selecting those items
        /// which the user is accessing most frequently.
        /// </summary>
        public double Priority => 0;

        /// <summary>
        /// Perform the completion
        /// </summary>
        /// <param name="textArea">The text area on which completion is performed.</param>
        /// <param name="completionSegment">The text segment that was used by the completion window if
        /// the user types (segment between CompletionWindow.StartOffset and CompletionWindow.EndOffset).</param>
        /// <param name="insertionRequestEventArgs">The EventArgs used for the insertion request.
        /// These can be TextCompositionEventArgs, KeyEventArgs, MouseEventArgs, depending on how
        /// the insertion was triggered.</param>
        public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
        {
            int pos = completionSegment.Offset;
            for(int i = pos - 1; i >= 0; --i)
            {
                if(!char.IsLetterOrDigit(textArea.Document.Text[i])) {
                    pos = i + 1;
                    break;
                }
            }
            textArea.Document.Replace(pos, completionSegment.EndOffset - pos, Text);
        }

        public CompletionData(INodeInfo info, Bitmap img = null)
        {
            Text        = info.Name ?? throw new ArgumentNullException(nameof(info));
            Image       = img == null ? ImageFrom(info.Type) : ImageFrom(img);
            Content     = info.Overname;
            Description = info.Description;
        }




        ///// <param name="text">Used to filter the list of visible elements and inserting</param>
        ///// <param name="content">Text to display in the list</param>
        ///// <param name="description">Description for tooltip</param>
        ///// <param name="image">Image in the list</param>
        //public CompletionData(string text, string content, string description, ImageSource image = null)
        //{
        //    Text        = text;
        //    Description = description;
        //    Image       = image;
        //}

        ///// <param name="text">Used to filter the list of visible elements and inserting</param>
        ///// <param name="description">Description for tooltip</param>
        ///// <param name="image">Image in the list</param>
        //public CompletionData(string text, string description, ImageSource image = null)
        //    : this(text, null, description, image)
        //{

        //}

        ///// <param name="text">Used to filter the list of visible elements and inserting</param>
        ///// <param name="content">Text to display in the list</param>
        ///// <param name="description">Description for tooltip</param>
        ///// <param name="image">Image in the list</param>
        //public CompletionData(string text, string content, string description, Bitmap image)
        //    : this(text, description)
        //{
        //    Image = imageFrom(image);
        //}

        ///// <param name="text">Used to filter the list of visible elements and inserting</param>
        ///// <param name="description">Description for tooltip</param>
        ///// <param name="image">Image in the list</param>
        //public CompletionData(string text, string description, Bitmap image)
        //    : this(text, null, description, image)
        //{

        //}

        ///// <param name="text">Used to filter the list of visible elements and inserting</param>
        ///// <param name="content">Text to display in the list</param>
        ///// <param name="description">Description for tooltip</param>
        ///// <param name="type">Type of used element</param>
        //public CompletionData(string text, string content, string description, NodeType type)
        //    : this(text, description)
        //{
        //    Image           = imageFrom(type);
        //    this.content    = content;
        //}

        ///// <param name="text">Used to filter the list of visible elements and inserting</param>
        ///// <param name="description">Description for tooltip</param>
        ///// <param name="type">Type of used element</param>
        //public CompletionData(string text, string description, NodeType type)
        //    : this(text, null, description, type)
        //{

        //}

        private ImageSource ImageFrom(NodeType type)
        {
            switch(type)
            {
                case NodeType.Component: {
                    return ImageFrom(DomIcon.package);
                }
                case NodeType.Definition: {
                    return ImageFrom(DomIcon.definition);
                }
                case NodeType.Property: {
                    return ImageFrom(DomIcon.property);
                }
                case NodeType.Method: {
                    return ImageFrom(DomIcon.function);
                }
                case NodeType.AliasToDefinition:
                case NodeType.AliasToComponent: {
                    return ImageFrom(DomIcon.alias);
                }
            }

            return null;
        }

        private ImageSource ImageFrom(Bitmap bmap)
        {
            MemoryStream ms = new MemoryStream();
            bmap.Save(ms, ImageFormat.Png);

            BitmapImage img = new BitmapImage();
            img.BeginInit();
            img.StreamSource = ms;
            img.EndInit();

            //ms.Dispose();
            return img;
        }
    }
}
