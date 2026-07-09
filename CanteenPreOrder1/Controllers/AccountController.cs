using System.Web.Mvc;
using CanteenPreOrder1.Helpers;
using CanteenPreOrder1.Models;

namespace CanteenPreOrder1.Controllers
{
    public class AccountController : Controller
    {
        private readonly DatabaseHelper _db = new DatabaseHelper();

        public ActionResult Login()
        {
            if (Session["UserID"] != null)
                return Session["UserRole"] != null && Session["UserRole"].ToString() == "Admin"
                    ? RedirectToAction("Dashboard", "Admin")
                    : RedirectToAction("Menu", "Student");
            return View();
        }

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = _db.ValidateLogin(model.Email, model.Password);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View(model);
            }

            Session["UserID"]   = user.UserID;
            Session["UserName"] = user.FullName;
            Session["UserRole"] = user.Role;

            return user.Role == "Admin"
                ? RedirectToAction("Dashboard", "Admin")
                : RedirectToAction("Menu", "Student");
        }

        
        public ActionResult Register()
        {
            if (Session["UserID"] != null) return RedirectToAction("Menu", "Student");
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            bool success = _db.RegisterUser(model.FullName, model.Email, model.Password);
            if (!success)
            {
                ModelState.AddModelError("Email", "This email is already registered.");
                return View(model);
            }

            TempData["Success"] = "Registration successful! Please login.";
            return RedirectToAction("Login");
        }

     
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Login");
        }
    }
}
