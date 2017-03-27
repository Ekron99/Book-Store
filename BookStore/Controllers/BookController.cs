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
using System.Data.Entity.Core.Objects;
using System.Data.Common;

namespace BookStore.Controllers
{
    public class BookController : Controller
    {
        private BookStoreEntities db = new BookStoreEntities();

        // GET: Book
        [AllowAnonymous]
        public ActionResult Index()
        {
            List<BookModel> result = new List<BookModel>();
            var books = db.Books.Include(b => b.Publisher);

            foreach (var item in books)
            {
                BookModel book = new BookModel();
                book.book = item;
                book.author = db.Authors.Where(x => x.Books.Where(i => i.BookID == item.BookID).Count() == 1).First();
                result.Add(book);   

            }

            return View(result.ToList());
        }

        // GET: Book/Details/5
        [AllowAnonymous]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }

        // GET: Book/Create
        [Authorize(Roles = "A")]
        public ActionResult Create()
        {
            ViewBag.PublisherID = new SelectList(db.Publishers, "PublisherID", "Name");
            return View();
        }

        // POST: Book/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "A")]
        public ActionResult Create([Bind(Include = "BookID,PublisherID,ISBN,Title,PublishYear,PageNum,Language,Edition,CoverType,Inventory,Cost")] Book book)
        {
            if (ModelState.IsValid)
            {
                db.Books.Add(book);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PublisherID = new SelectList(db.Publishers, "PublisherID", "Name", book.PublisherID);
            return View(book);
        }

        // GET: Book/Edit/5
        [Authorize(Roles = "A")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            ViewBag.PublisherID = new SelectList(db.Publishers, "PublisherID", "Name", book.PublisherID);
            return View(book);
        }

        // POST: Book/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "A")]
        public ActionResult Edit([Bind(Include = "BookID,PublisherID,ISBN,Title,PublishYear,PageNum,Language,Edition,CoverType,Inventory,Cost")] Book book)
        {
            if (ModelState.IsValid)
            {
                db.Entry(book).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PublisherID = new SelectList(db.Publishers, "PublisherID", "Name", book.PublisherID);
            return View(book);
        }

        // GET: Book/Delete/5
        [Authorize(Roles = "A")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }

        // POST: Book/Delete/5
        [Authorize(Roles = "A")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Book book = db.Books.Find(id);
            db.Books.Remove(book);
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
