using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Patterson.Domain.Abstract;
using Patterson.Domain.Entities;




namespace Patterson.WebUI.Controllers
{
    public class UserController : Controller
    {
        //
        // GET: /User/
        private IUserRepository repository;

        public UserController(IUserRepository repoParam)
        {
            repository = repoParam;
        }

        public ActionResult Index()
        {
            return View();
        }
        public FileContentResult GetImage(int? id)
        {
            User use = repository.Users.FirstOrDefault(p => p.id == id);
            if (use != null)
            {
                return File(use.ImageData, use.ImageMimeType);
            }
            else
            {
                return null;
            }
        }
    }
}
