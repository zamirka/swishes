using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using swishes.Models;
using swishes.DataAccess;
using swishes.Core.Entities.Wishes;
using swishes.Core.Entities.Enums;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;
using System.IO;

namespace swishes.Controllers
{
    public class WishesController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        //
        // GET: /Wishes/

        public ActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("~/Wishes/NotAuthenticated");
            }
            var userProfile = db.UserProfiles.Where(up => up.UserName == User.Identity.Name).SingleOrDefault();
            var wishesToShow = db.Wishes.Join(db.WishLists, w => w.WishListId, wl => wl.Id, (w, wl) => new { wishList = wl, wish = w }).Where(couple => couple.wishList.UserId == userProfile.UserId).Select(couple => couple.wish).ToList();
            return View(wishesToShow);
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
            ViewBag.Priorities = GetPriorityListForDropDown();
            return View();
        }

        //
        // POST: /Wishes/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Wish wish)
        {
            var userProfile = db.UserProfiles.Where(u => u.UserName == User.Identity.Name).SingleOrDefault();
            var wishListId = db.WishLists.Where(wl => wl.UserId == userProfile.UserId).Select(wl => wl.Id).SingleOrDefault();
            wish.WishListId = wishListId;
            wish.Status = WishStatuses.FreeForPresent;
            db.Wishes.Add(wish);
            db.SaveChanges();
            return RedirectToAction("Index");
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
            ViewBag.Priorities = GetPriorityListForDropDown();
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
                try
                {
                    db.Wishes.Attach(wish);
                    db.Entry(wish).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    ((IObjectContextAdapter)db).ObjectContext.Refresh(RefreshMode.ClientWins, db.Wishes);
                    db.SaveChanges();
                }
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
            DeleteFile(wish.ImageName);
            db.SaveChanges();
            
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        public ViewResult NotAuthenticated()
        {
            return View();
        }

        private string SaveFile (string userName, HttpPostedFileBase file)
        {
            string fileName = string.Empty;
            if (file != null && file.ContentLength > 0)
            {
                fileName = string.Format("{0}_{1}.jpg", userName, Guid.NewGuid());
                try
                {
                    file.SaveAs(string.Format("{0}/{1}", Server.MapPath("~/UserFiles/WishImages"), fileName));
                }
                catch (Exception ex)
                {
                    fileName = "NoImage.jpg";
                }
            }
            else
            {
                fileName = "NoImage.jpg";
            }
            return fileName;
        }

        private bool DeleteFile(string fileName)
        {
            try
            {
                System.IO.File.Delete(string.Format("{0}/{1}", Server.MapPath("~/UserFiles/WishImages"), fileName));
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private SelectList GetPriorityListForDropDown()
        {
            var priorities = new List<KeyValuePair<int, string>>()
            {
                new KeyValuePair<int, string>(0,"Must have"),
                new KeyValuePair<int, string>(1,"Nice to have"),
                new KeyValuePair<int, string>(2,"Maybe")
            };
            return new SelectList(priorities, "Key", "Value");
        }
    }
}