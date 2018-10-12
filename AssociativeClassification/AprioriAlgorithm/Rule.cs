using System;
using System.Collections.Generic;

namespace AssociativeClassification.AprioriAlgorithm
{
    public class Rule : IComparable<Rule>
    {
        #region Member Variables

        string combination, remaining;
        double confidence, support;
        int _totalItemsInRule = -1;
        List<string> _itemsInRule;

        #endregion

        #region Constructor

        public Rule(string combination, string remaining, double confidence, double support)
        {
            this.combination = combination;
            this.remaining = remaining;
            this.confidence = confidence;
            this.support = support;
        }

        #endregion

        #region Public Properties

        public string X { get { return combination; } }

        public string Y { get { return remaining; } }

        public double Confidence { get { return confidence; } }
        public double Support { get { return support; } }
        
        public List<string> ItemsInRule
        {
            get
            {
                if (_itemsInRule != null && _itemsInRule.Count > 0) return _itemsInRule;

                BuildItemsInRule();

                return _itemsInRule;
            }
        }

        public int TotalItemsInRule
        {
            get
            {
                if (_totalItemsInRule > 0) return _totalItemsInRule;

                BuildItemsInRule();

                return _totalItemsInRule;
            }
        }

        #endregion

        #region IComparable<clssRules> Members

        public int CompareTo(Rule other)
        {
            return X.CompareTo(other.X);
        }

        #endregion

        public override string ToString()
        {
            return $"{X}  ->  {Y}. (sup: {Support}, conf = {Confidence:0.00})";
        }

        public override int GetHashCode()
        {
            Sorter sorter = new Sorter();
            string sortedXY = sorter.Sort(X + ";" + Y);
            return sortedXY.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as Rule;
            if (other == null)
            {
                return false;
            }

            return other.X == this.X && other.Y == this.Y ||
                other.X == this.Y && other.Y == this.X;
        }


        private void BuildItemsInRule()
        {
            var Xsplit = X.Split(';');
            var Ysplit = Y.Split(';');
            _itemsInRule = new List<string>(Xsplit.Length + Ysplit.Length);
            _itemsInRule.AddRange(Xsplit);
            _itemsInRule.AddRange(Ysplit);

            _totalItemsInRule = _itemsInRule.Count;
        }
    }
}