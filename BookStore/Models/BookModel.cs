using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookStore.Models
{
    public class BookModel
    {
        public Book book { get; set; }
        public Author author { get; set; }
    }
}