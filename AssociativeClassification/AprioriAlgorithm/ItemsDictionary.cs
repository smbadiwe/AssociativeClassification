using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AssociativeClassification.AprioriAlgorithm
{
    public class ItemsDictionary : KeyedCollection<string, Item>
    {
        protected override string GetKeyForItem(Item item)
        {
            return item.ItemIDs;
        }

        internal void ConcatItems(IList<Item> frequentItems)
        {
            foreach (var item in frequentItems)
            {
                this.Add(item);
            }
        }
    }
}