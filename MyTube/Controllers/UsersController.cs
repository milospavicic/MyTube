using MyTube.Models;
using MyTube.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MyTube.Controllers
{
    public class UsersController : Controller
    {
        private MyTubeDBEntities db = new MyTubeDBEntities();
        private UsersRepository usersRepository;
        private UserTypesRepository userTypesRepository;
        private SubscribeRepository subscribeRepository;
        public UsersController()
        {
            this.usersRepository = new UsersRepository(new MyTubeDBEntities());
            this.userTypesRepository = new UserTypesRepository(new MyTubeDBEntities());
            this.subscribeRepository = new SubscribeRepository(new MyTubeDBEntities());
        }
        public JsonResult IndexPageChannels()
        {
            var currentUserUsername = (string)Session["loggedInUserUsername"];
            var channels = usersRepository.GetNRandomUsers(6, currentUserUsername);

            return Json(channels, JsonRequestBehavior.AllowGet);
        }


        [ChildActionOnly]
        public ActionResult LoginModal()
        {
            return PartialView("LoginForm");
        }
        public ActionResult Login(User user)
        {
            if (usersRepository.Login(user))
            {
                StoreInSession(user.Username);
                return PartialView("LoginSuccess");
            }
            CheckIfLoginModelEmpty(user);

            return PartialView("LoginForm", user);
        }
        public void CheckIfLoginModelEmpty(User user)
        {
            if (user.Username != null && user.Username != null)
            {
                ViewBag.Message = "Incorrect username or password.";
            }

        }
        public void StoreInSession(string username)
        {
            User loggedInUser = usersRepository.GetUserByUsername(username);
            Session.Add("loggedInUserUsername", loggedInUser.Username);
            Session.Add("loggedInUserUserType", loggedInUser.UserType);
            Session.Add("loggedInUserStatus", loggedInUser.Blocked.ToString());
        }
        public ActionResult Register()
        {
            ViewBag.UserType = userTypesRepository.GetUserTypesSelectList();
            return View();
        }
        [HttpPost]
        public ActionResult Register(User user)
        {
            bool usernameTaken = usersRepository.UsernameTaken(user.Username);
            if (usernameTaken)
            {
                ViewBag.Message = "Username taken";
                ViewBag.UserType = userTypesRepository.GetUserTypesSelectList();
                return View();
            }
            usersRepository.InsertUser(user);
            return RedirectToAction("Index", "Home");
        }
        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult BlockUser(string id)
        {
            usersRepository.BlockUser(id);
            ViewBag.Message = "User has been successfully blocked.";
            return PartialView("MessageModal");
        }

        [HttpPost]
        public ActionResult UnblockUser(string id)
        {
            usersRepository.UnblockUser(id);
            ViewBag.Message = "User has been successfully unblocked.";
            return PartialView("MessageModal");
        }

        [HttpPost]
        public ActionResult DeleteUser(string id)
        {
            usersRepository.DeleteUser(id);
            ViewBag.Message = "User has been successfully deleted.";
            return PartialView("MessageModal");
        }
        public ActionResult EditUserForm(string id)
        {
            EditUserModel userToEdit = EditUserModel.EditUserModalFromUser(usersRepository.GetUserByUsername(id));
            ViewBag.UserType = userTypesRepository.GetUserTypesSelectListAndSelect(userToEdit.UserType);
            return PartialView("EditUserForm", userToEdit);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUserForm([Bind(Include = "Firstname,Lastname,UserType,Email,UserDescription")] EditUserModel eum, string id)
        {

            if (ModelState.IsValid)
            {
                User editedUser = usersRepository.GetUserByUsername(id);
                editedUser = EditUserModel.UpdateUserFromEditUserModel(eum, editedUser);
                usersRepository.UpdateUser(editedUser);
                ViewBag.Message = "User has been successfully edited.";
                return PartialView("MessageModal");
            }
            ViewBag.UserType = userTypesRepository.GetUserTypesSelectListAndSelect(eum.UserType);
            return PartialView(eum);

        }
        public ActionResult AdminPage(string sortOrder, string searchString)
        {
            if (!CheckIfPermited()) { return RedirectToAction("Index", "Home"); }

            ViewBag.SortOrder = String.IsNullOrEmpty(sortOrder) ? "username_asc" : "";
            ViewBag.SearchString = searchString;

            var users = usersRepository.SearchUsers(searchString);
            users = SortUsers(users, sortOrder);

            ViewBag.Values = Models.User.UsersSortOrderSelectList();
            return View("~/Views/Home/AdminPage.cshtml", users);
        }
        public bool CheckIfPermited()
        {
            if (Session["loggedInUserUsername"] == null || Session["loggedInUserUserType"].ToString() != "ADMIN")
            {
                return false;
            }
            return true;
        }
        public IEnumerable<User> SortUsers(IEnumerable<User> users, string sortOrder)
        {
            switch (sortOrder)
            {
                case "username_asc":
                    users = users.OrderBy(s => s.Username);
                    break;
                case "username_desc":
                    users = users.OrderByDescending(s => s.Username);
                    break;
                case "firstname_asc":
                    users = users.OrderBy(s => s.Firstname);
                    break;
                case "firstname_desc":
                    users = users.OrderByDescending(s => s.Firstname);
                    break;
                case "lastname_asc":
                    users = users.OrderBy(s => s.Lastname);
                    break;
                case "lastname_desc":
                    users = users.OrderByDescending(s => s.Lastname);
                    break;
                case "email_asc":
                    users = users.OrderBy(s => s.Email);
                    break;
                case "email_desc":
                    users = users.OrderByDescending(s => s.Email);
                    break;
                case "user_type_asc":
                    users = users.OrderBy(s => s.UserType);
                    break;
                case "user_type_desc":
                    users = users.OrderByDescending(s => s.UserType);
                    break;
                case "status_asc":
                    users = users.OrderBy(s => s.Blocked);
                    break;
                case "status_desc":
                    users = users.OrderByDescending(s => s.Blocked);
                    break;

                default:
                    users = users.OrderBy(s => s.Username);
                    break;
            }
            return users;
        }
        public ActionResult Index()
        {
            return View(usersRepository.GetUsers());
        }

        // GET: Users
        public ActionResult ChannelPageUsersPartial(string id)
        {
            return PartialView(UsersFollowedBy(id));
        }
        public IEnumerable<User> UsersFollowedBy(string username)
        {
            var userType = (string)Session["loggedInUserUserType"];
            if (userType == "ADMIN")
                return usersRepository.GetAllUsersFollowedBy(username);
            else
                return usersRepository.GetAllAvaiableUsersFollowedBy(username);

        }
        public ActionResult ChannelPageInfoPartial(string id)
        {
            return PartialView(usersRepository.GetUserByUsername(id));
        }
        public JsonResult Subscribe(string id)
        {
            User currentUser = usersRepository.GetUserByUsername(Session["loggedInUserUsername"].ToString());


            bool exists = subscribeRepository.SubscriptionExists(id, currentUser.Username);
            if (exists)
            {
                subscribeRepository.DeleteSubscription(id, currentUser.Username);

            }
            else
            {
                subscribeRepository.NewSubscription(id, currentUser.Username);

            }
            User userSubscribedTo = usersRepository.GetUserByUsername(id);
            return Json(new { subStatus = exists, subCount = userSubscribedTo.SubscribersCount }, JsonRequestBehavior.AllowGet);

        }


        [HttpPost]
        public ActionResult ChangePictureUpload(HttpPostedFileBase image, string username)
        {
            if (image == null)
            {
                return null;
            }
            if (image.ContentLength > 0)
            {
                var extension = Path.GetExtension(image.FileName);
                var path = Path.Combine(Server.MapPath("~/Pictures/users"), username + extension);
                var finalUrl = "/Pictures/users/" + username + extension;
                var user = usersRepository.GetUserByUsername(username);
                user.ProfilePictureUrl = finalUrl;
                usersRepository.UpdateUser(user);
                image.SaveAs(path);
            }
            return RedirectToAction("ChannelPage/" + username, "Home");
        }
        [HttpPost]
        public ActionResult ChangePictureUrl(string username, string ProfilePictureUrl)
        {
            if (ProfilePictureUrl == null)
            {
                return null;
            }
            var user = usersRepository.GetUserByUsername(username);
            user.ProfilePictureUrl = ProfilePictureUrl;
            usersRepository.UpdateUser(user);
            return RedirectToAction("ChannelPage/" + username, "Home");

        }
        public ActionResult NewPassword()
        {
            NewPasswordModel npm = new NewPasswordModel();
            return PartialView(npm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewPassword([Bind(Include = "OldPassword,NewPassword,ConfirmNewPassword")] NewPasswordModel npm, string id)
        {
            if (!ModelState.IsValid)
            {
                return PartialView(npm);
            }
            var user = usersRepository.GetUserByUsername(id);
            if (npm.OldPassword != user.Pass)
            {
                ViewBag.Message = "Old password is incorrect";
                return PartialView(npm);
            }
            if (npm.NewPassword != npm.ConfirmNewPassword)
            {
                ViewBag.Message = "New passwords don't match.";
                return PartialView(npm);
            }
            user.Pass = npm.NewPassword;
            usersRepository.UpdateUser(user);
            ViewBag.Message = "Password has been successfully changed.";
            return PartialView("MessageModal");
        }

        //--------------------------------------------------------------------------------
        // GET: Users/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = usersRepository.GetUserByUsername(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            ViewBag.UserType = userTypesRepository.GetUserTypesSelectList();
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Username,Pass,Firstname,Lastname,UserType,Email,UserDescription,RegistrationDate,Blocked,Deleted,ProfilePictureUrl")] User user)
        {
            if (ModelState.IsValid)
            {
                usersRepository.InsertUser(user);
                return RedirectToAction("Index");
            }
            ViewBag.UserType = userTypesRepository.GetUserTypesSelectListAndSelect(user.UserType);
            return View(user);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = usersRepository.GetUserByUsername(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserType = userTypesRepository.GetUserTypesSelectListAndSelect(user.UserType);
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Username,Pass,Firstname,Lastname,UserType,Email,UserDescription,RegistrationDate,Blocked,Deleted,ProfilePictureUrl")] User user)
        {
            if (ModelState.IsValid)
            {
                usersRepository.UpdateUser(user);
                return RedirectToAction("Index");
            }
            ViewBag.UserType = userTypesRepository.GetUserTypesSelectListAndSelect(user.UserType);
            return View(user);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = usersRepository.GetUserByUsername(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            usersRepository.DeleteUser(id);
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
