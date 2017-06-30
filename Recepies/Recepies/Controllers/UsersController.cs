using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Recepies.Models;
using DbContext = Recepies.Models.DbContext;

namespace Recepies.Controllers
{
    [HandleError]
    public class UsersController : Controller
    {
        private DbContext db = new DbContext();

        // GET: Users
        public ActionResult Index()
        {
            var currentUser = ((User)HttpContext.Session["user"]);
            if (currentUser != null)
            {
                var users = db.Users.Select(s => s);
                if (!currentUser.IsManager)
                {
                    users = users.Where(u => u.Id == currentUser.Id);
                }
                return View(users.ToList());
            }
            return RedirectToAction("Index", "Error");
        }

        // GET: Users/Search
        public ActionResult Search(string name = null, bool isManager = false, string minRecipies = null)
        {
            var currentUser = ((User)HttpContext.Session["user"]);
            if (currentUser != null)
            {
                if (currentUser.IsManager)
                {
                    var returnDataQurey = db.Users.Include(u => u.Recipies).Select(u => u);

                    if (!string.IsNullOrEmpty(name))
                    {
                        returnDataQurey = returnDataQurey.Where(u => u.FullName.Contains(name));
                    }

                    returnDataQurey = returnDataQurey.Where(u => u.IsManager == isManager);

                    if (!string.IsNullOrEmpty(minRecipies))
                    {
                        var minRecipiesNumber = int.Parse(minRecipies);
                        returnDataQurey = returnDataQurey.Where(u => u.Recipies.Count() >= minRecipiesNumber);
                    }

                    return View(returnDataQurey.ToDictionary(u => u, y => y.Recipies.Count()));
                }
                else
                {
                    return RedirectToAction("Index", "Error", new { message = "you are not autorized" });
                }
            }
            return RedirectToAction("Index", "Error");
        }

        // GET: Users/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Error");
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Users/Register
        public ActionResult Register()
        {
            var currentUser = ((User)HttpContext.Session["user"]);
            if (currentUser == null)
            {
                return View();
            }
            else
            {
                if (currentUser.IsManager)
                {
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Error", new { message = "you are not autorized" });
                }
            }
        }

        // GET: Users/Login
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string fullName, string password)
        {
            var userInDataBase = db.Users.Where(s =>
                        s.FullName.Equals(fullName, System.StringComparison.Ordinal) &&
                        s.Password.Equals(password, System.StringComparison.Ordinal)).SingleOrDefault();
            if (userInDataBase != null)
            {
                System.Web.HttpContext.Current.Session["user"] = userInDataBase;
                return RedirectToAction("Index", "Recipies");
            }

            ViewBag.ErrMsg = "User name or password are incorrect.";
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register([Bind(Include = "Id,Password,IsManager,FullName")] User user)
        {
            if (ModelState.IsValid)
            {
                if (db.Users.Where(c => c.FullName.Equals(user.FullName)).Count() > 0)
                {
                    ViewBag.ErrMsg = "User name already exists, please try again";
                }
                else
                {
                    db.Users.Add(user);
                    db.SaveChanges();
                    if (System.Web.HttpContext.Current.Session["user"] == null)
                    {
                        System.Web.HttpContext.Current.Session["user"] = user;
                        return RedirectToAction("Index", "Recipies");
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                }
            }
            return View(user);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Error");
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Password,IsManager,FullName")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Error");
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var userInDataBase = db.Users.Where(s =>
                       s.Id == id).SingleOrDefault();

            User user = db.Users.Find(id);
            try
            {
                db.Users.Remove(user);
                db.SaveChanges();
            }
            catch
            {
                return RedirectToAction("Index", "Error", new { message = "ERROR - You are tring to delete user who owns one or more recipie" });
            }

            if (userInDataBase != null)
            {
                if (((User)(System.Web.HttpContext.Current.Session["user"])).IsManager)
                    return RedirectToAction("Index");
                else
                {
                    System.Web.HttpContext.Current.Session["user"] = null;
                    return RedirectToAction("Login");
                }
            }
            ViewBag.ErrMsg = "User does not exist.";
            return View();
        }

        public ActionResult Logoff()
        {
            System.Web.HttpContext.Current.Session["user"] = null;
            return RedirectToAction("Login");
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
