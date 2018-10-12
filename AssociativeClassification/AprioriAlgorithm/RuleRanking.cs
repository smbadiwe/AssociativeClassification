using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssociativeClassification.AprioriAlgorithm
{
    class RuleRanking
    {
        /*
        The CSA rule ordering strategy sorts the CAR list in a descending order 
        based on the value of confidence of each CAR. For these CARs that share 
        a common value of confidence, CSA sorts them in a descending order based 
        on their support value. For these CARs that share common values for both 
        confidence and support, CSA sorts them in an ascending order based on the 
        size of the rule antecedent.
        */
        public List<Rule> CSA(IList<Rule> rules)
        {
            return rules.OrderByDescending(x => x.Confidence)
                .ThenByDescending(x => x.Support)
                .ThenBy(x => x.TotalItemsInRule)
                .ToList();
        }

        /*
        The ACS rule ordering strategy is a variation of CSA. It takes the size of 
        the rule antecedent as its major factor (using a descending
        order) followed by the rule confidence and support values respectively.
        */
        public List<Rule> ACS(IList<Rule> rules)
        {
            return rules.OrderByDescending(x => x.TotalItemsInRule)
                .ThenByDescending(x => x.Confidence)
                .ThenByDescending(x => x.Support)
                .ToList();
        }
    }
}
