/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.Collections.Generic;
using System.Linq;
using net.r_eg.SobaScript.Exceptions;
using net.r_eg.vsSBE.Exceptions;
using net.r_eg.vsSBE.Extensions;

namespace net.r_eg.vsSBE.Receiver.Output
{
    /// <summary>
    /// Provides available items from the output.
    /// </summary>
    public sealed class Items: IItems
    {
        /// <summary>
        /// Thread-safe getting instance from Items.
        /// </summary>
        public static Items _
        {
            get { return _lazy.Value; }
        }
        private static readonly Lazy<Items> _lazy = new Lazy<Items>(() => new Items());

        /// <summary>
        /// Items based on errors/warnings container.
        /// </summary>
        private Dictionary<Ident, IItemEW> ItemEW
        {
            get { return itemEW; }
        }
        private Dictionary<Ident, IItemEW> itemEW = new Dictionary<Ident, IItemEW>();

        /// <summary>
        /// Get EW item for identifier.
        /// </summary>
        /// <param name="ident">Identifier of item.</param>
        /// <returns>EW item.</returns>
        public IItemEW getEW(Ident ident)
        {
            return (IItemEW)get(ItemType.EW, ident);
        }

        /// <summary>
        /// Get item for type and identifier.
        /// </summary>
        /// <param name="type">Type of item.</param>
        /// <param name="ident">Identifier of item.</param>
        /// <returns>Unspecified common item.</returns>
        public IItem get(ItemType type, Ident ident)
        {
            bool isFull = (ident.guid != null && ident.item != null);
            switch(type)
            {
                case ItemType.EW:
                {
                    // check with part from identifier
                    IItem item = itemEW.FirstOrDefault(i => (isFull && ident.guid.CompareGuids(i.Key.guid) && ident.item == i.Key.item)
                                                                || (!isFull && ident.guid != null && ident.guid.CompareGuids(i.Key.guid))
                                                                || (!isFull && ident.item != null && ident.item == i.Key.item)
                                                      ).Value;

                    if(item == null && isFull) {
                        itemEW[ident] = new ItemEW();
                        return itemEW[ident];
                    }
                    else if(item == null && ident.item == Settings._.DefaultOWPItem) { //TODO:
                        return new ItemEW();
                    }
                    else if(item == null) {
                        throw new NotFoundException(ident.item, $"OWP Items-EW: The '{ident.guid}:{ident.item}' is not found.", ident.guid);
                    }
                    return item;
                }
            }
            throw new NotFoundException(type, $"OWP Items: Type '{type}' is not supported.");
        }

        private Items() { }
    }
}
