
using System.Web.Mvc;
using CanteenPreOrder1.Helpers;

namespace CanteenPreOrder1.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (Session["UserID"] != null)
            {
                return Session["UserRole"] != null && Session["UserRole"].ToString() == "Admin"
                    ? RedirectToAction("Dashboard", "Admin")
                    : RedirectToAction("Menu", "Student");
            }
            return View();
        }

        public ActionResult AccessDenied() => View();
    }
}
