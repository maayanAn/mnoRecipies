using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Recepies.Models;
using DbContext = Recepies.Models.DbContext;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Recepies.Controllers
{
    public class RecipiesController : Controller
    {
        public class CreateRecipieViewModel
        {
            public CreateRecipieViewModel()
            {
                // Init the array for the case of no ingredients selected
                IngredientsIds = new List<int>();
            }
            public Recipe Recipe { get; set; }

            public List<int> IngredientsIds { get; set; }
        }


        private DbContext db = new DbContext();

        // GET: Recipies
        public ActionResult Index()
        {
            var currentUser = ((User)HttpContext.Session["user"]);
            if (currentUser != null)
            {
                return View(db.Recipies.Include(r => r.User).ToList());
            }
            return RedirectToAction("Index", "Error");

        }

        // GET: Recipies/Search
        public ActionResult Search(string ingredient = null, string user = null, string level = null)
        {
            ViewBag.Ingredients = new SelectList(db.Ingredients.Select(i => i.Name));
            if (((User)HttpContext.Session["user"]) != null)
            {
                var returnDataQurey = db.Recipies.Include(r => r.User).Select(r => r);

                if (!string.IsNullOrEmpty(ingredient))
                {
                    returnDataQurey = returnDataQurey.Where(r => r.Ingredients.Any(i => i.Name == ingredient));
                }

                if (!string.IsNullOrEmpty(user))
                {
                    returnDataQurey = returnDataQurey.Where(r => r.User.FullName.Contains(user));
                }

                if (!string.IsNullOrEmpty(level))
                {
                    returnDataQurey = returnDataQurey.Where(r => ((int)r.Difficulty).ToString() == level);
                }

                return View(returnDataQurey.ToList());
            }
            else
            {
                return RedirectToAction("Index", "Error");
            }
        }

        // GET: Recipies/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Error");
            }
            Recipe recipe = db.Recipies.Where(r => r.Id == id.Value).Include(r => r.Ingredients).Include(r => r.User).FirstOrDefault();
            if (recipe == null)
            {
                return HttpNotFound();
            }
            var c = recipe.Ingredients;
            return View(recipe);
        }

        // GET: Recipies/Create
        public ActionResult Create()
        {
            var ingredientList = new MultiSelectList(db.Ingredients.Select(i => i), "Id", "Name");
            ViewBag.Ingredients = ingredientList;
            if (((User)HttpContext.Session["user"]) != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Recipies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Recipe, IngredientsIds")] CreateRecipieViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // Add the selected Ingredients
                viewModel.Recipe.Ingredients = new List<Ingredient>();
                foreach (var ingredientId in viewModel.IngredientsIds)
                {
                    var ingredientToAdd = db.Ingredients.Find(ingredientId);
                    viewModel.Recipe.Ingredients.Add(ingredientToAdd);
                }

                // Set the current user as the owner of the recipe
                // Find - To avoid creating new user with the same name
                var owner = db.Users.Find(((User)HttpContext.Session["user"]).Id);
                viewModel.Recipe.User = owner;

                db.Recipies.Add(viewModel.Recipe);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(viewModel);
        }

        // GET: Recipies/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Error");
            }
            Recipe recipe = db.Recipies.Where(r => r.Id == id).Include(r => r.User).FirstOrDefault();
            if (recipe == null)
            {
                return HttpNotFound();
            }
            return View(recipe);
        }

        // POST: Recipies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Difficulty,PreparationTimeInMinutes,Context")] Recipe recipe)
        {
            if (ModelState.IsValid)
            {
                db.Entry(recipe).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(recipe);
        }

        // GET: Recipies/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Error");
            }
            Recipe recipe = db.Recipies.Include(r => r.User).Where(r => r.Id == id).FirstOrDefault();
            if (recipe == null)
            {
                return HttpNotFound();
            }
            return View(recipe);
        }

        // POST: Recipies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Recipe recipe = db.Recipies.Find(id);
            db.Recipies.Remove(recipe);
            db.SaveChanges();
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
    }
}
