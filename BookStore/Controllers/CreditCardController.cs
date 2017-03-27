using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BookStore;

namespace BookStore.Controllers
{
    [Authorize(Roles = "U,A")]
    public class CreditCardController : Controller
    {
        private BookStoreEntities db = new BookStoreEntities();

        // GET: CreditCard
        public ActionResult Index()
        {
            var credit_Card = db.Credit_Card.Include(c => c.Customer);
            return View(credit_Card.ToList());
        }

        // GET: CreditCard/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Credit_Card credit_Card = db.Credit_Card.Find(id);
            if (credit_Card == null)
            {
                return HttpNotFound();
            }
            return View(credit_Card);
        }

        public ActionResult CreateForCheckout()
        {
            return View();
        }

        // GET: CreditCard/Create
        public ActionResult Create()
        {
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "FirstName");
            return View();
        }

        // POST: CreditCard/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CardID,CustomerID,Number,ExpDate,CCV,CreditType")] Credit_Card credit_Card)
        {
            if (ModelState.IsValid)
            {
                Customer customer = db.Customers.Where(x => x.Username == User.Identity.Name).First();
                db.ups_AddCreditCard(customer.CustomerID, credit_Card.Number, credit_Card.ExpDate, credit_Card.CCV, credit_Card.CreditType);
                return RedirectToAction("ReviewInvoice", "Invoice", null);
            }

            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "FirstName", credit_Card.CustomerID);
            return View(credit_Card);
        }

        // GET: CreditCard/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Credit_Card credit_Card = db.Credit_Card.Find(id);
            if (credit_Card == null)
            {
                return HttpNotFound();
            }
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "FirstName", credit_Card.CustomerID);
            return View(credit_Card);
        }

        // POST: CreditCard/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CardID,CustomerID,Number,ExpDate,CCV,CreditType")] Credit_Card credit_Card)
        {
            if (ModelState.IsValid)
            {
                Customer customer = db.Customers.Where(x => x.Username == User.Identity.Name).First();
                db.usp_editCard(credit_Card.CardID, customer.CustomerID, credit_Card.Number, credit_Card.ExpDate, credit_Card.CCV, credit_Card.CreditType);
                return RedirectToAction("YourAccount", "Account", null);
            }
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "FirstName", credit_Card.CustomerID);
            return View(credit_Card);
        }

        // GET: CreditCard/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Credit_Card credit_Card = db.Credit_Card.Find(id);
            if (credit_Card == null)
            {
                return HttpNotFound();
            }
            return View(credit_Card);
        }

        // POST: CreditCard/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Credit_Card credit_Card = db.Credit_Card.Find(id);
            db.Credit_Card.Remove(credit_Card);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
