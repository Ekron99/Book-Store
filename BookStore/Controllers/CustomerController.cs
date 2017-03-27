using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Text;
using System.Security.Cryptography;
using System.Web.Helpers;
using System.Web.Security;

namespace BookStore.Controllers
{
    public class CustomerController : Controller
    {
        private BookStoreEntities db = new BookStoreEntities();

        // GET: Customer
        [Authorize(Roles = "A")]
        public ActionResult Index()
        {
            return View(db.Customers.ToList());
        }

        // GET: Customer/Details/5
        [Authorize(Roles = "A")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // GET: Customer/Create
        [AllowAnonymous]
        public ActionResult Create()
        {
            
            return View();
        }

        // POST: Customer/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult Create([Bind(Include = "CustomerID,FirstName,LastName,Username,Password,Gender,AccessLevel")] Customer customer)
        {
            
            if (ModelState.IsValid)
            {
                string salt = Crypto.GenerateSalt();
                string hash = hashString(salt + customer.Password);
                int hashLength = hash.Length;
                int saltLength = salt.Length;
                if (customer.AccessLevel == null)
                {
                    customer.AccessLevel = "U";
                }
                if (customer.FirstName == null || customer.LastName == null || customer.Username == null || customer.Password == null || customer.Gender == null)
                {
                    return View();
                }
                db.usp_AddCustomer(customer.FirstName, customer.LastName, customer.Username, hash, customer.Gender, customer.AccessLevel, salt);
                FormsAuthentication.SetAuthCookie(customer.Username, false);
                return RedirectToAction("Index", "Home", null);
            }

            return View(customer);
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

        // GET: Customer/Edit/5
        [Authorize(Roles = "A")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Customer/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "A")]
        public ActionResult Edit([Bind(Include = "CustomerID,FirstName,LastName,Username,Password,Gender,AccessLevel")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(customer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(customer);
        }

        // GET: Customer/Delete/5
        [Authorize(Roles = "A")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Customer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "A")]
        public ActionResult DeleteConfirmed(int id)
        {
            Customer customer = db.Customers.Find(id);
            db.Customers.Remove(customer);
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
