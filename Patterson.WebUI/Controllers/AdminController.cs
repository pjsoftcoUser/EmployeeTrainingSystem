using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Patterson.Domain.Abstract;
using Patterson.Domain.Entities;
using System.Web.Security;
using System.Web.UI.WebControls;
using System.Net.Mail;

namespace Patterson.WebUI.Controllers
{

    public class AdminController : Controller
    {
        private IUserRepository repository;
        private ISMTPRepository IsmtpRepository;
        string[] users = new string[1];
        string[] roles = new string[1];


        public AdminController(IUserRepository repo, ISMTPRepository IsmtpRepo)
        {
            repository = repo;
            IsmtpRepository = IsmtpRepo;
        }

        //
        //Get: /Home/
        [Authorize(Roles = "admin, UserManagement")]
        public ViewResult Index()
        {
            var UserRoles = Roles.GetRolesForUser(User.Identity.Name);
            ViewData["Roles"] = new SelectList(UserRoles, Session["selectedRoles"]);
            return View();

        }

        //Create User table
        public ActionResult UserTable(string sidx, string sord, int page, int rows)
        {
            List<User> userList = new List<User>();
            userList = repository.Users.ToList();

            var jsonData = new
            {
                total = 1,
                page = page,
                records = userList.Count(),

                rows = (from n in userList
                        select new
                        {
                            i = n.id,
                            cell = new string[]{
                                n.id.ToString(),
                                "<a href=\"/Admin/Edit/" + n.id.ToString() + "\">" + n.userName.ToString() + "</a>",
                                n.name.ToString(),
                                n.CreatedBy,
                                n.CreatedOn.ToString(),
                                n.ModifiedBy,
                                n.ModifiedOn.ToString()
                                //"<a href=\"/Admin/Delete/" + n.id.ToString() + "\">Delete User</a>"
                                 }
                        }).ToArray()
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);

        }

        
        //Create ASP pending User table
        public ActionResult AspUserTable(string sidx, string sord, int page, int rows)
        {
            var allAspUsers = Membership.GetAllUsers().Cast<MembershipUser>();
            List<MembershipUser> targetAspUsers = new List<MembershipUser>();
            List<User> userList = new List<User>();
            userList = repository.Users.ToList();
            foreach (var m in allAspUsers)
            {
                var registeredAspUser = userList.FirstOrDefault(u => u.userName == m.UserName);
                if (registeredAspUser == null)
                    targetAspUsers.Add(m);
            }
            var jsonData = new
            {
                total = 1,
                page = page,
                records = targetAspUsers.Count(),

                rows = (from n in targetAspUsers
                        select new
                        {
                            i = n.UserName,
                            cell = new string[]{
                                n.UserName.ToString(),
                                n.Email.ToString(),
                                n.IsOnline.ToString(),
                                n.LastActivityDate.ToString(),
                                "<a href=\"/Admin/Register" + "?user=" + n.UserName.ToString() + "\">Register</a>"
                                //"<a href=\"/Admin/Delete/" + n.id.ToString() + "\">Delete User</a>"
                                 }
                        }).ToArray()
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);

        }

        //
        // Post: /Home/
        [AcceptVerbs(HttpVerbs.Post), Authorize(Roles = "admin, UserManagement")]
        public ActionResult Index(FormCollection formValues)
        {
            var selected = formValues["RolesList"];
            Session["selectedRoles"] = selected;

            var UserRoles = Roles.GetRolesForUser(User.Identity.Name);
            ViewData["Roles"] = new SelectList(UserRoles, Session["selectedRoles"]);

            if (selected.Equals("admin"))
                return RedirectToAction("Index", "Admin");
            else
                return RedirectToAction("Index", "Home");
        }

        //
        //post: /Create/
        [Authorize(Roles = "admin, UserManagement")]
        public ViewResult Create()
        {
            var roles = Roles.GetAllRoles();
            List<string> roleList = new List<string>();
            foreach (var r in roles)
            {
                roleList.Add(r);
            }
            ViewData["Roles"] = roleList;

            return View("Edit", new User());
        }

