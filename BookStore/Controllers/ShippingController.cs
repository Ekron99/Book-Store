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
    public class ShippingController : Controller
    {
        private BookStoreEntities db = new BookStoreEntities();

        // GET: Shipping
        [Authorize(Roles = "A")]
        public ActionResult Index()
        {
            return View(db.Shippings.ToList());
        }

        public ActionResult CreateForCheckout()
        {

            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Shipping shipping = db.Shippings.Find(id);
            if (shipping == null)
            {
                return HttpNotFound();
            }
            return View(shipping);
        }

        // for some reason when you submit the info it won't fire this method...
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Shipping shipping)
        {
            if (ModelState.IsValid)
            {
                Customer customer = db.Customers.Where(x => x.Username == User.Identity.Name).First();
                db.usp_AddShipping(customer.CustomerID, shipping.Street, shipping.City,shipping.State, shipping.Zip);
                if (shipping.BillingIsSame)
                {
                    db.usp_AddBilling(customer.CustomerID, shipping.Street, shipping.City, shipping.State, shipping.Zip);
                } else
                {
                    //see if user has billing address
                    try
                    {
                        Billing billing = db.Billings.Where(x => x.CustomerID == customer.CustomerID).First();
                    }
                    catch (Exception e)
                    {
                        return RedirectToAction("Create", "Billing", null);
                    }
                }
                
                //see if user has credit card
                try
                {
                    Credit_Card card = db.Credit_Card.Where(x => x.CustomerID == customer.CustomerID).First();
                }
                catch (Exception e)
                {
                    return RedirectToAction("CreateForCheckout", "CreditCard", null);
                }

                return RedirectToAction("Index");
            }

            return View(shipping);
        }

        // GET: Shipping/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Shipping shipping = db.Shippings.Find(id);
            if (shipping == null)
            {
                return HttpNotFound();
            }
            return View(shipping);
        }

        // POST: Shipping/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ShipID,CustomerID,Street,City,State,Zip")] Shipping shipping)
        {
            if (ModelState.IsValid)
            {
                shipping.CustomerID = db.Customers.Where(x => x.Username == User.Identity.Name).First().CustomerID;
                db.Entry(shipping).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("YourAccount", "Account", null);
            }
            return View(shipping);
        }

        // GET: Shipping/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Shipping shipping = db.Shippings.Find(id);
            if (shipping == null)
            {
                return HttpNotFound();
            }
            return View(shipping);
        }

        // POST: Shipping/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Shipping shipping = db.Shippings.Find(id);
            db.Shippings.Remove(shipping);
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
