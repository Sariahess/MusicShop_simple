using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using MusicShop;

namespace MusicShop.Controllers
{
    public class SongsController : Controller
    {
        readonly private MusicShopEntities db = new MusicShopEntities();
        // Create a page where you select a song, then once you press “Submit”, 
        // you will see how many people bought this song.
        public ActionResult HowManyUsersBoughtTheSong(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Song song = db.Songs.Find(id);

            if (song == null) return HttpNotFound();

            return View(song);
        }

        [HttpPost]
        public ActionResult HowManyUsersBoughtTheSong(int SongId)
        {
            Song theSong = db.Songs.Find(SongId);

            if (theSong == null) return HttpNotFound("no song by that ID");

            int result = theSong.UserSongs.Count;
            ViewBag.SongId = new SelectList(db.Songs, "Id", "Name");
            return View(result);
        }

        // Create a page (Only GET) to show us the song with the top number of sales.
        // (designed this to return multiple songs in case there are more than one song)
        public ActionResult SongsSoldTheMost()
        {
            int maxNum = db.Songs.Max(s => s.UserSongs.Count);
            var topSongs = db.Songs.Where(s => s.UserSongs.Count == maxNum);
            return View(topSongs.ToList());
        }

        // Create a page (GET only) to show the top rated 3 songs.
        public ActionResult Top3RatedSongs()
        {
            var orderByRatings = db.Songs.OrderByDescending
                (s => s.UserSongs.Average
                (us => us.Rating)).Take(3);
            return View(orderByRatings.ToList());
        }

        // GET: Songs
        public ActionResult Index()
        {
            var songs = db.Songs.Include(s => s.Artist);
            return View(songs.ToList());
        }

        // GET: Songs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Song song = db.Songs.Find(id);
            if (song == null)
            {
                return HttpNotFound();
            }
            return View(song);
        }

        // GET: Songs/Create
        public ActionResult Create()
        {
            ViewBag.ArtistId = new SelectList(db.Artists, "Id", "Name");
            return View();
        }

        // POST: Songs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Price,ArtistId")] Song song)
        {
            if (ModelState.IsValid)
            {
                db.Songs.Add(song);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ArtistId = new SelectList(db.Artists, "Id", "Name", song.ArtistId);
            return View(song);
        }

        // GET: Songs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Song song = db.Songs.Find(id);
            if (song == null)
            {
                return HttpNotFound();
            }
            ViewBag.ArtistId = new SelectList(db.Artists, "Id", "Name", song.ArtistId);
            return View(song);
        }

        // POST: Songs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Price,ArtistId")] Song song)
        {
            if (ModelState.IsValid)
            {
                db.Entry(song).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ArtistId = new SelectList(db.Artists, "Id", "Name", song.ArtistId);
            return View(song);
        }

        // GET: Songs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Song song = db.Songs.Find(id);
            if (song == null)
            {
                return HttpNotFound();
            }
            return View(song);
        }

        // POST: Songs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Song song = db.Songs.Find(id);
            db.Songs.Remove(song);
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
