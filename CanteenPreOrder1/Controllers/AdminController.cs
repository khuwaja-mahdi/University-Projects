using System.Collections.Generic;
using System.Web.Mvc;
using CanteenPreOrder1.Filters;
using CanteenPreOrder1.Helpers;
using CanteenPreOrder1.Models;

namespace CanteenPreOrder1.Controllers
{
    [SessionAuthorize("Admin")]
    public class AdminController : Controller
    {
        private readonly DatabaseHelper _db = new DatabaseHelper();

        // GET: /Admin/Dashboard
        public ActionResult Dashboard()
        {
            var vm = _db.GetDashboardStats();
            vm.RecentOrders = _db.GetAllOrders();
            if (vm.RecentOrders.Count > 10)
                vm.RecentOrders = vm.RecentOrders.GetRange(0, 10);
            return View(vm);
        }

        public ActionResult Orders(string status = null)
        {
            var orders = _db.GetAllOrders(status);
            ViewBag.StatusFilter = status;
            return View(orders);
        }

        public ActionResult OrderDetail(int id)
        {
            var order = _db.GetOrderById(id);
            if (order == null) return HttpNotFound();
            return View(order);
        }

        [HttpPost]
        public ActionResult UpdateStatus(int orderId, string status)
        {
            _db.UpdateOrderStatus(orderId, status);
            return Json(new { success = true });
        }

       
        public ActionResult Menu()
        {
            var items = _db.GetMenuItems();
            return View(items);
        }

        public ActionResult AddMenuItem()
        {
            var vm = new MenuItemViewModel { Categories = _db.GetCategories() };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddMenuItem(MenuItemViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Categories = _db.GetCategories();
                return View(vm);
            }

            var item = new MenuItem
            {
                CategoryID  = vm.CategoryID,
                ItemName    = vm.ItemName,
                Description = vm.Description,
                Price       = vm.Price,
                ImageUrl    = vm.ImageUrl,
                IsAvailable = vm.IsAvailable
            };

            _db.AddMenuItem(item);
            TempData["Success"] = "Menu item added successfully!";
            return RedirectToAction("Menu");
        }

        public ActionResult EditMenuItem(int id)
        {
            var item = _db.GetMenuItemById(id);
            if (item == null) return HttpNotFound();

            var vm = new MenuItemViewModel
            {
                ItemID      = item.ItemID,
                CategoryID  = item.CategoryID,
                ItemName    = item.ItemName,
                Description = item.Description,
                Price       = item.Price,
                ImageUrl    = item.ImageUrl,
                IsAvailable = item.IsAvailable,
                Categories  = _db.GetCategories()
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditMenuItem(MenuItemViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Categories = _db.GetCategories();
                return View(vm);
            }

            var item = new MenuItem
            {
                ItemID      = vm.ItemID,
                CategoryID  = vm.CategoryID,
                ItemName    = vm.ItemName,
                Description = vm.Description,
                Price       = vm.Price,
                ImageUrl    = vm.ImageUrl,
                IsAvailable = vm.IsAvailable
            };

            _db.UpdateMenuItem(item);
            TempData["Success"] = "Menu item updated successfully!";
            return RedirectToAction("Menu");
        }

        [HttpPost]
        public ActionResult DeleteMenuItem(int id)
        {
            _db.DeleteMenuItem(id);
            return Json(new { success = true });
        }

     
        public ActionResult Categories()
        {
            var cats = _db.GetCategories();
            return View(cats);
        }

        [HttpPost]
        public ActionResult AddCategory(string name, string icon)
        {
            _db.AddCategory(name, icon ?? "fa-utensils");
            return Json(new { success = true });
        }
    }
}
