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

using System.Collections.Generic;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;

namespace net.r_eg.vsSBE.UI.WForms.Controls.TextEditorElements
{
    /// <summary>
    /// Foldings based on braces.
    /// </summary>
    public class BraceFoldingStrategy
    {
        public IEnumerable<NewFolding> CreateNewFoldings(ITextSource document)
        {
            List<NewFolding> ret = new List<NewFolding>();

            ret.AddRange(getOffsets('{', '}', document));
            ret.AddRange(getOffsets('[', ']', document)); // inc. SBE-Scripts #[ ... ]
            ret.AddRange(getOffsets('(', ')', document));

            ret.Sort((a, b) => a.StartOffset.CompareTo(b.StartOffset));
            return ret;
        }

        public IEnumerable<NewFolding> CreateNewFoldings(TextDocument document, out int firstErrorOffset)
        {
            firstErrorOffset = -1;
            return CreateNewFoldings(document);
        }

        public void UpdateFoldings(FoldingManager manager, TextDocument document)
        {
            int firstErrorOffset;
            IEnumerable<NewFolding> foldings = CreateNewFoldings(document, out firstErrorOffset);
            manager.UpdateFoldings(foldings, firstErrorOffset);
        }

        protected IEnumerable<NewFolding> getOffsets(char opening, char closing, ITextSource document)
        {
            List<NewFolding> ret    = new List<NewFolding>();
            Stack<int> openings     = new Stack<int>();
            bool multiline          = false; //flag of multiline braces

            for(int pos = 0; pos < document.TextLength; ++pos)
            {
                char c = document.GetCharAt(pos);

                if(c == opening) {
                    openings.Push(pos + 1);
                    multiline = false;
                }
                else if(char.IsControl(c)) {
                    multiline = true;
                }
                else if(openings.Count > 0 && c == closing)
                {
                    int offset = openings.Pop();
                    if(multiline) {
                        ret.Add(new NewFolding(offset, pos));
                    }
                }
            }
            return ret;
        }
    }
}
