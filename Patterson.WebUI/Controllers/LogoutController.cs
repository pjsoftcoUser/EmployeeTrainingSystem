using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Patterson.WebUI.Controllers
{
    public class LogoutController : Controller
    {
        //
        // GET: /Logout/

        public ActionResult Index()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return View(TempData["selectedRoles"]);
        }

    }
}
