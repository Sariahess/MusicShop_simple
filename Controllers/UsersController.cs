using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MusicShop;

namespace MusicShop.Controllers
{
    public class UsersController : Controller
    {
        readonly private MusicShopEntities db = new MusicShopEntities();
        // Create a page where you select a user, then once you press “submit”, 
        // you will view all the songs this user bought.
        public ActionResult SongsPurchasedByTheUser()
        {
            ViewBag.UserId = new SelectList(db.Users, "Id", "Name");
            return View();
        }

        [HttpPost]
        public ActionResult SongsPurchasedByTheUser(int UserId)
        {
            User theUser = db.Users.Find(UserId);

            if (theUser == null) return HttpNotFound("no user by that ID");

            var result = theUser.UserSongs.ToList();
            ViewBag.UserId = new SelectList(db.Users, "Id", "Name");
            return View(result);
        }

        // Create a page where a user can select a song from a list 
        // (only the songs that he didn’t buy already should appear in the list), 
        // then he clicks a “Buy” button, if the user has enough money, the purchase is successful, 
        // otherwise show a failure message that you don’t have enough money.
        public List<Song> NotPurchasedYet(int? id)
        {
            return db.Songs.Where(s => !s.UserSongs.Any(us => us.UserId == id)).ToList();
        }

        public ActionResult BuyASong(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            ViewBag.SongId = new SelectList(NotPurchasedYet(id), "Id", "Name");
            ViewBag.Rating = new SelectList(new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            return View();
        }

        [HttpPost]
        public ActionResult BuyASong(int id, int SongId, int Rating)
        {
            User user = db.Users.Find(id);
            Song song = db.Songs.Find(SongId);
            ViewBag.Message = $"{ user.Name } bought { song.Name }";

            if (song == null) return HttpNotFound("no song by that ID");

            if (user.Money >= song.Price)
            {
                UserSong newPurchase = new UserSong
                {
                    SongId = SongId,
                    UserId = id,
                    Rating = Rating,
                    PurchaseDate = new DateTime().Date
                };
                db.UserSongs.Add(newPurchase);
                user.Money -= song.Price;
                db.SaveChanges();
            }
            else ViewBag.Message = $"Not enough money for { user.Name } to buy \"{ song.Name }\" (price: ${ song.Price }) \n{ user.Name } currently owns ${ user.Money }.";

            ViewBag.SongId = new SelectList(NotPurchasedYet(id), "Id", "Name");
            ViewBag.Rating = new SelectList(new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            ViewBag.theSong = song;
            return View(user);
        }

        // GET: Users
        public ActionResult Index()
        {
            return View(db.Users.ToList());
        }

        // GET: Users/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Money")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(user);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Money")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
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
