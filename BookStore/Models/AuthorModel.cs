using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookStore.Models
{
    public class AuthorModel
    {
        public int AuthorID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int BookID { get; set; }
        public string Title { get; set; }
        public string ISBN { get; set; }


        public ICollection<Book> books = new HashSet<Book>();
    }


}