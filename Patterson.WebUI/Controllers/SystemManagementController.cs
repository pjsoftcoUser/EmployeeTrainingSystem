using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Patterson.Domain.Abstract;
using Patterson.Domain.Entities;

namespace Patterson.WebUI.Controllers
{
    public class SystemManagementController : Controller
    {
        private ISMTPRepository repository;

        public SystemManagementController(ISMTPRepository repo)
        {
            repository = repo;
        }

        //
        // GET: /SystemManagement/
        [Authorize(Roles = "admin, SystemManagement")]
        public ViewResult Index()
        {
            var smtp = repository.Smtp.First();
            return View(smtp);
        }

        [AcceptVerbs(HttpVerbs.Post), Authorize(Roles = "admin, SystemManagement")]
        public ActionResult Index(SMTP smtp)
        {
            var result = repository.SaveSMTP(smtp);
            if (result > 0)
                TempData["message"] = string.Format("System SMTP has been saved");
            else
                TempData["message"] = string.Format("Unsuccessful procedure!");

            return RedirectToAction("Index");
        }

    }
}
