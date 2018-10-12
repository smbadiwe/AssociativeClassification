using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssociativeClassification
{
    class CustomerOrder
    {
        public int CustomerID { get; set; }
        public string InvoiceNo { get; set; }
        public double TotalPrice { get; set; }
        public string Itemset { get; set; }
        /// <summary>
        /// True -> Valued customer. False, Regular Customer
        /// </summary>
        public bool Class { get; set; }
    }

    public class Order
    {
        public string Itemset { get; set; }
        /// <summary>
        /// True -> Valued customer. False, Regular Customer
        /// </summary>
        public bool Class { get; set; }
    }
}
