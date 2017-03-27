using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BookStore.Models
{
    public class CartResult
    {
        [System.ComponentModel.DataAnnotations.Key]
        public int BookID { get; set; }
        public int PublisherID { get; set; }
        public string ISBN { get; set; }
        public string Title { get; set; }
        public int PublishYear { get; set; }
        public Nullable<int> PageNum { get; set; }
        public string Language { get; set; }
        public Nullable<int> Edition { get; set; }
        public string CoverType { get; set; }

        public int CartID { get; set; }
        public int Amount { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }
        public int CustomerID { get; set; }

        public decimal getPrice() {
            return TotalPrice;
        }
    }
}