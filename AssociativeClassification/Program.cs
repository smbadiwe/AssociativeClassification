using System;

namespace AssociativeClassification
{
    class Program
    {
        static void Main(string[] args)
        {
            //string ordersFile = @"Test_Orders.csv";
            //string stocksFile = @"Test_Stocks.csv";
            //double minSup = 0.4;
            //double minConf = 0.8;

            string ordersFile = @"OnlineRetailItaly_Orders.csv";
            string stocksFile = @"OnlineRetailItaly_Stocks.csv";
            double minSup = 0.2;
            double minConf = 0.8;

            var processor = new Processor(ordersFile, stocksFile);
            // processor.PreProcessData(400);

            processor.FindFrequentItemSetAndAssociationRules(minSup, minConf);

            Console.ReadKey();
        }
    }
}
