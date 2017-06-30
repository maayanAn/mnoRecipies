using Recepies.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Recepies.Controllers
{
    public class AboutSiteController : Controller
    {
        // GET: AboutSite
        public ActionResult Index()
        {
            var currentUser = ((User)HttpContext.Session["user"]);
            if (currentUser != null)
            {
                return View();
            }
            return RedirectToAction("Index", "Error");            
        }

        public string GetAboutSiteText()
        {
            var text = System.IO.File.ReadAllText(Server.MapPath("\\Content\\TextFiles\\AboutSite.txt"));
            return text;
        }
    }
}