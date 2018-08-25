using MyTube.DTO;
using MyTube.Models;
using MyTube.Repository;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            CheckLoggedInUser();
        }

        private void CheckLoggedInUser()
        {
            if (Session == null)
            {
                return;
            }
            else
            {
                User loggedInUser = usersRepository.GetUserByUsername(Session["loggedInUserUsername"].ToString());
                if (loggedInUser == null)
                {
                    Session.Abandon();
                }
                else
                {
                    Session.Add("loggedInUserUsername", loggedInUser.Username);
                    Session.Add("loggedInUserUserType", loggedInUser.UserType);
                    Session.Add("loggedInUserStatus", loggedInUser.Blocked.ToString());
                }
            }
        }
        public ActionResult IndexPageUsers()
        {
            var currentUserUsername = (string)Session["loggedInUserUsername"];
            var users = usersRepository.GetNUsersWithout(6, currentUserUsername);
            users = users.OrderByDescending(x => x.SubscribersCount);
            var usersDTO = UserDTO.ConvertCollectionUserToDTO(users);

            return PartialView(usersDTO);
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
        public ActionResult OneUserForAdminPage(string id)
        {
            var user = usersRepository.GetUserByUsername(id);
            var userDTO = UserDTO.ConvertUserToDTO(user);
            return PartialView(userDTO);
        }
        public ActionResult Logout()
        {

            Session.Abandon();
            Response.ClearHeaders();
            Response.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate");
            Response.AddHeader("Pragma", "no-cache");
            Response.AddHeader("Expires", "0");
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult BlockUser(string id)
        {
            if (!LoggedInUserExists())
            {
                return null;
            }
            if (usersRepository.GetUserByUsername(id) == null)
            {
                return null;
            }
            usersRepository.BlockUser(id);
            ViewBag.Message = "User has been successfully blocked.";
            return PartialView("MessageModal");
        }

        [HttpPost]
        public ActionResult UnblockUser(string id)
        {
            if (!LoggedInUserExists())
            {
                return null;
            }
            if (usersRepository.GetUserByUsername(id) == null)
            {
                return null;
            }
            usersRepository.UnblockUser(id);
            ViewBag.Message = "User has been successfully unblocked.";
            return PartialView("MessageModal");
        }

        [HttpPost]
        public ActionResult DeleteUser(string id)
        {
            if (!LoggedInUserExists())
            {
                return null;
            }
            if (usersRepository.GetUserByUsername(id) == null)
            {
                return null;
            }
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
            if (!LoggedInUserExists())
            {
                return null;
            }

            if (ModelState.IsValid)
            {
                User editedUser = usersRepository.GetUserByUsername(id);
                if (editedUser == null)
                {
                    return null;
                }
                editedUser = EditUserModel.UpdateUserFromEditUserModel(eum, editedUser);
                usersRepository.UpdateUser(editedUser);
                ViewBag.Message = "User has been successfully edited.";
                return PartialView("MessageModal");
            }
            ViewBag.UserType = userTypesRepository.GetUserTypesSelectListAndSelect(eum.UserType);
            return PartialView(eum);

        }

        public ActionResult ChannelPageUsersPartial(string id)
        {
            var users = UsersFollowedBy(id);
            var usersDTO = UserDTO.ConvertCollectionUserToDTO(users);
            return PartialView(usersDTO);
        }

        public IEnumerable<User> UsersFollowedBy(string username)
        {
            var userType = (string)Session["loggedInUserUserType"];
            var loggedInUserUsername = (string)Session["loggedInUserUsername"];
            if (loggedInUserUsername == username)
                return usersRepository.GetAllUsersFollowedBy(username);
            else if (userType == "ADMIN")
                return usersRepository.GetAllUsersFollowedBy(username);
            else
                return usersRepository.GetAllAvaiableUsersFollowedBy(username);

        }
        public ActionResult ChannelPageInfoPartial(string id)
        {
            var user = usersRepository.GetUserByUsername(id);
            var userDTO = UserDTO.ConvertUserToDTO(user);
            return PartialView(userDTO);
        }
        public JsonResult Subscribe(string id)
        {
            if (!LoggedInUserExists())
            {
                return null;
            }
            User currentUser = usersRepository.GetUserByUsername((string)Session["loggedInUserUsername"]);

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
            if (!LoggedInUserExists())
            {
                return RedirectToAction("Error", "Home");
            }
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
                if (user == null)
                {
                    return RedirectToAction("Error", "Home");
                }
                DeleteExistingPictures(username);
                user.ProfilePictureUrl = finalUrl;
                usersRepository.UpdateUser(user);
                image.SaveAs(path);
            }
            return Redirect(Request.UrlReferrer.ToString());
        }
        public void DeleteExistingPictures(string username)
        {
            var path = Path.Combine(Server.MapPath("~/Pictures/users"));
            string[] fileList = Directory.GetFiles(path);
            foreach (string file in fileList)
            {
                string[] subStrings = file.Split('\\');
                string fileName = subStrings[subStrings.Count() - 1];
                if (fileName.ToUpper().Contains(username.ToUpper()))
                {
                    System.IO.File.Delete(file);
                    return;
                }
            }
        }
        [HttpPost]
        public ActionResult ChangePictureUrl(string username, string ProfilePictureUrl)
        {
            if (!LoggedInUserExists())
            {
                return RedirectToAction("Error", "Home");
            }
            if (ProfilePictureUrl == null)
            {
                return null;
            }
            var user = usersRepository.GetUserByUsername(username);
            if (user == null)
            {
                return RedirectToAction("Error", "Home");
            }
            user.ProfilePictureUrl = ProfilePictureUrl;
            usersRepository.UpdateUser(user);

            return Redirect(Request.UrlReferrer.ToString());
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
            if (!LoggedInUserExists())
            {
                return null;
            }
            if (!ModelState.IsValid)
            {
                return PartialView(npm);
            }
            var user = usersRepository.GetUserByUsername(id);
            if (user == null)
            {
                return null;
            }
            if (npm.OldPassword != user.Pass)
            {
                ViewBag.Message = "Old password is incorrect";
                return PartialView(npm);
            }
            if (npm.NewPassword == npm.OldPassword)
            {
                ViewBag.Message = "New password can't be same as old password.";
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public bool LoggedInUserExists()
        {
            if (Session["loggedInUserStatus"] != null)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
    }
}
