using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Recepies.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Recepies.Controllers
{
    public class StatsController : Controller
    {
        private DbContext db = new DbContext(); 

        // GET: Stats
        public ActionResult Stats()
        {
            if (((User)HttpContext.Session["user"]) != null)
            {
                var difficulties = db.Recipies.GroupBy(r => r.Difficulty).Select(g => new { difficulty = ((Difficulty)g.Key).ToString(), count = g.Count() }).ToList();
                var difficultiesData = JsonConvert.SerializeObject(difficulties);
                ViewBag.difficulties = difficultiesData;

                var ingredients = db.Ingredients.Select(i => new { name = i.Name, calories = i.Calories });
                var ingredientsData = JsonConvert.SerializeObject(ingredients);
                ViewBag.ingredients = ingredientsData;

                return View();
            }

            return RedirectToAction("Index", "Error");
        }

      
    }
}