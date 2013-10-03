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
    public class SkillsetManagementController : Controller
    {
        private IGroupRepository repository;


        public SkillsetManagementController(IGroupRepository GroupRepository)
        {
            repository = GroupRepository;
        }

        [Authorize(Roles = "admin, GroupManagement")]
        public ActionResult Index()
        {
            return View("Index");
        }

        [Authorize(Roles = "admin, GroupManagement")]
        public ActionResult CreateSkillset()
        {
            return View("CreateSkillset");
        }

        [Authorize(Roles = "admin, GroupManagement, UserManagement")]
        public ActionResult SkillsetList()
        {
            return View("SkillsetList");
        }

        [Authorize(Roles = "admin, GroupManagement, UserManagement")]
        public ActionResult UserList()
        {
            return View("UserList");
        }

        [Authorize(Roles = "admin, GroupManagement")]
        public ActionResult EditSkillset(int id)
        {
            var Skillset = repository.GetSkillset(id);
            return View("EditSkillset", Skillset);
        }

        // the page where you can add new users to the skillset
        [Authorize(Roles = "admin, GroupManagement, UserManagement")]
        public ActionResult AddSkillsetMember(int id)
        {
            Session["SkillsetID"] = id;
            string title = repository.GetSkillset(id).Title;
            Session["SkillsetTitle"] = title;
            return View("AddSkillsetMember");
        }



        [Authorize(Roles = "admin, GroupManagement, UserManagement")]
        public ActionResult DropSkillsetMember(int id)
        {
            int sid = (int)Session["SkillsetID"];

            int userID = id;

            SkillsetRosterEntry temp = new SkillsetRosterEntry { UserID = userID, SkillsetID = sid };
            repository.DropSkillsetRoster(temp);
            repository.SaveChanges();
            TempData["message"] = string.Format("User {0} has been dropped from skillset {1}.", temp.UserID, temp.SkillsetID);
            return RedirectToAction("AddSkillsetMember/" + sid.ToString());

        }

        //the actual methods that add members
        [AcceptVerbs(HttpVerbs.Post), Authorize(Roles = "admin, GroupManagement, UserManagement")]
        public ActionResult AddNewSkillsetMember(FormCollection formValues)
        {
            int sid = (int)Session["SkillsetID"];

            var uid = Int32.Parse(formValues["ID"]);

            // avoid duplicate entries
            SkillsetRosterEntry temp = new SkillsetRosterEntry { UserID = uid, SkillsetID = sid };
            if (repository.TestSkillsetRoster(temp) == null)
                repository.AddSkillsetRoster(uid, sid);
            TempData["message"] = string.Format("User {0} has been added to Skillset {1}.", temp.UserID, temp.SkillsetID);
            return RedirectToAction("AddSkillsetMember/" + sid.ToString());
        }

        //overloaded method
        [Authorize(Roles = "admin, GroupManagement, UserManagement")]
        public ActionResult AddNewSkillsetMemberByID(int id)
        {
            int sid = (int)Session["SkillsetID"];

            var uid = id;
            
            
               

            // avoid duplicate entries
            SkillsetRosterEntry temp = new SkillsetRosterEntry { UserID = uid, SkillsetID = sid };
            if (repository.TestSkillsetRoster(temp) == null)
            {
               var result = repository.AddSkillsetRoster(uid, sid);
               if (result > 0) 
                   TempData["message"] = string.Format("User {0} has been added to Skillset {1}.", temp.UserID, temp.SkillsetID);
               else
                   TempData["message"] = string.Format("Failed to add Skillset. Please verify that the ID number is correct and try again.");
               
            }
            return RedirectToAction("AddSkillsetMember/" + sid.ToString());
        }


        [Authorize(Roles = "admin, GroupManagement, UserManagement")]
        public ActionResult CreateSkillsetList(string sidx, string sord, int page, int rows)
        {
            var SkillsetList = repository.Skillsets.ToList();
            var qSkillsetList = SkillsetList.AsQueryable();

            var jsonData = new
            {
                total = 1,
                page = page,
                records = qSkillsetList.Count(),
                rows = (from n in qSkillsetList
                        select new
                        {
                            i = n.ID,
                            cell = new string[]{
                                 "<a href=\"AddSkillsetMember/" + n.ID.ToString() + "\">Edit Skillset Members</a>",
                                 "<a href=\"EditSkillset/" + n.ID.ToString() + "\">Edit Skillset Details</a>",
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
                                 "<a href=\"AddNewSkillsetMemberByID/" + n.id.ToString() + "\">Add to Skillset</a>"
                               }
                        }).ToArray()
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        [Authorize(Roles = "admin, GroupManagement, UserManagement")]
        public ActionResult CreateMemberList(string sidx, string sord, int page, int rows)
        {

            int sid = (int)Session["SkillsetID"];

            var SkillsetMembers = repository.GetSkillsetMembers(sid);
            var UserList = repository.GetSkillsetUsers(SkillsetMembers.ToList());
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
                                 "<a href=\"../DropSkillsetMember/" + n.id.ToString() + "\">Remove Skillset from user</a>",
                                n.id.ToString(),
                                n.userName.ToString()
                               }
                        }).ToArray()
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        [AcceptVerbs(HttpVerbs.Post), Authorize(Roles = "admin, GroupManagement")]
        public ActionResult CreateNewSkillset(FormCollection formValues)
        {
            var cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);

            var CreatorName = ticket.Name;
            var Creator = repository.Users.SingleOrDefault(u => u.userName == CreatorName);
            int CreatorID = Creator.id;

            var Identifier = formValues["Identifier"];
            var Title = formValues["SkillsetTitle"];
            var Description = formValues["Description"];

            var result = repository.AddSkillset(Identifier, Title, Description, CreatorID);
            if (result > 0)
                TempData["message"] = string.Format("Skillset {0} has been created.", Identifier);
            else
                TempData["message"] = string.Format("Unsuccessful procedure!");
            return View("Index");
        }

        [AcceptVerbs(HttpVerbs.Post), Authorize(Roles = "admin, GroupManagement")]
        public ActionResult Edit(Skillset skillset)
        {
            if (ModelState.IsValid)
            {

                var cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);
                var username = ticket.Name;
                var user = repository.Users.SingleOrDefault(u => u.userName == username);
                int userid = user.id;

                skillset.ModifiedBy = userid;
                skillset.ModifiedOn = DateTime.Now;
                skillset.ModifiedByName = username;

                var result = repository.EditSkillset(skillset);
                if (result > 0)
                    TempData["message"] = string.Format("Skillset {0} has been saved.", skillset.Identifier);
                else
                    TempData["message"] = string.Format("Unsuccessful procedure!");
            }
            return RedirectToAction("SkillsetList", "SkillsetManagement");
        }
    }
}
