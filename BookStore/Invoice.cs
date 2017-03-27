//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BookStore
{
    using System;
    using System.Collections.Generic;
    
    public partial class Invoice
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Invoice()
        {
            this.Book_Invoice = new HashSet<Book_Invoice>();
        }
    
        public int InvoiceID { get; set; }
        public int CardID { get; set; }
        public int CartID { get; set; }
        public int ShipID { get; set; }
        public int BillID { get; set; }
        public string Status { get; set; }
        public decimal TotalPrice { get; set; }
        public Nullable<int> CustomerID { get; set; }
    
        public virtual Billing Billing { get; set; }
        public virtual Credit_Card Credit_Card { get; set; }
        public virtual Shipping Shipping { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Book_Invoice> Book_Invoice { get; set; }
    }
}