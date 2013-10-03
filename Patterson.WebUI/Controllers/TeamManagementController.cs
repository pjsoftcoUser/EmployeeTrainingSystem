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
    public class TeamManagementController : Controller
    {
        private IGroupRepository repository;


        public TeamManagementController(IGroupRepository TeamRepository)
        {
            repository = TeamRepository;
        }

        [Authorize(Roles = "admin, GroupManagement")]
        public ActionResult Index()
        {
            return View("Index");
        }

        [Authorize(Roles = "admin, GroupManagement")]
        public ActionResult CreateTeam()
        {
            return View("CreateTeam");
        }

        [Authorize(Roles = "admin, GroupManagement, UserManagement")]
        public ActionResult TeamList()
        {
            return View("TeamList");
        }

        [Authorize(Roles = "admin, GroupManagement, UserManagement")]
        public ActionResult UserList()
        {
            return View("UserList");
        }

        [Authorize(Roles = "admin, GroupManagement")]
        public ActionResult EditTeam(int id)
        {
            var Team = repository.GetTeam(id);
            return View("EditTeam", Team);
        }

        // the page where you can add new members
        [Authorize(Roles = "admin, GroupManagement, UserManagement")]
        public ActionResult AddTeamMember(int id)
        {
            Session["TeamID"] = id;
            string title = repository.GetTeam(id).Title;
            Session["TeamTitle"] = title;
            return View("AddTeamMember");
        }



        [Authorize(Roles = "admin, GroupManagement, UserManagement")]
        public ActionResult DropTeamMember(int id)
        {
            int tid = (int)Session["TeamID"];

            int userID = id;

            TeamRosterEntry temp = new TeamRosterEntry { UserID = userID, TeamID = tid };
            repository.DropTeamRoster(temp);
            repository.SaveChanges();
            TempData["message"] = string.Format("User {0} has been dropped from team {1}.", temp.UserID, temp.TeamID);
            return RedirectToAction("AddTeamMember/" + tid.ToString());

        }

        //the actual methods that add members
        [AcceptVerbs(HttpVerbs.Post), Authorize(Roles = "admin, GroupManagement, UserManagement")]
        public ActionResult AddNewTeamMember(FormCollection formValues)
        {
            int tid = (int)Session["TeamID"];

            var uid = Int32.Parse(formValues["ID"]);

            // avoid duplicate entries
            TeamRosterEntry temp = new TeamRosterEntry { UserID = uid, TeamID = tid };
            if (repository.TestTeamRoster(temp) == null)
                repository.AddTeamRoster(uid, tid);
            TempData["message"] = string.Format("User {0} has been added to team {1}.", temp.UserID, temp.TeamID);
            return RedirectToAction("AddTeamMember/" + tid.ToString());
        }

       
        [Authorize(Roles = "admin, GroupManagement, UserManagement")]
        public ActionResult AddNewTeamMemberByID(int id)
        {
            int tid = (int)Session["TeamID"];

            var uid = id;

            // avoid duplicate entries
            TeamRosterEntry temp = new TeamRosterEntry { UserID = uid, TeamID = tid };
            if (repository.TestTeamRoster(temp) == null)
                repository.AddTeamRoster(uid, tid);
            TempData["message"] = string.Format("User {0} has been added to team {1}.", temp.UserID, temp.TeamID);
            return RedirectToAction("AddTeamMember/" + tid.ToString());
        }


        [Authorize(Roles = "admin, GroupManagement, UserManagement")]
        public ActionResult CreateTeamList(string sidx, string sord, int page, int rows)
        {
            var TeamList = repository.Teams.ToList();
            var qTeamList = TeamList.AsQueryable();

            var jsonData = new
            {
                total = 1,
                page = page,
                records = qTeamList.Count(),
                rows = (from n in qTeamList
                        select new
                        {
                            i = n.TeamID,
                            cell = new string[]{
                                 "<a href=\"AddTeamMember/" + n.TeamID.ToString() + "\">Edit Team Members</a>",
                                 "<a href=\"EditTeam/" + n.TeamID.ToString() + "\">Edit Team Details</a>",
                                n.Identifier.ToString(),
                                n.Title.ToString(),
                                n.ManagerID.ToString(),
                                n.ManagerName.ToString(),
                                n.CreatedByName.ToString(),
                                n.CreatedOn.ToString("d"),
                                n.ModifiedOn.ToString("d")
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
                                 "<a href=\"AddNewTeamMemberByID/" + n.id.ToString() + "\">Add to Team</a>"
                               }
                        }).ToArray()
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        [Authorize(Roles = "admin, GroupManagement, UserManagement")]
        public ActionResult CreateMemberList(string sidx, string sord, int page, int rows)
        {

            int tid = (int)Session["TeamID"];

            var TeamMembers = repository.GetTeamMembers(tid);
            var UserList = repository.GetTeamUsers(TeamMembers.ToList());
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
                                 "<a href=\"../DropTeamMember/" + n.id.ToString() + "\">Remove Team Member</a>",
                                n.id.ToString(),
                                n.userName.ToString()
                               }
                        }).ToArray()
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        [AcceptVerbs(HttpVerbs.Post), Authorize(Roles = "admin, GroupManagement")]
        public ActionResult CreateNewTeam(FormCollection formValues)
        {
            var cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);

            var CreatorName = ticket.Name;
            var Creator = repository.Users.SingleOrDefault(u => u.userName == CreatorName);
            int CreatorID = Creator.id;

            var Identifier = formValues["Identifier"];
            var Title = formValues["TeamTitle"];
            var Description = formValues["Description"];
            var ManagerID = Int32.Parse(formValues["ManagerID"]);

            var Manager = repository.Users.SingleOrDefault(u => u.id == ManagerID);
            var ManagerName = Manager.userName;

            var result = repository.AddTeam(Identifier, Title, Description, ManagerID, CreatorID, CreatorID, CreatorName, ManagerName);
            if (result > 0)
                TempData["message"] = string.Format("Team {0} has been created.", Identifier);
            else
                TempData["message"] = string.Format("Unsuccessful procedure!");
            return View("Index");
        }

        [AcceptVerbs(HttpVerbs.Post), Authorize(Roles = "admin, GroupManagement")]
        public ActionResult Edit(Team team)
        {
            if (ModelState.IsValid)
            {

                var cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);
                var username = ticket.Name;
                var user = repository.Users.SingleOrDefault(u => u.userName == username);
                int userid = user.id;

                team.ModifiedByID = userid;
                team.ModifiedOn = DateTime.Now;
                var result = repository.EditTeam(team);
                if (result > 0)
                    TempData["message"] = string.Format("Team {0} has been saved.", team.Identifier);
                else
                    TempData["message"] = string.Format("Unsuccessful procedure!");
            }
            return RedirectToAction("TeamList", "TeamManagement");
        }
    }
}
