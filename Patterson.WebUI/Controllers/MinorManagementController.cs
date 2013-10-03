using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Patterson.Domain.Abstract;
using System.Web.Security;
using Patterson.Domain.Entities;
using Patterson.WebUI.Models;


namespace Patterson.WebUI.Controllers
{
    public class MinorManagementController : Controller
    {
        private IGroupRepository repository;


        public MinorManagementController(IGroupRepository GroupRepository)
        {
            repository = GroupRepository;
        }

        [Authorize(Roles = "admin, GroupManagement")]
        public ActionResult Index()
        {
            return View("Index");
        }

        [Authorize(Roles = "admin, GroupManagement")]
        public ActionResult CreateMinor()
        {
            return View("CreateMinor");
        }

        [Authorize(Roles = "admin, GroupManagement, UserManagement")]
        public ActionResult MinorList()
        {
            return View("MinorList");
        }

        [Authorize(Roles = "admin, GroupManagement, UserManagement")]
        public ActionResult UserList()
        {
            return View("UserList");
        }

        [Authorize(Roles = "admin, GroupManagement")]
        public ActionResult EditMinor(int id)
        {
            var Minor = repository.GetMinor(id);
            return View("EditMinor", Minor);
        }

        // the page where you can add new users to the minor
        [Authorize(Roles = "admin, GroupManagement, UserManagement")]
        public ActionResult AddMinorMember(int id)
        {
            Session["MinorID"] = id;
            string title = repository.GetMinor(id).Title;
            Session["MinorTitle"] = title;
            return View("AddMinorMember");
        }



        [Authorize(Roles = "admin, GroupManagement, UserManagement")]
        public ActionResult DropMinorMember(int id)
        {
            int mid = (int)Session["MinorID"];

            int userID = id;

            MinorRosterEntry temp = new MinorRosterEntry { UserID = userID, MinorID = mid };
            repository.DropMinorRoster(temp);
            repository.SaveChanges();
            TempData["message"] = string.Format("User {0} has been dropped from minor {1}.", temp.UserID, temp.MinorID);
            return RedirectToAction("AddMinorMember/" + mid.ToString());

        }

        //the actual methods that add members
        [AcceptVerbs(HttpVerbs.Post), Authorize(Roles = "admin, GroupManagement, UserManagement")]
        public ActionResult AddNewMinorMember(FormCollection formValues)
        {
            int mid = (int)Session["MinorID"];

            var uid = Int32.Parse(formValues["ID"]);

            // avoid duplicate entries
            MinorRosterEntry temp = new MinorRosterEntry { UserID = uid, MinorID = mid };
            if (repository.TestMinorRoster(temp) == null)
                repository.AddMinorRoster(uid, mid);
            TempData["message"] = string.Format("User {0} has been added to minor {1}.", temp.UserID, temp.MinorID);
            return RedirectToAction("AddMinorMember/" + mid.ToString());
        }

        //overloaded method
        [Authorize(Roles = "admin, GroupManagement, UserManagement")]
        public ActionResult AddNewMinorMemberByID(int id)
        {
            int mid = (int)Session["MinorID"];

            var uid = id;




            // avoid duplicate entries
            MinorRosterEntry temp = new MinorRosterEntry { UserID = uid, MinorID = mid };
            if (repository.TestMinorRoster(temp) == null)
            {
                repository.AddMinorRoster(uid, mid);
                //if (result > 0)
                    TempData["message"] = string.Format("User {0} has been added to Minor {1}.", temp.UserID, temp.MinorID);
                // else
                //    TempData["message"] = string.Format("Failed to add Minor. Please verify that the ID number is correct and try again.");

            }
            return RedirectToAction("AddMinorMember/" + mid.ToString());
        }


        [Authorize(Roles = "admin, GroupManagement, UserManagement")]
        public ActionResult CreateMinorList(string sidx, string sord, int page, int rows)
        {
            var MinorList = repository.Minors.ToList();
            var qMinorList = MinorList.AsQueryable();

            var jsonData = new
            {
                total = 1,
                page = page,
                records = qMinorList.Count(),
                rows = (from n in qMinorList
                        select new
                        {
                            i = n.ID,
                            cell = new string[]{
                                 "<a href=\"AddMinorMember/" + n.ID.ToString() + "\">Edit Minor Members</a>",
                                 "<a href=\"EditMinor/" + n.ID.ToString() + "\">Edit Minor Details</a>",
                                n.Identifier.ToString(),
                                n.Title.ToString(),
                                n.CreatedByName,
                                n.CreatedOn.ToString(),
                                n.ModifiedByName,
                                n.ModifiedOn.ToString()
                               }
                        }).ToArray()
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "admin, GroupManagement, UserManagement")]
        public ActionResult CreateUserList(string sidx, string sord, int page, int rows)
        {
            var users = repository.Users.ToList();
            var qUsers = users.AsQueryable();

            var jsonData = new
            {
                total = 1,
                page = page,
                records = qUsers.Count(),
                rows = (from n in qUsers
                        select new
                        {
                            i = n.id,
                            cell = new string[]{
                                n.id.ToString(),
                                n.userName,
                                 "<a href=\"AddNewMinorMemberByID/" + n.id.ToString() + "\">Add to Minor</a>"
                               }
                        }).ToArray()
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        [Authorize(Roles = "admin, GroupManagement, UserManagement")]
        public ActionResult CreateMemberList(string sidx, string sord, int page, int rows)
        {

            int mid = (int)Session["MinorID"];

            var MinorMembers = repository.GetMinorMembers(mid);
            var UserList = repository.GetMinorUsers(MinorMembers.ToList());
            var qUserList = UserList.AsQueryable();

            var jsonData = new
            {
                total = 1,
                page = page,
                records = qUserList.Count(),
                rows = (from n in qUserList
                        select new
                        {
                            i = n.id,
                            cell = new string[]{
                                 "<a href=\"../DropMinorMember/" + n.id.ToString() + "\">Remove Minor from user</a>",
                                n.id.ToString(),
                                n.userName.ToString()
                               }
                        }).ToArray()
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        [AcceptVerbs(HttpVerbs.Post), Authorize(Roles = "admin, GroupManagement")]
        public ActionResult CreateNewMinor(FormCollection formValues)
        {
            var cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);

            var CreatorName = ticket.Name;
            var Creator = repository.Users.SingleOrDefault(u => u.userName == CreatorName);
            int CreatorID = Creator.id;

            var Identifier = formValues["Identifier"];
            var Title = formValues["MinorTitle"];
            var Description = formValues["Description"];

            var result = repository.AddMinor(Identifier, Title, Description, CreatorID);
            if (result > 0)
                TempData["message"] = string.Format("Minor {0} has been created.", Identifier);
            else
                TempData["message"] = string.Format("Unsuccessful procedure!");
            return View("Index");
        }

        [AcceptVerbs(HttpVerbs.Post), Authorize(Roles = "admin, GroupManagement")]
        public ActionResult Edit(Minor minor)
        {
            if (ModelState.IsValid)
            {

                var cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);
                var username = ticket.Name;
                var user = repository.Users.SingleOrDefault(u => u.userName == username);
                int userid = user.id;

                minor.ModifiedBy = userid;
                minor.ModifiedOn = DateTime.Now;
                minor.ModifiedByName = username;

                var result = repository.EditMinor(minor);
                if (result > 0)
                    TempData["message"] = string.Format("Minor {0} has been saved.", minor.Identifier);
                else
                    TempData["message"] = string.Format("Unsuccessful procedure!");
            }
            return RedirectToAction("MinorList", "MinorManagement");
        }
    }
}
