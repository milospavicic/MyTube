using MyTube.Models;
using MyTube.Repository;
using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace MyTube.Controllers
{
    public class UsersController : Controller
    {
        private MyTubeDBEntities db = new MyTubeDBEntities();
        private UsersRepository usersRepository;
        private UserTypesRepository userTypesRepository;
        public UsersController()
        {
            this.usersRepository = new UsersRepository(new MyTubeDBEntities());
            this.userTypesRepository = new UserTypesRepository(new MyTubeDBEntities());
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
            Session.Add("loggedInUserUsername", username);
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
            return RedirectToAction("Login", "Account");
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
            if (Session["loggedInUserUsername"] != null)
            {
                var loggedInUser = usersRepository.GetUserByUsername(Session["loggedInUserUsername"].ToString());
                if (loggedInUser != null)
                {
                    ViewBag.CurrentUserUserType = loggedInUser.UserType;
                }
            }

            EditUserModel userToEdit = EditUserModel.EditUserModalFromUser(usersRepository.GetUserByUsername(id));
            ViewBag.UserType = userTypesRepository.GetUserTypesSelectListAndSelect(userToEdit.UserType);
            return PartialView("EditUserForm", userToEdit);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUserForm([Bind(Include = "Pass,Firstname,Lastname,UserType,Email,UserDescription,ProfilePictureUrl")] EditUserModel eum, string id)
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

            //var userToEdit = usersRepository.GetUserByUsername(id);
            //var tempUsername = userToEdit.Username;
            //ViewBag.UserType = userTypesRepository.GetUserTypesSelectListAndSelect(userToEdit.UserType);
            //return PartialView("EditUserForm", userToEdit);

        }
        public ActionResult SortAndSearchUsers(string sortOrder, string searchString)
        {
            if (!CheckIfPermited()) { return RedirectToAction("Index", "Home"); }

            ViewBag.SortOrder = String.IsNullOrEmpty(sortOrder) ? "username_asc" : "";
            ViewBag.SearchString = searchString;
            var users = usersRepository.GetUsers();
            if (!String.IsNullOrEmpty(searchString))
            {
                users = users.Where(s => s.Username.Contains(searchString)
                                       || s.Firstname.Contains(searchString)
                                       || s.Lastname.Contains(searchString)
                                       || s.Email.Contains(searchString)
                                       || s.UserType.Contains(searchString));
            }
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
            ViewBag.Values = Models.User.UsersSortOrder();
            return View("~/Views/Home/AdminPage.cshtml", users);
        }
        public bool CheckIfPermited()
        {
            if (Session["loggedInUserUsername"] == null)
            {
                return false;
            }
            else
            {
                var loggedInUser = usersRepository.GetUserByUsername(Session["loggedInUserUsername"].ToString());
                if (loggedInUser.UserType != "ADMIN")
                {
                    return false;
                }
            }
            return true;
        }
        //--------------------------------------------------------------------------------


        // GET: Users
        public ActionResult Index()
        {
            return View(usersRepository.GetUsers());
        }
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
