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
    
    public partial class usp_Search_Result
    {
        public int BookID { get; set; }
        public int PublisherID { get; set; }
        public string ISBN { get; set; }
        public string Title { get; set; }
        public int PublishYear { get; set; }
        public Nullable<int> PageNum { get; set; }
        public string Language { get; set; }
        public Nullable<int> Edition { get; set; }
        public string CoverType { get; set; }
        public int Inventory { get; set; }
        public decimal Cost { get; set; }
    }
}
