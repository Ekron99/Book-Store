using BookStore.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BookStore.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private BookStoreEntities db = new BookStoreEntities();
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BookResult()
        {
            return View();
        }

        public ActionResult Results(string text, int? type)
        {
            if (type == null)
            {
                type = 1;
            }
            if (text == null)
            {
                text = "";
            }
            List<BookModel> result = new List<BookModel>();
            if (type == 1)
            {
                //search by book
                var list = db.Books.Where(x => x.Title.Contains(text));
                
                foreach (var item in list)
                {
                    BookModel model = new BookModel();
                    Author author = db.Authors.Where(x => x.Books.Where(i => i.BookID == item.BookID).Count() == 1).First();
                    model.book = item;
                    model.author = author;
                    result.Add(model);
                }

            } else if (type == 2)
            {
                return RedirectToAction("AuthorResult", new { text = text });

            } else if (type == 3)
            {
                //search by publisher
                return RedirectToAction("PublisherResult", new { text = text });
            }
            ViewBag.SearchText = text;
            return View(result.ToList());
        }

        public ActionResult PublisherResult(string text)
        {
            ViewBag.SearchText = text; 
            var result = db.Publishers.Where(x => x.Name.Contains(text));
            var first = db.Publishers.Where(x => x.FirstName.Contains(text));
            var last = db.Publishers.Where(x => x.LastName.Contains(text));
            List<Publisher> list = new List<Publisher>();
            foreach (var item in result.Concat(first).Concat(last))
            {
                list.Add(item);
            }
            return View(list.ToList());
        }

        public ActionResult AuthorResult(string text)
        {
            ViewBag.SearchText = text;
            var first = db.Authors.Where(x => x.FirstName.Contains(text));
            var last = db.Authors.Where(x => x.LastName.Contains(text));
            var list = first.Concat(last);
            List<Author> result = new List<Author>();
            foreach(var item in list)
            {
                result.Add(item);
            }

            return View(result.ToList());
        }

        public ActionResult Contact()
        {
            return View();
        }
    }
}
