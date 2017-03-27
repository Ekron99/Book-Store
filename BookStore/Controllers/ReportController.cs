using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BookStore.Controllers
{
    [Authorize(Roles ="A")]
    public class ReportController : Controller
    {
        private BookStoreEntities db = new BookStoreEntities();
        // GET: Report
        public ActionResult Customer()
        {
            ViewBag.Number = db.Customers.Count();
            ViewBag.Activity = db.Invoices.Count();
            return View();
        }

        public ActionResult Product()
        {
            ViewBag.Number = db.Books.Count();
            ViewBag.Inventory = db.Books.Sum(x => x.Inventory);
            ViewBag.Low = db.Books.OrderBy(x => x.Cost).First().Cost;
            ViewBag.High = db.Books.OrderByDescending(x => x.Cost).First().Cost;
            return View();
        }
    }
}