using MyTube.DTO;
using MyTube.Models;
using MyTube.Repository;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace MyTube.Controllers
{
    public class CommentRatingsController : Controller
    {
        private MyTubeDBEntities db = new MyTubeDBEntities();
        private CommentRatingsRepository commentRatingsRepository;
        private CommentsRepository commentsRepository;

        public CommentRatingsController()
        {
            this.commentRatingsRepository = new CommentRatingsRepository(new MyTubeDBEntities());
            this.commentsRepository = new CommentsRepository(new MyTubeDBEntities());
        }
        public JsonResult CommentRatingsForVideo(long? id)
        {
            var username = (string)Session["loggedInUserUsername"];
            if (username != null)
            {
                var crs = commentRatingsRepository.GetCommentRatingsForVideo((long)id, username);
                var crsDTO = CommentRatingsDTO.ConvertCollectionCommentToDTO(crs);
                return Json(crsDTO, JsonRequestBehavior.AllowGet);
            }
            return null;
        }
        [HttpPost]
        public JsonResult CreateCommentRating(long commentId, bool newRating)
        {
            string username = (string)Session["loggedInUserUsername"].ToString();
            CommentRating cr = commentRatingsRepository.GetCommentRating(commentId, username);
            if (cr != null)
            {
                return AlterExistingCommentRating(cr, newRating);
            }
            else
            {
                return CreateNewCommentRating(commentId, newRating);
            }
        }
        public JsonResult AlterExistingCommentRating(CommentRating cr, bool newRating)
        {
            string returnMessage = "";
            Comment comment = commentsRepository.GetCommentById((long)cr.CommentId);
            //true = like, false = dislike
            if (cr.IsLike)
            {
                if (newRating)
                {
                    comment.LikesCount -= 1;
                    returnMessage = "neutral";
                }
                else
                {
                    comment.LikesCount -= 1;
                    comment.DislikesCount += 1;
                    returnMessage = "dislike";
                }
            }
            else
            {
                if (newRating)
                {
                    comment.LikesCount += 1;
                    comment.DislikesCount -= 1;
                    returnMessage = "like";
                }
                else
                {
                    comment.DislikesCount -= 1;
                    returnMessage = "neutral";
                }
            }

            commentsRepository.UpdateComment(comment);

            cr.IsLike = newRating;
            if (returnMessage == "neutral")
            {
                commentRatingsRepository.DeleteCommentRating(cr.LikeID);
            }
            else
            {
                commentRatingsRepository.UpdateCommentRating(cr);
            }

            return Json(new { returnMessage, comment.LikesCount, comment.DislikesCount }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult CreateNewCommentRating(long commentId, bool newRating)
        {
            CommentRating cr = new CommentRating
            {
                LikeOwner = Session["loggedInUserUsername"].ToString(),
                CommentId = commentId,
                LikeDate = DateTime.Now,
                IsLike = newRating
            };
            commentRatingsRepository.CreateCommentRating(cr);
            Comment comment = commentsRepository.GetCommentById((long)cr.CommentId);
            if (newRating)
            {
                comment.LikesCount += 1;
            }
            else
            {
                comment.DislikesCount += 1;
            }
            commentsRepository.UpdateComment(comment);

            string returnMessage = (newRating == true) ? "like" : "dislike";

            return Json(new { returnMessage, comment.LikesCount, comment.DislikesCount }, JsonRequestBehavior.AllowGet);
        }
        // GET: CommentRatings
        public ActionResult Index()
        {
            var commentRatings = db.CommentRatings.Include(c => c.Comment).Include(c => c.User);
            return View(commentRatings.ToList());
        }

        // GET: CommentRatings/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CommentRating commentRating = db.CommentRatings.Find(id);
            if (commentRating == null)
            {
                return HttpNotFound();
            }
            return View(commentRating);
        }

        // GET: CommentRatings/Create
        public ActionResult Create()
        {
            ViewBag.CommentId = new SelectList(db.Comments, "CommentID", "CommentOwner");
            ViewBag.LikeOwner = new SelectList(db.Users, "Username", "Pass");
            return View();
        }

        // POST: CommentRatings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LikeID,LikeOwner,CommentId,IsLike,LikeDate,Deleted")] CommentRating commentRating)
        {
            if (ModelState.IsValid)
            {
                db.CommentRatings.Add(commentRating);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CommentId = new SelectList(db.Comments, "CommentID", "CommentOwner", commentRating.CommentId);
            ViewBag.LikeOwner = new SelectList(db.Users, "Username", "Pass", commentRating.LikeOwner);
            return View(commentRating);
        }

        // GET: CommentRatings/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CommentRating commentRating = db.CommentRatings.Find(id);
            if (commentRating == null)
            {
                return HttpNotFound();
            }
            ViewBag.CommentId = new SelectList(db.Comments, "CommentID", "CommentOwner", commentRating.CommentId);
            ViewBag.LikeOwner = new SelectList(db.Users, "Username", "Pass", commentRating.LikeOwner);
            return View(commentRating);
        }

        // POST: CommentRatings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LikeID,LikeOwner,CommentId,IsLike,LikeDate,Deleted")] CommentRating commentRating)
        {
            if (ModelState.IsValid)
            {
                db.Entry(commentRating).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CommentId = new SelectList(db.Comments, "CommentID", "CommentOwner", commentRating.CommentId);
            ViewBag.LikeOwner = new SelectList(db.Users, "Username", "Pass", commentRating.LikeOwner);
            return View(commentRating);
        }

        // GET: CommentRatings/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CommentRating commentRating = db.CommentRatings.Find(id);
            if (commentRating == null)
            {
                return HttpNotFound();
            }
            return View(commentRating);
        }

        // POST: CommentRatings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            CommentRating commentRating = db.CommentRatings.Find(id);
            db.CommentRatings.Remove(commentRating);
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
