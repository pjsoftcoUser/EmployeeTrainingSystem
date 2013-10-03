using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls;
using Patterson.Domain.Abstract;
using Patterson.Domain.Entities;

namespace Patterson.WebUI.Controllers
{
    public class ProfileController : Controller
    {
        private IUserRepository repository;

        public ProfileController(IUserRepository repo)
        {
            repository = repo;
        }

        //
        // GET: /Profile/
        [Authorize]
        public ActionResult Index()
        {
            User user = repository.Users.FirstOrDefault(p => p.userName == User.Identity.Name);
            if (user == null)
                return View("Error");
            return View(user);
        }

        [AcceptVerbs(HttpVerbs.Post), Authorize]
        public ActionResult Index(User user, HttpPostedFileBase image, FormCollection formValues)
        {

            if (!Roles.RoleExists(user.userName))
                return View("Error");
            else
            {
                
                    if (image != null)
                    {
                        user.ImageMimeType = image.ContentType;
                        user.ImageData = new byte[image.ContentLength];
                        image.InputStream.Read(user.ImageData, 0, image.ContentLength);
                    }
                    // save the User
                    var result = repository.SaveUser(user);
                    if (result > 0)
                    // add a message to the viewbag
                        TempData["message"] = string.Format("{0} has been saved", user.name);
                    else
                        TempData["message"] = string.Format("Unsuccessful procedure");
                    // return the user to the list
                    return RedirectToAction("Index", "Home");
                
            }
        }
    }
}
