using BookStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;

namespace BookStore.Controllers
{
    public class AccountController : Controller
    {
        BookStoreEntities db = new BookStoreEntities();
        
        [Authorize(Roles = "U,A")]
        public ActionResult YourAccount()
        {
            Customer customer = db.Customers.Where(x => x.Username == User.Identity.Name).First();
            Shipping ship = null;
            Billing bill = null;
            Credit_Card card = null;
            try
            {
                ship = db.Shippings.Where(x => x.CustomerID == customer.CustomerID).First();
            } catch (Exception e)
            {

            }

            try
            {
                bill = db.Billings.Where(x => x.CustomerID == customer.CustomerID).First();
            } catch (Exception e)
            {

            }

            try
            {
                card = db.Credit_Card.Where(x => x.CustomerID == customer.CustomerID).First();
            }
            catch (Exception e)
            {

            }

            InvoiceModel invoice = new InvoiceModel();
            invoice.Invoices = new List<Invoice>();


            if (card != null)
            {
                var invoices = db.Invoices.Where(x => x.CardID == card.CardID);
                foreach (var item in invoices)
                {
                    invoice.Invoices.Add(item);
                }
                invoice.CardID = card.CardID;
                invoice.Card = card;
            }
            if (ship != null)
            {
                invoice.ShippingID = ship.ShipID;
                invoice.Shipping = ship;
            }
            if (bill != null)
            {
                invoice.Billing = bill;
                invoice.BillingID = bill.BillID;
            }
            invoice.CustomerID = customer.CustomerID;
            
            
            

            return View(invoice);
        }
        
        public ActionResult ChangePassword()
        {
            ViewBag.incorrect = false;
            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(PasswordModel password)
        {
            ViewBag.incorrect = false;
            Customer customer = db.Customers.Where(x => x.Username == User.Identity.Name).First();
            string temppassword = hashString(customer.salt + password.OldPassword);
            int count = db.Customers.Where(x => x.Username == customer.Username && x.Password == temppassword).Count();
            if (count == 1 && password.CurrentPassword == password.ConfirmPassword)
            {
                string salt = Crypto.GenerateSalt();
                string hash = hashString(salt + password.ConfirmPassword);
                db.usp_UpdateCustomerPassword(customer.CustomerID, salt, hash);

                return RedirectToAction("YourAccount", "Account", null);
            } else
            {
                ViewBag.incorrect = true;
            }

            return View();
        }

        [AllowAnonymous]
        public ActionResult Login(string ReturnUrl)
        {
            ViewBag.Failed = false;
            ViewBag.ReturnUrl = ReturnUrl;

            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(Customer customer, string ReturnUrl)
        {
            ViewBag.Failed = false;
            if (customer.Username == null)
            {
                return View();
            }
            Customer real = db.Customers.Where(x => x.Username == customer.Username).First();
            string password = hashString(real.salt + customer.Password);
            var count = db.Customers.Where(x => x.Username == customer.Username && x.Password == password).Count();
            if (count == 0)
            {
                ViewBag.Message = "Invalid User";
                ViewBag.Failed = true;
                return View();
            }
            else
            {
                FormsAuthentication.SetAuthCookie(customer.Username, false);
                if (ReturnUrl == null)
                {
                    return RedirectToAction("Index", "Home", null);
                }
                return Redirect(ReturnUrl);
            }
        }

        private string hashString(string input)
        {
            SHA256 crypt = SHA256.Create();
            byte[] result = crypt.ComputeHash(Encoding.Default.GetBytes(input));
            StringBuilder sb = new StringBuilder();
            for (int n = 0; n < result.Length; n++)
            {
                sb.Append(result[n]);
            }
            return sb.ToString();
        }

        [AllowAnonymous]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}