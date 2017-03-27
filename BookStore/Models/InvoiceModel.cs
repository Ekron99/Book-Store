using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookStore.Models
{
    public class InvoiceModel
    {
        public virtual ICollection<Book> Books { get; set; }
        public virtual ICollection<decimal> Prices { get; set; }
        public virtual ICollection<int> Amounts { get; set; }
        public virtual ICollection<string> Publishers { get; set; }
        public virtual ICollection<Invoice> Invoices { get; set; }
        public virtual ICollection<Author> Authors { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public virtual Shopping Cart { get; set; }
        public virtual Shipping Shipping { get; set; }
        public virtual Billing Billing { get; set; }
        public virtual Credit_Card Card { get; set; }
        public string Status { get; set; }
        public int CustomerID { get; set; }
        public int CardID { get; set; }
        public int ShippingID { get; set; }
        public int BillingID { get; set; }
        public int InvoiceID { get; set; }
        public Invoice Invoice { get; set; }
    }
}