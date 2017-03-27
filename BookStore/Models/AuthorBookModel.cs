using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookStore.Models
{
    public class AuthorBookModel
    {
        public int AuthorID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

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