using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CanteenPreOrder1.Models
{
    

    public class User
    {
        public int UserID { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class Category
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string IconClass { get; set; }
        public bool IsActive { get; set; }
    }

    public class MenuItem
    {
        public int ItemID { get; set; }
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string ItemName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CartItem
    {
        public int CartID { get; set; }
        public int UserID { get; set; }
        public int ItemID { get; set; }
        public string ItemName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal => Price * Quantity;
    }

    public class Order
    {
        public int OrderID { get; set; }
        public int UserID { get; set; }
        public string StudentName { get; set; }
        public string TokenNumber { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public string PickupTime { get; set; }
        public DateTime OrderedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
    }

    public class OrderItem
    {
        public int OrderItemID { get; set; }
        public int OrderID { get; set; }
        public int ItemID { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal => UnitPrice * Quantity;
    }

    
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please confirm your password")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }

    public class MenuViewModel
    {
        public List<Category> Categories { get; set; }
        public List<MenuItem> MenuItems { get; set; }
        public int SelectedCategory { get; set; }
        public int CartCount { get; set; }
    }

    public class CartViewModel
    {
        public List<CartItem> Items { get; set; }
        public decimal Total { get; set; }
        public string PickupTime { get; set; }
    }

    public class MenuItemViewModel
    {
        public int ItemID { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public int CategoryID { get; set; }

        [Required(ErrorMessage = "Item name is required")]
        [StringLength(150)]
        public string ItemName { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 99999, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        public string ImageUrl { get; set; }
        public bool IsAvailable { get; set; } = true;
        public List<Category> Categories { get; set; }
    }

    public class DashboardViewModel
    {
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public int PreparingOrders { get; set; }
        public int ReadyOrders { get; set; }
        public int CompletedOrders { get; set; }
        public decimal TodayRevenue { get; set; }
        public int TotalMenuItems { get; set; }
        public int TotalStudents { get; set; }
        public List<Order> RecentOrders { get; set; }
    }
}
