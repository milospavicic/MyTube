using MyTube.App_Start;
using MyTube.DTO;
using MyTube.Helpers;
using MyTube.Models;
using MyTube.Repository;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyTube.Controllers
{
    [SessionFilter]
    public class UsersController : Controller
    {
        private IUsersRepository _usersRepository;
        public SelectList GetUserTypesAndSelect(string currentType)
        {
            using (var userTypesRepository = new UserTypesRepository(new MyTubeDBEntities()))
            {
                var userType = userTypesRepository.GetUserTypes();
                return new SelectList(userType, "TypeName", "TypeName", currentType);
            }
        }
        public UsersController(IUsersRepository usersRepository)
        {
            this._usersRepository = usersRepository;
        }
        public ActionResult IndexPageUsers()
        {
            var currentUserUsername = UsersHelper.LoggedInUserUsername(Session);
            var users = _usersRepository.GetNUsersWithout(6, currentUserUsername);
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
            if (_usersRepository.Login(user))
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
            User loggedInUser = _usersRepository.GetUserByUsername(username);
            var currentUser = new UserSessionModel
            {
                Username = loggedInUser.Username,
                UserType = loggedInUser.UserType,
                Blocked = loggedInUser.Blocked
            };
            Session.Add(UsersHelper.loggedInUser, currentUser);
        }
        public ActionResult OneUserForAdminPage(string id)
        {
            var user = _usersRepository.GetUserByUsername(id);
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
            if (UsersHelper.LoggedInUserUsername(Session) == null)
            {
                return null;
            }
            if (_usersRepository.GetUserByUsername(id) == null)
            {
                return null;
            }
            _usersRepository.BlockUser(id);
            ViewBag.Message = "User has been successfully blocked.";
            return PartialView("MessageModal");
        }

        [HttpPost]
        public ActionResult UnblockUser(string id)
        {
            if (UsersHelper.LoggedInUserUsername(Session) == null)
            {
                return null;
            }
            if (_usersRepository.GetUserByUsername(id) == null)
            {
                return null;
            }
            _usersRepository.UnblockUser(id);
            ViewBag.Message = "User has been successfully unblocked.";
            return PartialView("MessageModal");
        }

        [HttpPost]
        public ActionResult DeleteUser(string id)
        {
            if (UsersHelper.LoggedInUserUsername(Session) == null)
            {
                return null;
            }
            if (_usersRepository.GetUserByUsername(id) == null)
            {
                return null;
            }
            _usersRepository.DeleteUser(id);
            ViewBag.Message = "User has been successfully deleted.";
            return PartialView("MessageModal");
        }
        public ActionResult EditUserForm(string id)
        {
            EditUserModel userToEdit = EditUserModel.EditUserModalFromUser(_usersRepository.GetUserByUsername(id));
            ViewBag.UserType = GetUserTypesAndSelect(userToEdit.UserType);
            return PartialView("EditUserForm", userToEdit);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUserForm([Bind(Include = "Firstname,Lastname,UserType,Email,UserDescription")] EditUserModel eum, string id)
        {
            if (UsersHelper.LoggedInUserUsername(Session) == null)
            {
                return null;
            }

            if (ModelState.IsValid)
            {
                User editedUser = _usersRepository.GetUserByUsername(id);
                if (editedUser == null)
                {
                    return null;
                }
                editedUser = EditUserModel.UpdateUserFromEditUserModel(eum, editedUser);
                _usersRepository.UpdateUser(editedUser);
                ViewBag.Message = "User has been successfully edited.";
                return PartialView("MessageModal");
            }
            ViewBag.UserType = GetUserTypesAndSelect(eum.UserType);
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
            var userType = UsersHelper.LoggedInUserUserType(Session);
            var loggedInUserUsername = UsersHelper.LoggedInUserUsername(Session);
            if (loggedInUserUsername == username)
                return _usersRepository.GetAllUsersFollowedBy(username);
            else if (userType == "ADMIN")
                return _usersRepository.GetAllUsersFollowedBy(username);
            else
                return _usersRepository.GetAllAvaiableUsersFollowedBy(username);

        }
        public ActionResult ChannelPageInfoPartial(string id)
        {
            var user = _usersRepository.GetUserByUsername(id);
            var userDTO = UserDTO.ConvertUserToDTO(user);
            return PartialView(userDTO);
        }
        public JsonResult Subscribe(string id)
        {
            if (UsersHelper.LoggedInUserUsername(Session) == null)
            {
                return null;
            }
            var currentUser = UsersHelper.LoggedInUserUsername(Session);
            using (var subscribeRepository = new SubscribeRepository(new MyTubeDBEntities()))
            {
                bool exists = subscribeRepository.SubscriptionExists(id, currentUser);
                if (exists)
                {
                    subscribeRepository.DeleteSubscription(id, currentUser);

                }
                else
                {
                    subscribeRepository.NewSubscription(id, currentUser);

                }
                User userSubscribedTo = _usersRepository.GetUserByUsername(id);
                return Json(new { subStatus = exists, subCount = userSubscribedTo.SubscribersCount }, JsonRequestBehavior.AllowGet);
            }

        }


        [HttpPost]
        public ActionResult ChangePictureUpload(HttpPostedFileBase image, string username)
        {
            if (UsersHelper.LoggedInUserUsername(Session) == null)
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
                var user = _usersRepository.GetUserByUsername(username);
                if (user == null)
                {
                    return RedirectToAction("Error", "Home");
                }
                DeleteExistingPictures(username);
                user.ProfilePictureUrl = finalUrl;
                _usersRepository.UpdateUser(user);
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
            if (UsersHelper.LoggedInUserUsername(Session) == null)
            {
                return RedirectToAction("Error", "Home");
            }
            if (ProfilePictureUrl == null)
            {
                return null;
            }
            var user = _usersRepository.GetUserByUsername(username);
            if (user == null)
            {
                return RedirectToAction("Error", "Home");
            }
            user.ProfilePictureUrl = ProfilePictureUrl;
            _usersRepository.UpdateUser(user);

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
            if (UsersHelper.LoggedInUserUsername(Session) == null)
            {
                return null;
            }
            if (!ModelState.IsValid)
            {
                return PartialView(npm);
            }
            var user = _usersRepository.GetUserByUsername(id);
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
            _usersRepository.UpdateUser(user);
            ViewBag.Message = "Password has been successfully changed.";
            return PartialView("MessageModal");
        }
    }
}
