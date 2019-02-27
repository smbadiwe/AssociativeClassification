using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssociativeClassification
{
    class Processor
    {
        private readonly string ordersFile;
        private readonly string stocksFile;
        public Processor(string ordersFile, string stocksFile)
        {
            this.ordersFile = ordersFile;
            this.stocksFile = stocksFile;
        }
        
        public void FindFrequentItemSetAndAssociationRules(double minSupport, double minConfidence)
        {
            List<string> orderItems2 = new List<string>();
            List<Order> orderItems = new List<Order>();
            using (TextFieldParser csvParser = new TextFieldParser(ordersFile))
            {
                csvParser.CommentTokens = new string[] { "#" };
                csvParser.SetDelimiters(new string[] { "," });
                csvParser.HasFieldsEnclosedInQuotes = true;

                // Skip the row with the column names
                csvParser.ReadLine();

                while (!csvParser.EndOfData)
                {
                    // Read current line fields, pointer moves to the next line.
                    string[] values = csvParser.ReadFields();

                    orderItems.Add(new Order
                    {
                        Itemset = values[2], // Itemset field
                        Class = bool.Parse(values[4]) // Class
                    });
                }
            }
            
            List<string> stockItems = new List<string>();
            using (TextFieldParser csvParser = new TextFieldParser(stocksFile))
            {
                csvParser.CommentTokens = new string[] { "#" };
                csvParser.SetDelimiters(new string[] { "," });
                csvParser.HasFieldsEnclosedInQuotes = true;

                // Skip the row with the column names
                csvParser.ReadLine();

                while (!csvParser.EndOfData)
                {
                    // Read current line fields, pointer moves to the next line.
                    string[] values = csvParser.ReadFields();

                    stockItems.Add(values[0]); // StockCode field
                }
            }
            
            new AprioriAlgorithm.Apriori()
                .ProcessTransaction(minSupport, minConfidence, stockItems, orderItems);
        }

        public void PreProcessData(double thresholdAmt)
        {
            if (File.Exists("OnlineRetailItaly_Orders.csv") && File.Exists("OnlineRetailItaly_Stocks.csv"))
            {
                return;
            }

            List<OnlineRetailRecord> records = new List<OnlineRetailRecord>();
            using (TextFieldParser csvParser = new TextFieldParser(@"OnlineRetailItaly.csv"))
            {
                csvParser.CommentTokens = new string[] { "#" };
                csvParser.SetDelimiters(new string[] { "," });
                csvParser.HasFieldsEnclosedInQuotes = true;

                // Skip the row with the column names
                csvParser.ReadLine();

                while (!csvParser.EndOfData)
                {
                    // Read current line fields, pointer moves to the next line.
                    string[] values = csvParser.ReadFields();

                    records.Add(new OnlineRetailRecord
                    {
                        InvoiceNo = values[0],
                        StockCode = values[1],
                        Description = values[2],
                        Quantity = int.Parse(values[3]),
                        InvoiceDate = DateTime.Parse(values[4]),
                        UnitPrice = double.Parse(values[5]),
                        CustomerID = int.Parse(values[6])
                    });
                }
            }

            Console.WriteLine("Raw Data loaded. # records: {0}", records.Count);

            var uniqueStockItems = records.Select(x => x.StockCode).Distinct();
            
            var uniqueStockItemsDict = uniqueStockItems.ToDictionary(x => x);
            foreach (var record in records)
            {
                uniqueStockItemsDict[record.StockCode] = record.Description;
            }
            Console.WriteLine("Total unique stock items: {0}", uniqueStockItemsDict.Count);

            // Group such that each row is a customer order on a given date
            List<CustomerOrder> orders = new List<CustomerOrder>();
            var byCustomer = records.GroupBy(x => x.CustomerID);
            foreach (var customerSet in byCustomer)
            {
                foreach (var orderSet in customerSet.GroupBy(x => x.InvoiceNo))
                {
                    var order = new CustomerOrder
                    {
                        CustomerID = customerSet.Key,
                        InvoiceNo = orderSet.Key,
                        Itemset = string.Join(";", orderSet.Select(x => x.StockCode)),
                        TotalPrice = orderSet.Sum(x => x.Quantity * x.UnitPrice)
                    };
                    orders.Add(order);
                }
            }

            Console.WriteLine("Orders summarized. # records: {0}", orders.Count);

            // Rule Labelling (Building a classifier)
            
            foreach (var order in orders)
            {
                if (order.TotalPrice >= thresholdAmt)
                    order.Class = true;
            }
            // Save orders to CSV
            var sb = new StringBuilder("CustomerID, InvoiceNo, Itemset, TotalPrice, Class");
            sb.AppendLine();
            foreach (var order in orders)
            {
                sb.AppendFormat("{0}, {1}, {2}, {3}, {4}", order.CustomerID, order.InvoiceNo, order.Itemset, order.TotalPrice, order.Class)
                    .AppendLine();
            }
            File.WriteAllText("OnlineRetailItaly_Orders.csv", sb.ToString());

            Console.WriteLine("Orders data saved.");

            // Save stocks list to CSV
            sb.Clear();
            sb.AppendLine("StockCode, Description");
            foreach (var stock in uniqueStockItemsDict)
            {
                sb.AppendFormat("{0}, \"{1}\"", stock.Key, stock.Value)
                    .AppendLine();
            }
            File.WriteAllText("OnlineRetailItaly_Stocks.csv", sb.ToString());

            Console.WriteLine("Stocks data saved.");
        }
    }
}
