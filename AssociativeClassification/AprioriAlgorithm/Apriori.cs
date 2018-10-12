using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssociativeClassification.AprioriAlgorithm
{
    // See detailed description at https://www.codeproject.com/Articles/70371/Apriori-Algorithm
    public class Apriori
    {
        #region Member Variables

        readonly Sorter _sorter;

        #endregion

        #region Constructor

        public Apriori()
        {
            _sorter = new Sorter();
        }

        #endregion

        #region Apriori

        public Output ProcessTransaction(double minSupport, double minConfidence, IEnumerable<string> items, List<Order> transactions)
        {
            //Console.WriteLine("=======================\nPROCESSING FOR CLASS: Valued\n====================\n");
            //ProcessTransaction(minSupport, minConfidence, items, 
            //    transactions.Where(x => x.Class).Select(x => x.Itemset), true);
            //Console.WriteLine("\n=======================\nPROCESSING FOR CLASS: Non-Valued\n====================\n");
            minSupport = 0.1;
            ProcessTransaction(minSupport, minConfidence, items,
                transactions.Where(x => !x.Class).Select(x => x.Itemset), false);
            Console.WriteLine("\n=================END===================\n");

            return null;
        }

        public Output ProcessTransaction(double minSupport, double minConfidence, IEnumerable<string> items, IEnumerable<string> transactions, bool @class)
        {
            Print(minSupport, minConfidence);
            IList<Item> frequentItems = GetL1FrequentItems(minSupport, items, transactions);
            Print(frequentItems);
            ItemsDictionary allFrequentItems = new ItemsDictionary();
            allFrequentItems.ConcatItems(frequentItems);
            IDictionary<string, double> candidates = new Dictionary<string, double>();
            double transactionsCount = transactions.Count();

            do
            {
                candidates = GenerateCandidates(frequentItems, transactions);
                frequentItems = GetFrequentItems(candidates, minSupport, transactionsCount);
                Print(frequentItems);
                allFrequentItems.ConcatItems(frequentItems);
            }
            while (candidates.Count != 0);

            HashSet<Rule> rules = GenerateRules(allFrequentItems);
            IList<Rule> strongRules = GetStrongRules(minConfidence, rules, allFrequentItems);
            Dictionary<string, Dictionary<string, double>> closedItemSets = GetClosedItemSets(allFrequentItems);
            IList<string> maximalItemSets = GetMaximalItemSets(closedItemSets);
            Print(strongRules);

            // Rule Prunning
            var prune = new RulePrunning();
            var rulesAfterPrunning = prune
                .DatabaseCoverageMethod(strongRules, transactions.ToList());

            Console.WriteLine("Rules AFTER prunning with Database Coverage method.");
            Print(rulesAfterPrunning);

            // Rule Ranking
            var rank = new RuleRanking();
            var rankedRules = rank.ACS(rulesAfterPrunning);
            Console.WriteLine("Rules ranked using ACS");
            Print(rankedRules);

            rankedRules = rank.CSA(rulesAfterPrunning);
            Console.WriteLine("Rules ranked using CSA");
            Print(rankedRules);
            
            var sb = new StringBuilder($"Itemset\t\tClass\n=====================\n");
            foreach (var rule in rankedRules)
            {
                sb.AppendFormat("{0};{1}\t{2}\n", rule.X, rule.Y, @class);
            }
            sb.AppendLine();
            Console.WriteLine(sb);


            //return new Output
            //{
            //    StrongRules = strongRules,
            //    MaximalItemSets = maximalItemSets,
            //    ClosedItemSets = closedItemSets,
            //    FrequentItems = allFrequentItems
            //};
            return null;
        }

        #endregion

        #region Private Methods

        private void Print(double minSup, double minConf)
        {
            Console.WriteLine("Minimum Support: {0}\nMinimum Confidence: {1}", minSup, minConf);
        }

        private void Print(IList<Rule> rules)
        {
            if (rules.Count == 0) return;

            var sb = new StringBuilder($"Rules (Total = {rules.Count})\n===================\n");
            foreach (var rule in rules)
            {
                sb.AppendFormat("{0}\n", rule);
            }
            sb.AppendLine();
            Console.WriteLine(sb);
        }

        private void Print(IList<Item> items)
        {
            if (items.Count == 0) return;

            var sb = new StringBuilder("Itemset\t\tSupport\n================================\n");
            foreach (var item in items.OrderByDescending(x => x.Support))
            {
                sb.AppendFormat("{0}\t\t{1}\n", item.ItemIDs, item.Support);
            }
            sb.AppendLine();
            Console.WriteLine(sb);
        }

        private List<Item> GetL1FrequentItems(double minSupport, IEnumerable<string> items, IEnumerable<string> transactions)
        {
            var frequentItemsL1 = new List<Item>();
            double transactionsCount = transactions.Count();

            foreach (var item in items)
            {
                double support = GetSupport(item, transactions);

                if (support / transactionsCount >= minSupport)
                {
                    frequentItemsL1.Add(new Item { ItemIDs = item, Support = support });
                }
            }
            frequentItemsL1.Sort();
            return frequentItemsL1;
        }

        private double GetSupport(string generatedCandidate, IEnumerable<string> transactionsList)
        {
            double support = 0;

            foreach (string transaction in transactionsList)
            {
                if (CheckIsSubset(generatedCandidate, transaction))
                {
                    support++;
                }
            }

            return support;
        }

        private bool CheckIsSubset(string child, string parent)
        {
            var childList = child.Split(';');
            foreach (var oneChild in childList)
            {
                if (!parent.Contains(oneChild)) return false;
            }
            return true;
        }

        private Dictionary<string, double> GenerateCandidates(IList<Item> frequentItems, IEnumerable<string> transactions)
        {
            Dictionary<string, double> candidates = new Dictionary<string, double>();

            for (int i = 0; i < frequentItems.Count - 1; i++)
            {
                string firstItem = _sorter.Sort(frequentItems[i].ItemIDs);

                for (int j = i + 1; j < frequentItems.Count; j++)
                {
                    string secondItem = _sorter.Sort(frequentItems[j].ItemIDs);
                    string generatedCandidate = GenerateCandidate(firstItem, secondItem);

                    if (generatedCandidate != string.Empty)
                    {
                        double support = GetSupport(generatedCandidate, transactions);
                        candidates.Add(generatedCandidate, support);
                    }
                }
            }

            return candidates;
        }

        private string GenerateCandidate(string firstItem, string secondItem)
        {
            string[] firstItemList = firstItem.Split(';');
            string[] secondItemList = secondItem.Split(';');
            int length = firstItemList.Length; // == secondItemList.Length

            if (length == 1)
            {
                return firstItem + ";" + secondItem;
            }
            else
            {
                for (int i = 0; i < length -1; i++)
                {
                    if (firstItemList[i] != secondItemList[i]) return string.Empty;
                }

                return firstItem + ";" + secondItemList[length - 1];
            }
        }

        private List<Item> GetFrequentItems(IDictionary<string, double> candidates, double minSupport, double transactionsCount)
        {
            var frequentItems = new List<Item>();

            foreach (var item in candidates)
            {
                if (item.Value / transactionsCount >= minSupport)
                {
                    frequentItems.Add(new Item { ItemIDs = item.Key, Support = item.Value });
                }
            }

            return frequentItems;
        }

        private Dictionary<string, Dictionary<string, double>> GetClosedItemSets(ItemsDictionary allFrequentItems)
        {
            var closedItemSets = new Dictionary<string, Dictionary<string, double>>();
            int i = 0;

            foreach (var item in allFrequentItems)
            {
                Dictionary<string, double> parents = GetItemParents(item.ItemIDs, ++i, allFrequentItems);

                if (CheckIsClosed(item.ItemIDs, parents, allFrequentItems))
                {
                    closedItemSets.Add(item.ItemIDs, parents);
                }
            }

            return closedItemSets;
        }

        private Dictionary<string, double> GetItemParents(string child, int index, ItemsDictionary allFrequentItems)
        {
            var parents = new Dictionary<string, double>();
            int childSplitLength = child.Split(';').Length;
            for (int j = index; j < allFrequentItems.Count; j++)
            {
                string parent = allFrequentItems[j].ItemIDs;
                
                if (parent.Split(';').Length == childSplitLength + 1)
                {
                    if (CheckIsSubset(child, parent))
                    {
                        parents.Add(parent, allFrequentItems[parent].Support);
                    }
                }
            }

            return parents;
        }

        private bool CheckIsClosed(string child, Dictionary<string, double> parents, ItemsDictionary allFrequentItems)
        {
            foreach (string parent in parents.Keys)
            {
                if (allFrequentItems[child].Support == allFrequentItems[parent].Support)
                {
                    return false;
                }
            }

            return true;
        }

        private IList<string> GetMaximalItemSets(Dictionary<string, Dictionary<string, double>> closedItemSets)
        {
            var maximalItemSets = new List<string>();

            foreach (var item in closedItemSets)
            {
                Dictionary<string, double> parents = item.Value;

                if (parents.Count == 0)
                {
                    maximalItemSets.Add(item.Key);
                }
            }

            return maximalItemSets;
        }

        private HashSet<Rule> GenerateRules(ItemsDictionary allFrequentItems)
        {
            var rulesSet = new HashSet<Rule>();

            foreach (var item in allFrequentItems)
            {
                //if we have more than one item in the itemset
                if (item.ItemIDs.Contains(";"))
                {
                    IEnumerable<string> subsetsList = GenerateSubsets(item.ItemIDs);

                    foreach (var subset in subsetsList)
                    {
                        string remaining = GetRemaining(subset, item.ItemIDs);
                        Rule rule = new Rule(subset, remaining, 0, item.Support);
                        
                        rulesSet.Add(rule);
                    }
                }
            }

            return rulesSet;
        }

        private IEnumerable<string> GenerateSubsets(string item)
        {
            IList<string> allSubsets = new List<string>();
            var itemSplitted = item.Split(';');
            var n = itemSplitted.Length;
            int setSize = (1 << n); // = 2^n
            for (int i = 1; i < setSize - 1; i++) // remove the empty set [00...0] and the full [11...1]
            {
                BitArray b = new BitArray(BitConverter.GetBytes(i));
                for (int bit = 0; bit < n; bit++)
                {
                    if (b[bit])
                    {
                        allSubsets.Add(itemSplitted[bit]);
                    }
                }
            }
            return allSubsets;
        }
        
        private string GetRemaining(string child, string parent)
        {
            var childList = child.Split(';');
            var parentList = parent.Split(';').ToList();
            for (int i = childList.Length - 1; i >= 0; i--)
            {
                parentList.Remove(childList[i]);
            }
            return string.Join(";", parentList);
        }

        private IList<Rule> GetStrongRules(double minConfidence, HashSet<Rule> rules, ItemsDictionary allFrequentItems)
        {
            var strongRules = new List<Rule>();

            foreach (Rule rule in rules)
            {
                string xy = _sorter.Sort(rule.X + ";" + rule.Y);
                AddStrongRule(rule, xy, strongRules, minConfidence, allFrequentItems);
            }

            strongRules.Sort();
            return strongRules;
        }

        private void AddStrongRule(Rule rule, string XY, List<Rule> strongRules, double minConfidence, ItemsDictionary allFrequentItems)
        {
            double confidence = GetConfidence(rule.X, XY, allFrequentItems);

            if (confidence >= minConfidence)
            {
                Rule newRule = new Rule(rule.X, rule.Y, confidence, rule.Support);
                strongRules.Add(newRule);
            }

            confidence = GetConfidence(rule.Y, XY, allFrequentItems);

            if (confidence >= minConfidence)
            {
                Rule newRule = new Rule(rule.Y, rule.X, confidence, rule.Support);
                strongRules.Add(newRule);
            }
        }

        private double GetConfidence(string X, string XY, ItemsDictionary allFrequentItems)
        {
            double supportX = allFrequentItems[X].Support;
            double supportXY = allFrequentItems[XY].Support;
            return supportXY / supportX;
        }

        #endregion
    }
}
