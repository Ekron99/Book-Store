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
    public class BillingController : Controller
    {
        private BookStoreEntities db = new BookStoreEntities();

        // GET: Billing
        [Authorize(Roles = "A")]
        public ActionResult Index()
        {
            var billings = db.Billings.Include(b => b.Customer);
            return View(billings.ToList());
        }

        // GET: Billing/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Billing billing = db.Billings.Find(id);
            if (billing == null)
            {
                return HttpNotFound();
            }
            return View(billing);
        }

        public ActionResult CreateForCheckout(int invoiceID)
        {

            return View();
        }

        // GET: Billing/Create
        public ActionResult Create()
        {
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "FirstName");
            return View();
        }

        // POST: Billing/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BillID,CustomerID,Street,City,State,Zip")] Billing billing)
        {
            if (ModelState.IsValid)
            {
                Customer customer = db.Customers.Where(x => x.Username == User.Identity.Name).First();
                db.usp_AddBilling(customer.CustomerID, billing.Street, billing.City, billing.State, billing.Zip);
                try
                {
                    Credit_Card card = db.Credit_Card.Where(x => x.CustomerID == customer.CustomerID).First();
                } catch (Exception e)
                {
                    return RedirectToAction("CreateForCheckout", "CreditCard", null);
                }
                return RedirectToAction("Index", "Invoice", null);
            }

            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "FirstName", billing.CustomerID);
            return View(billing);
        }

        // GET: Billing/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Billing billing = db.Billings.Find(id);
            Customer customer = db.Customers.Where(x => x.Username == User.Identity.Name).First();
            if (billing == null)
            {
                return HttpNotFound();
            }
            if (!User.IsInRole("A") || billing.CustomerID != customer.CustomerID)
            {
                return RedirectToAction("Login", "Account", new { @ReturnUrl = "Billing/Edit/" + id });
            }
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "FirstName", billing.CustomerID);
            return View(billing);
        }

        // POST: Billing/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BillID,CustomerID,Street,City,State,Zip")] Billing billing)
        {
            if (ModelState.IsValid)
            {
                Customer customer = db.Customers.Where(x => x.Username == User.Identity.Name).First();
                billing.CustomerID = customer.CustomerID;
                db.Entry(billing).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("YourAccount", "Account", null);
            }
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "FirstName", billing.CustomerID);
            return View(billing);
        }

        // GET: Billing/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Billing billing = db.Billings.Find(id);
            if (billing == null)
            {
                return HttpNotFound();
            }
            return View(billing);
        }

        // POST: Billing/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Billing billing = db.Billings.Find(id);
            db.Billings.Remove(billing);
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
