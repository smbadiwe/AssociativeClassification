using System;

namespace AssociativeClassification.AprioriAlgorithm
{
    public class Item : IComparable<Item>
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the item (stock) identifier(s) - ;-delimited.
        /// </summary>
        /// <value>
        /// The item identifier(s).
        /// </value>
        public string ItemIDs { get; set; }
        public double Support { get; set; }

        #endregion

        #region IComparable

        public int CompareTo(Item other)
        {
            return ItemIDs.CompareTo(other.ItemIDs);
        }

        #endregion

        public override string ToString()
        {
            return $"{ItemIDs}. [Sup: {Support}]";
        }
    }
}
