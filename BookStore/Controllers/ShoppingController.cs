using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Data.SqlClient;
using BookStore.Models;
using System;

namespace BookStore.Controllers
{
    public class ShoppingController : Controller
    {
        private BookStoreEntities db = new BookStoreEntities();

        // GET: Shopping
        [Authorize(Roles ="A, U")]
        public ActionResult Index()
        {
            var user = User.Identity.Name;
            Customer customer = db.Customers.Where(x => x.Username == user).First();
            Shopping cart = db.Shoppings.Where(x => x.CustomerID == customer.CustomerID).First();
            List<CartResult> results = new List<CartResult>();

            using (SqlConnection connection = new SqlConnection("data source=cs.cofo.edu;initial catalog=gvaught;persist security info=True;user id=gvaught;password=beargav;MultipleActiveResultSets=True;App=EntityFramework"))
            {
                SqlCommand cmd = new SqlCommand("usp_GetCart", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter p1 = new SqlParameter("@cartID", SqlDbType.Int);
                SqlParameter p2 = new SqlParameter("@custID", SqlDbType.Int);
                p1.Value = cart.CartID;
                p2.Value = customer.CustomerID;
                cmd.Parameters.Add(p1);
                cmd.Parameters.Add(p2);
                connection.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    CartResult book = new CartResult();
                    book.BookID = (int)dr["BookID"];
                    book.Title = (string)dr["Title"];
                    book.ISBN = (string)dr["ISBN"];
                    book.PublisherID = (int)dr["PublisherID"];
                    book.PublishYear = (int)dr["PublishYear"];
                    book.PageNum = (int)dr["PageNum"];
                    book.Language = (string)dr["Language"];
                    book.Edition = (int)dr["Edition"];
                    book.CoverType = (string)dr["CoverType"];
                    book.Amount = (int)dr["Amount"];
                    book.Price = (decimal)dr["Price"];
                    book.TotalPrice = (decimal)dr["TotalPrice"];
                    book.CustomerID = (int)dr["CustomerID"];
                    results.Add(book);
                }
                connection.Close();
            }

            //var shoppings = db.Shoppings.Include(s => s.Customer);    
            return View(results);
        }

        public ActionResult Checkout(string Name)
        {
            Customer customer = db.Customers.Where(x => x.Username == Name).First();
            //see if they have a shipping address 
            try
            {
                Shipping ship = db.Shippings.Where(x => x.CustomerID == customer.CustomerID).First();
            } catch (Exception e)
            {
                //RedirectToRoute("Shipping/Create");
                return RedirectToAction("CreateForCheckout", "Shipping", null);
            }
            //see if they have a billing address
            try
            {
                Billing bill = db.Billings.Where(x => x.CustomerID == customer.CustomerID).First();
            }
            catch (Exception e)
            {
                return RedirectToAction("CreateForCheckout", "Billing", null);
            }
            //see if they have a credit card
            try
            {
                Credit_Card card = db.Credit_Card.Where(x => x.CustomerID == customer.CustomerID).First();
            }
            catch (Exception e)
            {
                return RedirectToAction("CreateForCheckout", "CreditCard", null);
            }
            return RedirectToAction("ReviewInvoice", "Invoice", null);
        }

        [AllowAnonymous]
        public ActionResult AddToCart(int? id, int? quantity)
        {
            var user = User.Identity.Name;
            if (User.Identity.IsAuthenticated)
            {
                Book book = db.Books.Where(x => x.BookID == id).First();
                Customer customer = db.Customers.Where(x => x.Username == user).First();
                Shopping cart = db.Shoppings.Where(x => x.CustomerID == customer.CustomerID).First();
                // Cart_Book cb = db.Cart_Book.Where(x => x.CartID == cart.CartID && x.BookID == id).First();
                if (quantity > book.Inventory)
                {
                    quantity = book.Inventory;
                }
                db.usp_AddBookToCart(cart.CartID, book.BookID, customer.CustomerID, quantity);

            } else
            {
                return RedirectToAction("Login", "Account", null);
            }

            return RedirectToAction("Index", "Book", null);
        }

        public ActionResult RemoveFromCart(int? id, int? quantity)
        {
            if (User.Identity.IsAuthenticated)
            {
                int customerID = db.Customers.Where(x => x.Username == User.Identity.Name).First().CustomerID;
                db.usp_RemoveFromCart(id, customerID, quantity);
            }

            return RedirectToAction("Index", "Shopping", null);
        }


        // GET: Shopping/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Shopping shopping = db.Shoppings.Find(id);
            if (shopping == null)
            {
                return HttpNotFound();
            }
            return View(shopping);
        }

        // GET: Shopping/Create
        public ActionResult Create()
        {
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "FirstName");
            return View();
        }

        // POST: Shopping/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CartID,CustomerID,TotalPrice")] Shopping shopping)
        {
            if (ModelState.IsValid)
            {
                db.Shoppings.Add(shopping);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "FirstName", shopping.CustomerID);
            return View(shopping);
        }

        // GET: Shopping/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Shopping shopping = db.Shoppings.Find(id);
            if (shopping == null)
            {
                return HttpNotFound();
            }
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "FirstName", shopping.CustomerID);
            return View(shopping);
        }

        // POST: Shopping/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CartID,CustomerID,TotalPrice")] Shopping shopping)
        {
            if (ModelState.IsValid)
            {
                db.Entry(shopping).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "FirstName", shopping.CustomerID);
            return View(shopping);
        }

        // GET: Shopping/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Shopping shopping = db.Shoppings.Find(id);
            if (shopping == null)
            {
                return HttpNotFound();
            }
            return View(shopping);
        }

        // POST: Shopping/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Shopping shopping = db.Shoppings.Find(id);
            db.Shoppings.Remove(shopping);
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
