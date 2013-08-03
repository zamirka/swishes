using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using swishes.Models;
using swishes.DataAccess;

namespace swishes.Controllers
{
    public class WishesController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        //
        // GET: /Wishes/

        public ActionResult Index()
        {
            return View(db.Wishes.ToList());
        }

        //
        // GET: /Wishes/Details/5

        public ActionResult Details(int id = 0)
        {
            Wish wish = db.Wishes.Find(id);
            if (wish == null)
            {
                return HttpNotFound();
            }
            return View(wish);
        }

        //
        // GET: /Wishes/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Wishes/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Wish wish)
        {
            wish.UserId=Membership.GetUser().ProviderUserKey   
            db.Wishes.Add(wish);
                db.SaveChanges();
                return RedirectToAction("Index");

            return View(wish);
        }

        //
        // GET: /Wishes/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Wish wish = db.Wishes.Find(id);
            if (wish == null)
            {
                return HttpNotFound();
            }
            return View(wish);
        }

        //
        // POST: /Wishes/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Wish wish)
        {
            if (ModelState.IsValid)
            {
                db.Entry(wish).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(wish);
        }

        //
        // GET: /Wishes/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Wish wish = db.Wishes.Find(id);
            if (wish == null)
            {
                return HttpNotFound();
            }
            return View(wish);
        }

        //
        // POST: /Wishes/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Wish wish = db.Wishes.Find(id);
            db.Wishes.Remove(wish);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}