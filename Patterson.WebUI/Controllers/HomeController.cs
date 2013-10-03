using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Mvc;
using Patterson.Domain.Abstract;
using Patterson.WebUI.Models;
using Patterson.WebUI.Helpers;

namespace Patterson.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private IUserRepository repository;
       

        public HomeController(IUserRepository repo)
        {
            repository = repo;
        }
        //
        // GET: /Home/
        [Authorize]
        public ViewResult Index()
        {
            var UserRoles = Roles.GetRolesForUser(User.Identity.Name);
            ViewData["Roles"] = new SelectList(UserRoles, Session["selectedRoles"]);
            var user = repository.Users.FirstOrDefault(u => u.userName == User.Identity.Name);
            return View(user);
        }

        //
        // Post: /Home/
        [AcceptVerbs(HttpVerbs.Post), Authorize]
        public ActionResult Index(FormCollection formValues)
        {
            var selected = formValues["RolesList"];
            Session["selectedRoles"] = selected;

            var UserRoles = Roles.GetRolesForUser(User.Identity.Name);
            ViewData["Roles"] = new SelectList(UserRoles, Session["selectedRoles"]);

            if (selected.Equals("admin"))
                return RedirectToAction("Index", "Admin");
            else 
            {
                
                return RedirectToAction("Index", "Home");
            }
            
        }

        public FileContentResult GetImage()
        {
            var use = repository.Users.FirstOrDefault(p => p.userName == User.Identity.Name);
            if (use != null)
            {
                if (use.ImageData == null)
                    return null;
                return File(use.ImageData, use.ImageMimeType);
            }
            else
            {
                return null;
            }

        }
    }
}
