using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssociativeClassification.AprioriAlgorithm
{
    class RulePrunning
    {
        public IList<Rule> DatabaseCoverageMethod(IList<Rule> rules, List<string> itemsets)
        {
            IList<Rule> newRules = new List<Rule>();
            var orderedRule = rules.OrderByDescending(x => x.Confidence)
                .ThenByDescending(x => x.TotalItemsInRule);

            // For each rule r in R, do:
            foreach (var rule in orderedRule)
            {
                List<string> allItemsetsThatMatchRule = new List<string>();
                foreach (var itemset in itemsets)
                {
                    bool allGood = true;
                    foreach (var itemInRule in rule.ItemsInRule)
                    {
                        if (!itemset.Contains(itemInRule))
                        {
                            allGood = false;
                            break;
                        }
                    }
                    if (allGood) allItemsetsThatMatchRule.Add(itemset);
                }
                if (allItemsetsThatMatchRule.Count > 0)
                {
                    newRules.Add(rule);
                    foreach (var treated in allItemsetsThatMatchRule)
                    {
                        itemsets.Remove(treated);
                    }
                    
                    if (itemsets.Count == 0) break;
                }
            }
            return newRules;
        }
    }
}
