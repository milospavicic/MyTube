using MyTube.Models;
using MyTube.Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace MyTube.Controllers
{
    public class CommentsController : Controller
    {
        private MyTubeDBEntities db = new MyTubeDBEntities();
        private CommentsRepository commentsRepository;

        public CommentsController()
        {
            this.commentsRepository = new CommentsRepository(new MyTubeDBEntities());
        }
        public ActionResult CommentSection(long? id, string sortOrder)
        {
            ViewBag.VideoId = id;
            ViewBag.SortOrder = String.IsNullOrEmpty(sortOrder) ? "latest" : "";
            ViewBag.Values = Comment.CommentsSortOrderSelectList();
            if (id == null)
            {
                return PartialView();
            }
            var comments = commentsRepository.GetAllCommentsForVideo((long)id);
            comments = SortComments(comments, sortOrder);
            return PartialView(comments);
        }
        public IEnumerable<Comment> SortComments(IEnumerable<Comment> comments, string sortOrder)
        {
            switch (sortOrder)
            {
                case "latest":
                    comments = comments.OrderByDescending(s => s.DatePosted);
                    break;
                case "oldest":
                    comments = comments.OrderBy(s => s.DatePosted);
                    break;
                case "most_popular":
                    comments = comments.OrderByDescending(s => s.LikesCount).ThenBy(s => s.DislikesCount);
                    break;
                case "least_popular":
                    comments = comments.OrderByDescending(s => s.DislikesCount).ThenBy(s => s.DislikesCount);
                    break;

                default:
                    comments = comments.OrderByDescending(s => s.DatePosted);
                    break;
            }
            return comments;
        }
        public ActionResult CreateComment(Comment comment)
        {
            var currentUser = (string)Session["loggedInUserUsername"];
            if (currentUser == null)
            {
                return null;
            }
            comment.CommentOwner = currentUser;
            comment.DatePosted = DateTime.Now;
            commentsRepository.CreateComment(comment);

            ViewBag.Values = Comment.CommentsSortOrderSelectList();

            return PartialView("SingleComment", comment);

        }
        [HttpPost]
        public ActionResult EditComment(long? id, string text)
        {
            if (id == null)
            {
                return null;
            }
            Comment comment = commentsRepository.GetCommentById((long)id);
            if (comment == null)
            {
                return null;
            }
            comment.CommentText = text;
            commentsRepository.UpdateComment(comment);
            ViewBag.Message = "Comment has been successfully edited.";
            return PartialView("MessageModal");

        }
        public ActionResult DeleteComment(long id)
        {
            commentsRepository.DeleteComment(id);
            ViewBag.Message = "Comment has been successfully deleted.";
            return PartialView("MessageModal");

        }
        // GET: Comments
        public ActionResult Index()
        {
            var comments = db.Comments.Include(c => c.User).Include(c => c.Video);
            return View(comments.ToList());
        }

        // GET: Comments/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // GET: Comments/Create
        public ActionResult Create()
        {
            ViewBag.CommentOwner = new SelectList(db.Users, "Username", "Pass");
            ViewBag.VideoID = new SelectList(db.Videos, "VideoID", "VideoUrl");
            return View();
        }

        // POST: Comments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CommentID,VideoID,CommentOwner,CommentText,DatePosted,LikesCount,DislikesCount,Deleted")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                db.Comments.Add(comment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CommentOwner = new SelectList(db.Users, "Username", "Pass", comment.CommentOwner);
            ViewBag.VideoID = new SelectList(db.Videos, "VideoID", "VideoUrl", comment.VideoID);
            return View(comment);
        }

        // GET: Comments/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            ViewBag.CommentOwner = new SelectList(db.Users, "Username", "Pass", comment.CommentOwner);
            ViewBag.VideoID = new SelectList(db.Videos, "VideoID", "VideoUrl", comment.VideoID);
            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CommentID,VideoID,CommentOwner,CommentText,DatePosted,LikesCount,DislikesCount,Deleted")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(comment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CommentOwner = new SelectList(db.Users, "Username", "Pass", comment.CommentOwner);
            ViewBag.VideoID = new SelectList(db.Videos, "VideoID", "VideoUrl", comment.VideoID);
            return View(comment);
        }

        // GET: Comments/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Comment comment = db.Comments.Find(id);
            db.Comments.Remove(comment);
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
