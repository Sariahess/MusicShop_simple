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
    public class UserSongsController : Controller
    {
        readonly private MusicShopEntities db = new MusicShopEntities();

        // GET: UserSongs
        public ActionResult Index()
        {
            var userSongs = db.UserSongs.Include(u => u.Song).Include(u => u.User);
            return View(userSongs.ToList());
        }

        // GET: UserSongs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserSong userSong = db.UserSongs.Find(id);
            if (userSong == null)
            {
                return HttpNotFound();
            }
            return View(userSong);
        }

        // GET: UserSongs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserSong userSong = db.UserSongs.Find(id);
            if (userSong == null)
            {
                return HttpNotFound();
            }
            ViewBag.SongId = new SelectList(db.Songs, "Id", "Name", userSong.SongId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "Name", userSong.UserId);
            return View(userSong);
        }

        // POST: UserSongs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,SongId,UserId,Rating,PurchaseDate")] UserSong userSong)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userSong).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.SongId = new SelectList(db.Songs, "Id", "Name", userSong.SongId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "Name", userSong.UserId);
            return View(userSong);
        }

        // GET: UserSongs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserSong userSong = db.UserSongs.Find(id);
            if (userSong == null)
            {
                return HttpNotFound();
            }
            return View(userSong);
        }

        // POST: UserSongs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UserSong userSong = db.UserSongs.Find(id);
            db.UserSongs.Remove(userSong);
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