        //Get: /Register/
        [Authorize(Roles = "admin, UserManagement")]
        public ActionResult Register(string user)
        {
            var curAspUser = Membership.GetAllUsers().Cast<MembershipUser>().FirstOrDefault(u => u.UserName == user);
            var roles = Roles.GetAllRoles();
            List<string> roleList = new List<string>();
            foreach (var r in roles)
            {
                roleList.Add(r);
            }
            ViewData["Roles"] = roleList;
            var userToRegister = new User();
            userToRegister.userName = curAspUser.UserName;
            userToRegister.email = curAspUser.Email;

            return View("RegiterInSystem", userToRegister);
        }
        //
        //Get: /Edit/
        [Authorize(Roles = "admin, UserManagement")]
        public ViewResult Edit(int? id)
        {
            User user = repository.Users.FirstOrDefault(p => p.id == id);
            var roles = Roles.GetAllRoles();
            var UserRoles = Roles.GetRolesForUser(user.userName);
            List<string> AllRoleList = new List<string>();
            List<string> userRoleList = new List<string>();
            foreach (var r in roles)
            {
                AllRoleList.Add(r);
            }
            foreach (var uR in UserRoles)
            {
                userRoleList.Add(uR);
            }
            ViewData["Roles"] = AllRoleList;
            ViewData["UserInRoles"] = userRoleList;

            return View(user);
        }

        [AcceptVerbs(HttpVerbs.Post), Authorize(Roles = "admin, UserManagement")]
        public ActionResult Delete(int? id)
        {
            User use = repository.Users.FirstOrDefault(p => p.id == id);
            if (use != null)
            {
                var result = repository.DeleteUser(use);
                if (result > 0)
                    TempData["message"] = string.Format("{0} was deleted", use.name);
                else
                    TempData["message"] = string.Format("Unsuccessful procedure!");
                
            }
            return RedirectToAction("Index");
        }

