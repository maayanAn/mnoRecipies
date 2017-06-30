using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Recepies.Models;
using DbContext = Recepies.Models.DbContext;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace Recepies.Controllers
{
    public class IngredientsController : Controller
    {
        private DbContext db = new DbContext();

        // GET: Ingredients
        public ActionResult Index()
        {
            if (((User)HttpContext.Session["user"]) != null)
            {
                return View(db.Ingredients.ToList());
            }
            return RedirectToAction("Index", "Error");

        }

        // GET: Ingredients/Search
        public ActionResult Search(string caloriesFrom = null, string caloriesTo = null)
        {
            if (((User)HttpContext.Session["user"]) != null)
            {
                var returnDataQurey = db.Ingredients.Select(i => i);

                if (!string.IsNullOrEmpty(caloriesFrom))
                {
                    var caloriesFromNumber = double.Parse(caloriesFrom);
                    returnDataQurey = returnDataQurey.Where(i => i.Calories >= caloriesFromNumber);
                }

                if (!string.IsNullOrEmpty(caloriesTo))
                {
                    var caloriesToNumber = double.Parse(caloriesTo);
                    returnDataQurey = returnDataQurey.Where(i => i.Calories <= caloriesToNumber);
                }

                var result = returnDataQurey.GroupBy(i => i.Calories).ToDictionary(x => x.Key, y => y.ToList());
                return View(result);
            }
            return RedirectToAction("Index", "Error");
        }

        // GET: Ingredients/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Error");
            }
            Ingredient ingredient = db.Ingredients.Find(id);
            if (ingredient == null)
            {
                return HttpNotFound();
            }
            return View(ingredient);
        }

        // GET: Ingredients/Create
        public ActionResult Create()
        {
            var curUser = ((User)HttpContext.Session["user"]);
            if (curUser != null)
            {
                if (curUser.IsManager)
                {
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Error", new { message = "you are not authorized" });
                }
            }
            return RedirectToAction("Index", "Error");
        }

        // POST: Ingredients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] Ingredient ingredient)
        {
            if (ModelState.IsValid)
            {
                var newIngredient = GetMetadata(ingredient);
                db.Ingredients.Add(newIngredient);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(ingredient);
        }

        // GET: Ingredients/Edit/5
        public ActionResult Edit(int? id)
        {
            var curUser = ((User)HttpContext.Session["user"]);
            if (curUser != null)
            {
                if (curUser.IsManager)
                {
                    if (id == null)
                    {
                        return RedirectToAction("Index", "Error");
                    }
                    Ingredient ingredient = db.Ingredients.Find(id);
                    if (ingredient == null)
                    {
                        return HttpNotFound();
                    }
                    return View(ingredient);
                }
                else
                {
                    return RedirectToAction("Index", "Error", new { message = "you are not authorized" });
                }
            }
            else
            {
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Ingredients/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Calories,Protein,sugar,Iron")] Ingredient ingredient)
        {
            var curUser = ((User)HttpContext.Session["user"]);
            if (curUser != null)
            {
                if (curUser.IsManager)
                {
                    if (ModelState.IsValid)
                    {
                        db.Entry(ingredient).State = EntityState.Modified;
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    return View(ingredient);
                }
                else
                {
                    return RedirectToAction("Index", "Error", new { message = "you are not authorized" });
                }
            }
            else
            {
                return RedirectToAction("Index", "Error");
            }
        }

        // GET: Ingredients/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Error");
            }
            Ingredient ingredient = db.Ingredients.Find(id);
            if (ingredient == null)
            {
                return HttpNotFound();
            }
            return View(ingredient);
        }

        // POST: Ingredients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ingredient ingredient = db.Ingredients.Find(id);
            try
            {
                db.Ingredients.Remove(ingredient);
                db.SaveChanges();
            }
            catch
            {
                return RedirectToAction("Index", "Error", new { message = "ERROR - You are tring to delete ingredient that is part of one or more recipie" });
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        // This method use the web service to get the calories, iron, protein and sugar of ingredient
        private Ingredient GetMetadata(Ingredient indredient)
        {
            using (var client = new HttpClient())
            {
                var response = client.GetAsync(string.Format("https://api.nutritionix.com/v1_1/search/{0}?results=0%3A1&cal_min=0&cal_max=50000&fields=item_name%2Cnf_calories%2Cnf_sugars%2Cnf_protein%2Cnf_iron_dv&appId=70b305ba&appKey=32f30b733ae3f3b3c9727e99a6935fcc", indredient.Name)).Result;
                var result = response.Content.ReadAsStringAsync().Result;
                var data = JObject.Parse(result);
                if (data["hits"].Count() != 0)
                {
                    var fields = data["hits"].First["fields"];
                    indredient.Calories = fields["nf_calories"].Value<double>();
                    indredient.Iron = fields["nf_iron_dv"].Value<double>();
                    indredient.Protein = fields["nf_protein"].Value<double>();
                    indredient.sugar = fields["nf_sugars"].Value<double>();
                }
                return indredient;
            }
        }
    }
}
