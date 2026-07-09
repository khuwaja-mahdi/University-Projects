using System.Web.Mvc;
using CanteenPreOrder1.Filters;
using CanteenPreOrder1.Helpers;
using CanteenPreOrder1.Models;

namespace CanteenPreOrder1.Controllers
{
    [SessionAuthorize("Student")]
    public class StudentController : Controller
    {
        private readonly DatabaseHelper _db = new DatabaseHelper();

        private int UserId => (int)Session["UserID"];

      
        public ActionResult Menu(int categoryId = 0)
        {
            var vm = new MenuViewModel
            {
                Categories       = _db.GetCategories(activeOnly: true),
                MenuItems        = _db.GetMenuItems(categoryId, availableOnly: true),
                SelectedCategory = categoryId,
                CartCount        = _db.GetCartCount(UserId)
            };
            return View(vm);
        }

      
        [HttpPost]
        public ActionResult AddToCart(int itemId)
        {
            _db.AddToCart(UserId, itemId);
            return Json(new { success = true, cartCount = _db.GetCartCount(UserId) });
        }

        public ActionResult Cart()
        {
            var items = _db.GetCartItems(UserId);
            decimal total = 0;
            foreach (var i in items) total += i.Subtotal;
            var vm = new CartViewModel { Items = items, Total = total };
            return View(vm);
        }

   
        [HttpPost]
        public ActionResult UpdateCart(int cartId, int quantity)
        {
            _db.UpdateCartQuantity(cartId, quantity);
            return Json(new { success = true });
        }

        // POST: /Student/RemoveFromCart
        [HttpPost]
        public ActionResult RemoveFromCart(int cartId)
        {
            _db.RemoveFromCart(cartId);
            return Json(new { success = true });
        }

        // POST: /Student/PlaceOrder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PlaceOrder(string pickupTime)
        {
            var items = _db.GetCartItems(UserId);
            if (items.Count == 0)
            {
                TempData["Error"] = "Your cart is empty!";
                return RedirectToAction("Cart");
            }

            decimal total = 0;
            foreach (var i in items) total += i.Subtotal;

            int orderId = _db.PlaceOrder(UserId, total, pickupTime, items);
            if (orderId == 0)
            {
                TempData["Error"] = "Failed to place order. Please try again.";
                return RedirectToAction("Cart");
            }

            return RedirectToAction("OrderConfirmation", new { id = orderId });
        }

        // GET: /Student/OrderConfirmation/5
        public ActionResult OrderConfirmation(int id)
        {
            var order = _db.GetOrderById(id);
            if (order == null || order.UserID != UserId) return HttpNotFound();
            return View(order);
        }

        // GET: /Student/OrderHistory
        public ActionResult OrderHistory()
        {
            var orders = _db.GetOrdersByUser(UserId);
            return View(orders);
        }

        // GET: /Student/OrderDetail/5
        public ActionResult OrderDetail(int id)
        {
            var order = _db.GetOrderById(id);
            if (order == null || order.UserID != UserId) return HttpNotFound();
            return View(order);
        }
    }
}
