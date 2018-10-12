using System;

namespace AssociativeClassification.AprioriAlgorithm
{
    public class Sorter
    {
        public string Sort(string token)
        {
            string[] tokenArray = token.Split(';');
            if (tokenArray.Length == 1) return token;

            Array.Sort(tokenArray);
            return string.Join(";", tokenArray);
        }
    }
}