        [AcceptVerbs(HttpVerbs.Post), Authorize(Roles = "admin, UserManagement")]
        public ActionResult Edit(User user, HttpPostedFileBase image, FormCollection formValues)
        {
            var result = 0;
            if (ModelState.IsValid)
            {
                if (user.CreatedOn == null)
                {
                    user.CreatedBy = User.Identity.Name;
                    user.CreatedOn = DateTime.Now;
                }
                else
                {
                    user.ModifiedBy = User.Identity.Name;
                    user.ModifiedOn = DateTime.Now;
                }

                if (image != null)
                {
                    user.ImageMimeType = image.ContentType;
                    user.ImageData = new byte[image.ContentLength];
                    image.InputStream.Read(user.ImageData, 0, image.ContentLength);
                }

                if (formValues["admin"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[0].Equals("false") &&
                        formValues["instructor"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[0].Equals("false") &&
                        formValues["student"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[0].Equals("false") &&
                        formValues["ClassManagement"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[0].Equals("false") &&
                        formValues["CourseManagement"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[0].Equals("false") &&
                        formValues["GroupManagement"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[0].Equals("false") &&
                        formValues["SystemManagement"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[0].Equals("false") &&
                        formValues["TestManagement"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[0].Equals("false") &&
                        formValues["UserManagement"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[0].Equals("false"))
                    return View("Error");
                // Save the role in userInRole Table
                users[0] = user.userName;
                if (formValues["admin"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[0].Equals("true"))
                {
                    roles[0] = "admin";
                    // save the User
                    result = repository.SaveUser(user);
                    Roles.AddUsersToRoles(users, roles);
                }
                else
                {
                    roles[0] = "admin";
                    Roles.RemoveUsersFromRoles(users, roles);
                }
                if (formValues["instructor"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[0].Equals("true"))
                {
                    roles[0] = "instructor";
                    // save the User
                    result = repository.SaveUser(user);
                    Roles.AddUsersToRoles(users, roles);
                }
                else
                {
                    roles[0] = "instructor";
                    Roles.RemoveUsersFromRoles(users, roles);
                }
                if (formValues["student"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[0].Equals("true"))
                {
                    roles[0] = "student";
                    // save the User
                    result = repository.SaveUser(user);
                    Roles.AddUsersToRoles(users, roles);
                }
                else
                {
                    roles[0] = "student";
                    Roles.RemoveUsersFromRoles(users, roles);
                }
                if (formValues["ClassManagement"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[0].Equals("true"))
                {
                    roles[0] = "ClassManagement";
                    // save the User
                    result = repository.SaveUser(user);
                    Roles.AddUsersToRoles(users, roles);
                }
                else
                {
                    roles[0] = "ClassManagement";
                    Roles.RemoveUsersFromRoles(users, roles);
                }
                if (formValues["CourseManagement"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[0].Equals("true"))
                {
                    roles[0] = "CourseManagement";
                    // save the User
                    result = repository.SaveUser(user);
                    Roles.AddUsersToRoles(users, roles);
                }
                else
                {
                    roles[0] = "CourseManagement";
                    Roles.RemoveUsersFromRoles(users, roles);
                }
                if (formValues["GroupManagement"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[0].Equals("true"))
                {
                    roles[0] = "GroupManagement";
                    // save the User
                    result = repository.SaveUser(user);
                    Roles.AddUsersToRoles(users, roles);
                }
                else
                {
                    roles[0] = "GroupManagement";
                    Roles.RemoveUsersFromRoles(users, roles);
                }
                if (formValues["SystemManagement"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[0].Equals("true"))
                {
                    roles[0] = "SystemManagement";
                    // save the User
                    result = repository.SaveUser(user);
                    Roles.AddUsersToRoles(users, roles);
                }
                else
                {
                    roles[0] = "SystemManagement";
                    Roles.RemoveUsersFromRoles(users, roles);
                }
                if (formValues["TestManagement"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[0].Equals("true"))
                {
                    roles[0] = "TestManagement";
                    // save the User
                    result = repository.SaveUser(user);
                    Roles.AddUsersToRoles(users, roles);
                }
                else
                {
                    roles[0] = "TestManagement";
                    Roles.RemoveUsersFromRoles(users, roles);
                }
                if (formValues["UserManagement"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[0].Equals("true"))
                {
                    roles[0] = "UserManagement";
                    // save the User
                    result = repository.SaveUser(user);
                    Roles.AddUsersToRoles(users, roles);
                }
                else
                {
                    roles[0] = "UserManagement";
                    Roles.RemoveUsersFromRoles(users, roles);
                }


                // add a message to the viewbag
                if (result > 0)
                    TempData["message"] = string.Format("{0} has been saved", user.name);
                else
                    TempData["message"] = string.Format("Unsuccessful procedure!");
                
                // return the user to the list
                return RedirectToAction("Index");
            }
            else
            {
                // there is something wrong with the data values
                return View(user);
            }
        }
        
        [AcceptVerbs(HttpVerbs.Post), Authorize(Roles = "admin, UserManagement")]
        public ActionResult RegiterInSystem(User user, HttpPostedFileBase image, FormCollection formValues)
        {
            var result = 0;
            if (ModelState.IsValid)
            {
                if (user.CreatedOn == null)
                {
                    user.CreatedBy = User.Identity.Name;
                    user.CreatedOn = DateTime.Now;
                }
                else
                {
                    user.ModifiedBy = User.Identity.Name;
                    user.ModifiedOn = DateTime.Now;
                }

                if (image != null)
                {
                    user.ImageMimeType = image.ContentType;
                    user.ImageData = new byte[image.ContentLength];
                    image.InputStream.Read(user.ImageData, 0, image.ContentLength);
                }

                if (formValues["admin"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[0].Equals("false") &&
                        formValues["instructor"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[0].Equals("false") &&
                        formValues["student"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[0].Equals("false") &&
                        formValues["ClassManagement"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[0].Equals("false") &&
                        formValues["CourseManagement"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[0].Equals("false") &&
                        formValues["GroupManagement"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[0].Equals("false") &&
                        formValues["SystemManagement"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[0].Equals("false") &&
                        formValues["TestManagement"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[0].Equals("false") &&
                        formValues["UserManagement"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[0].Equals("false"))
                    return View("Error");
                // Save the role in userInRole Table
                users[0] = user.userName;
                if (formValues["admin"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[0].Equals("true"))
                {
                    roles[0] = "admin";
                    // save the User
                    result = repository.SaveUser(user);
                    Roles.AddUsersToRoles(users, roles);
                }
                else
                {
                    roles[0] = "admin";
                    Roles.RemoveUsersFromRoles(users, roles);
                }
                if (formValues["instructor"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[0].Equals("true"))
                {
                    roles[0] = "instructor";
                    // save the User
                    result = repository.SaveUser(user);
                    Roles.AddUsersToRoles(users, roles);
                }
                else
                {
                    roles[0] = "instructor";
                    Roles.RemoveUsersFromRoles(users, roles);
                }
                if (formValues["student"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[0].Equals("true"))
                {
                    roles[0] = "student";
                    // save the User
                    result = repository.SaveUser(user);
                    Roles.AddUsersToRoles(users, roles);
                }
                else
                {
                    roles[0] = "student";
                    Roles.RemoveUsersFromRoles(users, roles);
                }
                if (formValues["ClassManagement"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[0].Equals("true"))
                {
                    roles[0] = "ClassManagement";
                    // save the User
                    result = repository.SaveUser(user);
                    Roles.AddUsersToRoles(users, roles);
                }
                else
                {
                    roles[0] = "ClassManagement";
                    Roles.RemoveUsersFromRoles(users, roles);
                }
                if (formValues["CourseManagement"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[0].Equals("true"))
                {
                    roles[0] = "CourseManagement";
                    // save the User
                    result = repository.SaveUser(user);
                    Roles.AddUsersToRoles(users, roles);
                }
                else
                {
                    roles[0] = "CourseManagement";
                    Roles.RemoveUsersFromRoles(users, roles);
                }
                if (formValues["GroupManagement"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[0].Equals("true"))
                {
                    roles[0] = "GroupManagement";
                    // save the User
                    result = repository.SaveUser(user);
                    Roles.AddUsersToRoles(users, roles);
                }
                else
                {
                    roles[0] = "GroupManagement";
                    Roles.RemoveUsersFromRoles(users, roles);
                }
                if (formValues["SystemManagement"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[0].Equals("true"))
                {
                    roles[0] = "SystemManagement";
                    // save the User
                    result = repository.SaveUser(user);
                    Roles.AddUsersToRoles(users, roles);
                }
                else
                {
                    roles[0] = "SystemManagement";
                    Roles.RemoveUsersFromRoles(users, roles);
                }
                if (formValues["TestManagement"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[0].Equals("true"))
                {
                    roles[0] = "TestManagement";
                    // save the User
                    result = repository.SaveUser(user);
                    Roles.AddUsersToRoles(users, roles);
                }
                else
                {
                    roles[0] = "TestManagement";
                    Roles.RemoveUsersFromRoles(users, roles);
                }
                if (formValues["UserManagement"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[0].Equals("true"))
                {
                    roles[0] = "UserManagement";
                    // save the User
                    result = repository.SaveUser(user);
                    Roles.AddUsersToRoles(users, roles);
                }
                else
                {
                    roles[0] = "UserManagement";
                    Roles.RemoveUsersFromRoles(users, roles);
                }


                // add a message to the viewbag
                if (result > 0)
                {
                    TempData["message"] = string.Format("{0} has been saved", user.name);
                    var MySmtp = IsmtpRepository.Smtp.First();
                    // send notification email through gmail
                    // email address "pattersontrainingprogram@gmail.com"
                    // password "P@ttersonetsemail" (Patterson employee training system email)
                    System.Net.Mail.MailMessage email = new System.Net.Mail.MailMessage();
                    email.To.Add(user.email);
                    email.Subject = "Registeration on system";
                    var fromAddress = new MailAddress(MySmtp.user);
                    email.From = fromAddress;
                    email.Body = "This is an automated message from 'Patterson Employee Training System' to inform you that you have been registered on the system and you can logon since now.";

                    System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(MySmtp.server);
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new System.Net.NetworkCredential(fromAddress.Address, MySmtp.password);
                    smtp.EnableSsl = true;
                    smtp.Port = MySmtp.port;

                    smtp.Send(email);
                }
                else
                    TempData["message"] = string.Format("Unsuccessful procedure!");
                
                // return the user to the list
                return RedirectToAction("Index");
            }
            else
            {
                // there is something wrong with the data values
                return View(user);
            }
        }

    }
}
