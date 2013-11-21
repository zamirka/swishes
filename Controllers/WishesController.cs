namespace swishes.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Entity;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Data.Entity.Infrastructure;
    using System.Data.Objects;
    using System.IO;

    using swishes.Models;
    using swishes.Core.Entities.Wishes;
    using swishes.Core.Entities.Enums;
    using swishes.Infrastructure.Logging;
    using swishes.Infrastructure.Repositories;
    using swishes.Core.Entities.Profile;

    public class WishesController : Controller
    {
        private readonly ILogger _logger;
        private readonly IUnitOfWork _uow;
        private readonly IRepository<Wish> _wishesRepo;
        private readonly IRepository<UserProfile> _userProfileRepo;
        private readonly IRepository<WishList> _wishListsRepo;

        public WishesController(ILogger logger, IUnitOfWork uow)
        {
            try
            {
                _logger = logger;
                _uow = uow;
                _wishesRepo = _uow.GetRepository<Wish>();
                _userProfileRepo = _uow.GetRepository<UserProfile>();
                _wishListsRepo = _uow.GetRepository<WishList>();
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Error in WishesController.ctor(ILogger, IUnitOfWork)", ex);
            }
        }

        public ActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("~/Wishes/NotAuthenticated");
            }
            var userProfile = _userProfileRepo.GetAll().Where(up => up.UserName == User.Identity.Name).SingleOrDefault();
            var wishesToShow = _wishesRepo.GetAll()
                .Join(_wishListsRepo.GetAll(), w => w.WishListId, wl => wl.Id, (w, wl) => new { wishList = wl, wish = w })
                .Where(couple => couple.wishList.UserId == userProfile.UserId)
                .Select(couple => couple.wish).ToList();
            
            return View(wishesToShow);
        }

        public ActionResult Details(int id = 0)
        {
            var wish = GetWishById(id);
            if (wish == null)
            {
                return HttpNotFound();
            }
            return View(wish);
        }

        public ActionResult Create()
        {
            ViewBag.Priorities = GetPriorityListForDropDown();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Wish wish)
        {
            int wishListId;
            UserProfile userProfile;
            try
            {
                userProfile = _userProfileRepo.GetAll().Where(u => u.UserName == User.Identity.Name).SingleOrDefault();
                wishListId = _wishListsRepo.GetAll().Where(wl => wl.UserId == userProfile.UserId).Select(wl => wl.Id).SingleOrDefault();
            }
            catch (Exception ex)
            {
                _logger.WarnException("Collection is null or has more than one entity with key", ex);
                return HttpNotFound();
            }
    
            wish.WishListId = wishListId;
            wish.Status = WishStatuses.FreeForPresent;
            _wishesRepo.Add(wish);
            _uow.Save();
            return RedirectToAction("Index");
        }
        
        public ActionResult Edit(int id = 0)
        {
            var wish = GetWishById(id);
            if (wish == null)
            {
                return HttpNotFound();
            }

            ViewBag.Priorities = GetPriorityListForDropDown();
            return View(wish);
        }

        [ValidateAntiForgeryToken]
        public ActionResult Edit(Wish wish)
        {
            if (ModelState.IsValid)
            {
                _wishesRepo.Update(wish);
                _uow.Save();
                //try
                //{
                //    db.Wishes.Attach(wish);
                //    db.Entry(wish).State = EntityState.Modified;
                //    db.SaveChanges();
                //}
                //catch (Exception ex)
                //{
                //    ((IObjectContextAdapter)db).ObjectContext.Refresh(RefreshMode.ClientWins, db.Wishes);
                //    db.SaveChanges();
                //}
                //return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id = 0)
        {
            var wish = GetWishById(id);
            if (wish == null)
            {
                return HttpNotFound();
            }
            return View(wish);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var wish = GetWishById(id);
            if (wish == null)
            {
                return HttpNotFound();
            }
            _wishesRepo.Delete(wish);
            _uow.Save();
            
            return RedirectToAction("Index");
        }

        private Wish GetWishById(int id)
        {
            Wish wish = null;
            try
            {
                wish = _wishesRepo.GetAll().Where(w => w.Id == id).SingleOrDefault();
            }
            catch (Exception ex)
            {
                _logger.WarnException(string.Format("Collection is null or has more than one entity with id = {0}", id), ex);
            }

            return wish;
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