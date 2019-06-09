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

namespace net.r_eg.vsSBE.SBEScripts.Dom
{
    public class CompletionData: ICompletionData
    {
        /// <summary>
        /// Image in the list
        /// </summary>
        public ImageSource Image
        {
            get;
            protected set;
        }

        /// <summary>
        /// Used to filter the list of visible elements and inserting
        /// </summary>
        public string Text
        {
            get;
            protected set;
        }

        /// <summary>
        /// Text to display in the list
        /// </summary>
        public object Content
        {
            get { return (content)?? Text; }
        }
        protected object content;

        /// <summary>
        /// Description for tooltip
        /// </summary>
        public object Description
        {
            get;
            protected set;
        }

        /// <summary>
        /// Used in the selection logic to selecting those items
        /// which the user is accessing most frequently.
        /// </summary>
        public double Priority
        {
            get { return 0; }
        }

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
            for(int i = pos - 1; i >= 0; --i) {
                if(!char.IsLetterOrDigit(textArea.Document.Text[i])) {
                    pos = i + 1;
                    break;
                }
            }
            textArea.Document.Replace(pos, completionSegment.EndOffset - pos, Text);
        }

        /// <param name="text">Used to filter the list of visible elements and inserting</param>
        /// <param name="content">Text to display in the list</param>
        /// <param name="description">Description for tooltip</param>
        /// <param name="image">Image in the list</param>
        public CompletionData(string text, string content, string description, ImageSource image = null)
        {
            Text        = text;
            Description = description;
            Image       = image;
        }

        /// <param name="text">Used to filter the list of visible elements and inserting</param>
        /// <param name="description">Description for tooltip</param>
        /// <param name="image">Image in the list</param>
        public CompletionData(string text, string description, ImageSource image = null)
            : this(text, null, description, image)
        {

        }

        /// <param name="text">Used to filter the list of visible elements and inserting</param>
        /// <param name="content">Text to display in the list</param>
        /// <param name="description">Description for tooltip</param>
        /// <param name="image">Image in the list</param>
        public CompletionData(string text, string content, string description, Bitmap image)
            : this(text, description)
        {
            Image = imageFrom(image);
        }

        /// <param name="text">Used to filter the list of visible elements and inserting</param>
        /// <param name="description">Description for tooltip</param>
        /// <param name="image">Image in the list</param>
        public CompletionData(string text, string description, Bitmap image)
            : this(text, null, description, image)
        {

        }

        /// <param name="text">Used to filter the list of visible elements and inserting</param>
        /// <param name="content">Text to display in the list</param>
        /// <param name="description">Description for tooltip</param>
        /// <param name="type">Type of used element</param>
        public CompletionData(string text, string content, string description, InfoType type)
            : this(text, description)
        {
            Image           = imageFrom(type);
            this.content    = content;
        }

        /// <param name="text">Used to filter the list of visible elements and inserting</param>
        /// <param name="description">Description for tooltip</param>
        /// <param name="type">Type of used element</param>
        public CompletionData(string text, string description, InfoType type)
            : this(text, null, description, type)
        {

        }

        protected ImageSource imageFrom(InfoType type)
        {
            switch(type) {
                case InfoType.Component: {
                    return imageFrom(Icon.package);
                }
                case InfoType.Definition: {
                    return imageFrom(Icon.definition);
                }
                case InfoType.Property: {
                    return imageFrom(Icon.property);
                }
                case InfoType.Method: {
                    return imageFrom(Icon.function);
                }
                case InfoType.AliasToDefinition:
                case InfoType.AliasToComponent: {
                    return imageFrom(Icon.alias);
                }
            }
            return null;
        }

        protected ImageSource imageFrom(Bitmap bmap)
        {
            MemoryStream ms = new MemoryStream();
            bmap.Save(ms, ImageFormat.Png);

            BitmapImage img = new BitmapImage();
            img.BeginInit();
            img.StreamSource = ms;
            img.EndInit();

            return img;
        }
    }
}
