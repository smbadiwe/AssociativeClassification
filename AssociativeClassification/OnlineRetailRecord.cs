using System;

namespace AssociativeClassification
{
    class OnlineRetailRecord
    {
        #region Straight from the upload file

        public string InvoiceNo { get; set; }
        public string StockCode { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public DateTime InvoiceDate { get; set; }
        public double UnitPrice { get; set; }
        public int CustomerID { get; set; }

        #endregion
    }
}
