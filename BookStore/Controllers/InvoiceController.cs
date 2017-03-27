using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BookStore;
using BookStore.Models;
using System.Data.SqlClient;

namespace BookStore.Controllers
{
    public class InvoiceController : Controller
    {
        private BookStoreEntities db = new BookStoreEntities();

        // GET: Invoice
        public ActionResult Index()
        {
            var customer = db.Customers.Where(x => x.Username == User.Identity.Name).First();
            var result = db.Invoices.Where(x => x.CustomerID == customer.CustomerID);

            return View(result.ToList());
        }

        public ActionResult ReviewInvoice()
        {
            Customer customer = db.Customers.Where(x => x.Username == User.Identity.Name).First();
            Shopping cart = db.Shoppings.Where(x => x.CustomerID == customer.CustomerID).First();
            Shipping ship = db.Shippings.Where(x => x.CustomerID == customer.CustomerID).First();
            Billing bill = db.Billings.Where(x => x.CustomerID == customer.CustomerID).First();
            Credit_Card card = db.Credit_Card.Where(x => x.CustomerID == customer.CustomerID).First();
            List<Book> results = new List<Book>();
            InvoiceModel invoice = new InvoiceModel();
            invoice.Prices = new List<decimal>();
            invoice.Amounts = new List<int>();
            invoice.Publishers = new List<string>();

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
                    Book book = new Book();
                    book.BookID = (int)dr["BookID"];
                    book.Title = (string)dr["Title"];
                    book.ISBN = (string)dr["ISBN"];
                    book.PublisherID = (int)dr["PublisherID"];
                    book.PublishYear = (int)dr["PublishYear"];
                    book.PageNum = (int)dr["PageNum"];
                    book.Language = (string)dr["Language"];
                    book.Edition = (int)dr["Edition"];
                    book.CoverType = (string)dr["CoverType"];
                    invoice.Prices.Add((decimal)dr["Price"]);
                    invoice.Amounts.Add((int)dr["Amount"]);
                    int pubID = (int)dr["PublisherID"];
                    invoice.Publishers.Add(db.Publishers.Where(x => x.PublisherID == pubID).First().Name);
                    results.Add(book);
                }
                connection.Close();
            }
            
            invoice.Books = results.ToList();
            invoice.Cart = cart;
            invoice.Shipping = ship;
            invoice.Billing = bill;
            invoice.Card = card;
            invoice.TotalPrice = cart.TotalPrice;

            return View(invoice);
        }

        public ActionResult ConfirmOrder(int cartID, int shipID, int billID, int cardID)
        {
            Customer customer = db.Customers.Where(x => x.Username == User.Identity.Name).First();
            var books = db.Cart_Book.Where(x => x.CartID == cartID);
            List<Cart_Book> list = new List<Cart_Book>();
            foreach (var item in books)
            {
                list.Add(item);
            }
            db.usp_AddInvoice(cartID, shipID, billID, cardID, customer.CustomerID);
            db.usp_CreateCart(customer.CustomerID);
            Invoice invoice = db.Invoices.Where(x => x.CartID == cartID).First();
            foreach (var item in list)
            {
                db.usp_AddBookToInvoice(item.BookID, invoice.InvoiceID, item.Amount, item.Price);
            }

            return RedirectToAction("Index", "Invoice", null);
            
        }

        // GET: Invoice/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invoice invoice = db.Invoices.Find(id);
            if (invoice == null)
            {
                return HttpNotFound();
            }

            var invoices = db.Book_Invoice.Where(x => x.InvoiceID == id);
            InvoiceModel model = new InvoiceModel();
            Customer customer = db.Customers.Where(x => x.Username == User.Identity.Name).First();
            model.Books = new List<Book>();
            model.Amounts = new List<int>();
            model.Prices = new List<decimal>();
            model.Authors = new List<Author>();
            model.Status = invoice.Status;
            model.TotalPrice = invoice.TotalPrice;
            model.Shipping = db.Shippings.Where(x => x.ShipID == invoice.ShipID).First();
            model.Card = db.Credit_Card.Where(x => x.CustomerID == customer.CustomerID).First();
            foreach (var item in invoices)
            {
                Book book = db.Books.Where(x => x.BookID == item.BookID).First();
                model.Authors.Add(db.Authors.Where(x => x.Books.Where(i => i.BookID == item.BookID).Count() == 1).First());
                model.Books.Add(book);
                model.Amounts.Add(item.Quantity);
                model.Prices.Add(item.Price);
            }

            return View(model);
        }

        // GET: Invoice/Create
        [Authorize(Roles ="A")]
        public ActionResult Create()
        {
            ViewBag.BillID = new SelectList(db.Billings, "BillID", "Street");
            ViewBag.CardID = new SelectList(db.Credit_Card, "CardID", "Number");
            ViewBag.CartID = new SelectList(db.Shoppings, "CartID", "CartID");
            ViewBag.ShipID = new SelectList(db.Shippings, "ShipID", "Street");
            return View();
        }

        // POST: Invoice/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "InvoiceID,CardID,CartID,ShipID,BillID,Status,TotalPrice")] Invoice invoice)
        {
            if (ModelState.IsValid)
            {
                db.Invoices.Add(invoice);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BillID = new SelectList(db.Billings, "BillID", "Street", invoice.BillID);
            ViewBag.CardID = new SelectList(db.Credit_Card, "CardID", "Number", invoice.CardID);
            ViewBag.CartID = new SelectList(db.Shoppings, "CartID", "CartID", invoice.CartID);
            ViewBag.ShipID = new SelectList(db.Shippings, "ShipID", "Street", invoice.ShipID);
            return View(invoice);
        }

        // GET: Invoice/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invoice invoice = db.Invoices.Find(id);
            if (invoice == null)
            {
                return HttpNotFound();
            }
            ViewBag.BillID = new SelectList(db.Billings, "BillID", "Street", invoice.BillID);
            ViewBag.CardID = new SelectList(db.Credit_Card, "CardID", "Number", invoice.CardID);
            ViewBag.CartID = new SelectList(db.Shoppings, "CartID", "CartID", invoice.CartID);
            ViewBag.ShipID = new SelectList(db.Shippings, "ShipID", "Street", invoice.ShipID);
            return View(invoice);
        }

        // POST: Invoice/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "InvoiceID,CardID,CartID,ShipID,BillID,Status,TotalPrice")] Invoice invoice)
        {
            if (ModelState.IsValid)
            {
                db.Entry(invoice).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BillID = new SelectList(db.Billings, "BillID", "Street", invoice.BillID);
            ViewBag.CardID = new SelectList(db.Credit_Card, "CardID", "Number", invoice.CardID);
            ViewBag.CartID = new SelectList(db.Shoppings, "CartID", "CartID", invoice.CartID);
            ViewBag.ShipID = new SelectList(db.Shippings, "ShipID", "Street", invoice.ShipID);
            return View(invoice);
        }

        // GET: Invoice/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invoice invoice = db.Invoices.Find(id);
            if (invoice == null)
            {
                return HttpNotFound();
            }
            return View(invoice);
        }

        // POST: Invoice/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Invoice invoice = db.Invoices.Find(id);
            db.Invoices.Remove(invoice);
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
