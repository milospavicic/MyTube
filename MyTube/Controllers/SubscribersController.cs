using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MyTube.Models;

namespace MyTube.Controllers
{
    public class SubscribersController : Controller
    {
        private MyTubeDBEntities db = new MyTubeDBEntities();

        // GET: Subscribers
        public ActionResult Index()
        {
            var subscribers = db.Subscribers.Include(s => s.User).Include(s => s.User1);
            return View(subscribers.ToList());
        }

        // GET: Subscribers/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subscriber subscriber = db.Subscribers.Find(id);
            if (subscriber == null)
            {
                return HttpNotFound();
            }
            return View(subscriber);
        }

        // GET: Subscribers/Create
        public ActionResult Create()
        {
            ViewBag.ChannelSubscribed = new SelectList(db.Users, "Username", "Pass");
            ViewBag.Subscriber1 = new SelectList(db.Users, "Username", "Pass");
            return View();
        }

        // POST: Subscribers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SubID,ChannelSubscribed,Subscriber1")] Subscriber subscriber)
        {
            if (ModelState.IsValid)
            {
                db.Subscribers.Add(subscriber);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ChannelSubscribed = new SelectList(db.Users, "Username", "Pass", subscriber.ChannelSubscribed);
            ViewBag.Subscriber1 = new SelectList(db.Users, "Username", "Pass", subscriber.Subscriber1);
            return View(subscriber);
        }

        // GET: Subscribers/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subscriber subscriber = db.Subscribers.Find(id);
            if (subscriber == null)
            {
                return HttpNotFound();
            }
            ViewBag.ChannelSubscribed = new SelectList(db.Users, "Username", "Pass", subscriber.ChannelSubscribed);
            ViewBag.Subscriber1 = new SelectList(db.Users, "Username", "Pass", subscriber.Subscriber1);
            return View(subscriber);
        }

        // POST: Subscribers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SubID,ChannelSubscribed,Subscriber1")] Subscriber subscriber)
        {
            if (ModelState.IsValid)
            {
                db.Entry(subscriber).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ChannelSubscribed = new SelectList(db.Users, "Username", "Pass", subscriber.ChannelSubscribed);
            ViewBag.Subscriber1 = new SelectList(db.Users, "Username", "Pass", subscriber.Subscriber1);
            return View(subscriber);
        }

        // GET: Subscribers/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subscriber subscriber = db.Subscribers.Find(id);
            if (subscriber == null)
            {
                return HttpNotFound();
            }
            return View(subscriber);
        }

        // POST: Subscribers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Subscriber subscriber = db.Subscribers.Find(id);
            db.Subscribers.Remove(subscriber);
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
