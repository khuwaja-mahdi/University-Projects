using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using CanteenPreOrder1.Models;

namespace CanteenPreOrder1.Helpers
{
    public class DatabaseHelper
    {
        private readonly string _connStr;

        public DatabaseHelper()
        {
            _connStr = ConfigurationManager.ConnectionStrings["CanteenDB"].ConnectionString;
        }

        private SqlConnection GetConnection() => new SqlConnection(_connStr);

        public static string HashPassword(string password)
        {
            using (var sha1 = SHA1.Create())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }

       
        public User GetUserByEmail(string email)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM Users WHERE Email = @Email", conn);
                cmd.Parameters.AddWithValue("@Email", email);
                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return new User
                    {
                        UserID   = (int)reader["UserID"],
                        FullName = reader["FullName"].ToString(),
                        Email    = reader["Email"].ToString(),
                        Password = reader["Password"].ToString(),
                        Role     = reader["Role"].ToString(),
                        CreatedAt = (DateTime)reader["CreatedAt"]
                    };
                }
                return null;
            }
        }

        public bool RegisterUser(string fullName, string email, string password)
        {
            if (GetUserByEmail(email) != null) return false;
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    "INSERT INTO Users (FullName, Email, Password, Role) VALUES (@F,@E,@P,'Student')", conn);
                cmd.Parameters.AddWithValue("@F", fullName);
                cmd.Parameters.AddWithValue("@E", email);
                cmd.Parameters.AddWithValue("@P", HashPassword(password));
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public User ValidateLogin(string email, string password)
        {
            var user = GetUserByEmail(email);
            if (user == null) return null;
            return user.Password == HashPassword(password) ? user : null;
        }

        public int GetStudentCount()
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT COUNT(*) FROM Users WHERE Role='Student'", conn);
                return (int)cmd.ExecuteScalar();
            }
        }

      
        public List<Category> GetCategories(bool activeOnly = false)
        {
            var list = new List<Category>();
            using (var conn = GetConnection())
            {
                conn.Open();
                var sql = activeOnly
                    ? "SELECT * FROM Categories WHERE IsActive=1 ORDER BY CategoryName"
                    : "SELECT * FROM Categories ORDER BY CategoryName";
                var reader = new SqlCommand(sql, conn).ExecuteReader();
                while (reader.Read())
                    list.Add(new Category
                    {
                        CategoryID   = (int)reader["CategoryID"],
                        CategoryName = reader["CategoryName"].ToString(),
                        IconClass    = reader["IconClass"].ToString(),
                        IsActive     = (bool)reader["IsActive"]
                    });
            }
            return list;
        }

        public bool AddCategory(string name, string icon)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    "INSERT INTO Categories (CategoryName, IconClass) VALUES (@N, @I)", conn);
                cmd.Parameters.AddWithValue("@N", name);
                cmd.Parameters.AddWithValue("@I", icon);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public List<MenuItem> GetMenuItems(int categoryId = 0, bool availableOnly = false)
        {
            var list = new List<MenuItem>();
            using (var conn = GetConnection())
            {
                conn.Open();
                var sql = @"SELECT m.*, c.CategoryName FROM MenuItems m
                            JOIN Categories c ON m.CategoryID = c.CategoryID WHERE 1=1";
                if (categoryId > 0) sql += " AND m.CategoryID = @CatID";
                if (availableOnly)  sql += " AND m.IsAvailable = 1";
                sql += " ORDER BY c.CategoryName, m.ItemName";

                var cmd = new SqlCommand(sql, conn);
                if (categoryId > 0) cmd.Parameters.AddWithValue("@CatID", categoryId);

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                    list.Add(MapMenuItem(reader));
            }
            return list;
        }

        public MenuItem GetMenuItemById(int id)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    "SELECT m.*, c.CategoryName FROM MenuItems m JOIN Categories c ON m.CategoryID=c.CategoryID WHERE m.ItemID=@ID", conn);
                cmd.Parameters.AddWithValue("@ID", id);
                var reader = cmd.ExecuteReader();
                return reader.Read() ? MapMenuItem(reader) : null;
            }
        }

        private MenuItem MapMenuItem(SqlDataReader r) => new MenuItem
        {
            ItemID       = (int)r["ItemID"],
            CategoryID   = (int)r["CategoryID"],
            CategoryName = r["CategoryName"].ToString(),
            ItemName     = r["ItemName"].ToString(),
            Description  = r["Description"] == DBNull.Value ? "" : r["Description"].ToString(),
            Price        = (decimal)r["Price"],
            ImageUrl     = r["ImageUrl"] == DBNull.Value ? "" : r["ImageUrl"].ToString(),
            IsAvailable  = (bool)r["IsAvailable"],
            CreatedAt    = (DateTime)r["CreatedAt"]
        };

        public bool AddMenuItem(MenuItem item)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    @"INSERT INTO MenuItems (CategoryID,ItemName,Description,Price,ImageUrl,IsAvailable)
                      VALUES (@C,@N,@D,@P,@I,@A)", conn);
                cmd.Parameters.AddWithValue("@C", item.CategoryID);
                cmd.Parameters.AddWithValue("@N", item.ItemName);
                cmd.Parameters.AddWithValue("@D", (object)item.Description ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@P", item.Price);
                cmd.Parameters.AddWithValue("@I", (object)item.ImageUrl ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@A", item.IsAvailable);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool UpdateMenuItem(MenuItem item)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    @"UPDATE MenuItems SET CategoryID=@C,ItemName=@N,Description=@D,
                      Price=@P,ImageUrl=@I,IsAvailable=@A WHERE ItemID=@ID", conn);
                cmd.Parameters.AddWithValue("@C", item.CategoryID);
                cmd.Parameters.AddWithValue("@N", item.ItemName);
                cmd.Parameters.AddWithValue("@D", (object)item.Description ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@P", item.Price);
                cmd.Parameters.AddWithValue("@I", (object)item.ImageUrl ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@A", item.IsAvailable);
                cmd.Parameters.AddWithValue("@ID", item.ItemID);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool DeleteMenuItem(int id)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("DELETE FROM MenuItems WHERE ItemID=@ID", conn);
                cmd.Parameters.AddWithValue("@ID", id);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public int GetMenuItemCount()
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                return (int)new SqlCommand("SELECT COUNT(*) FROM MenuItems WHERE IsAvailable=1", conn).ExecuteScalar();
            }
        }

    
        public List<CartItem> GetCartItems(int userId)
        {
            var list = new List<CartItem>();
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    @"SELECT c.CartID, c.UserID, c.ItemID, c.Quantity, m.ItemName, m.Price
                      FROM Cart c JOIN MenuItems m ON c.ItemID=m.ItemID WHERE c.UserID=@UID", conn);
                cmd.Parameters.AddWithValue("@UID", userId);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                    list.Add(new CartItem
                    {
                        CartID   = (int)reader["CartID"],
                        UserID   = (int)reader["UserID"],
                        ItemID   = (int)reader["ItemID"],
                        ItemName = reader["ItemName"].ToString(),
                        Price    = (decimal)reader["Price"],
                        Quantity = (int)reader["Quantity"]
                    });
            }
            return list;
        }

        public int GetCartCount(int userId)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT ISNULL(SUM(Quantity),0) FROM Cart WHERE UserID=@UID", conn);
                cmd.Parameters.AddWithValue("@UID", userId);
                return (int)cmd.ExecuteScalar();
            }
        }

        public bool AddToCart(int userId, int itemId)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                // Check if already in cart
                var check = new SqlCommand(
                    "SELECT CartID FROM Cart WHERE UserID=@U AND ItemID=@I", conn);
                check.Parameters.AddWithValue("@U", userId);
                check.Parameters.AddWithValue("@I", itemId);
                var existing = check.ExecuteScalar();

                if (existing != null)
                {
                    var upd = new SqlCommand(
                        "UPDATE Cart SET Quantity=Quantity+1 WHERE CartID=@ID", conn);
                    upd.Parameters.AddWithValue("@ID", existing);
                    return upd.ExecuteNonQuery() > 0;
                }
                else
                {
                    var ins = new SqlCommand(
                        "INSERT INTO Cart (UserID,ItemID,Quantity) VALUES (@U,@I,1)", conn);
                    ins.Parameters.AddWithValue("@U", userId);
                    ins.Parameters.AddWithValue("@I", itemId);
                    return ins.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool UpdateCartQuantity(int cartId, int quantity)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                if (quantity <= 0)
                {
                    var del = new SqlCommand("DELETE FROM Cart WHERE CartID=@ID", conn);
                    del.Parameters.AddWithValue("@ID", cartId);
                    return del.ExecuteNonQuery() > 0;
                }
                var cmd = new SqlCommand("UPDATE Cart SET Quantity=@Q WHERE CartID=@ID", conn);
                cmd.Parameters.AddWithValue("@Q", quantity);
                cmd.Parameters.AddWithValue("@ID", cartId);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool RemoveFromCart(int cartId)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("DELETE FROM Cart WHERE CartID=@ID", conn);
                cmd.Parameters.AddWithValue("@ID", cartId);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool ClearCart(int userId)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("DELETE FROM Cart WHERE UserID=@UID", conn);
                cmd.Parameters.AddWithValue("@UID", userId);
                return cmd.ExecuteNonQuery() >= 0;
            }
        }

     
        public string GenerateToken()
        {
            var rand = new Random();
            return "TKN-" + rand.Next(1000, 9999).ToString();
        }

        public int PlaceOrder(int userId, decimal total, string pickupTime, List<CartItem> items)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var tran = conn.BeginTransaction();
                try
                {
                    var token = GenerateToken();
                    var orderCmd = new SqlCommand(
                        @"INSERT INTO Orders (UserID,TokenNumber,TotalAmount,Status,PickupTime)
                          VALUES (@U,@T,@A,'Pending',@P);
                          SELECT SCOPE_IDENTITY();", conn, tran);
                    orderCmd.Parameters.AddWithValue("@U", userId);
                    orderCmd.Parameters.AddWithValue("@T", token);
                    orderCmd.Parameters.AddWithValue("@A", total);
                    orderCmd.Parameters.AddWithValue("@P", (object)pickupTime ?? DBNull.Value);
                    int orderId = Convert.ToInt32(orderCmd.ExecuteScalar());

                    foreach (var item in items)
                    {
                        var itemCmd = new SqlCommand(
                            "INSERT INTO OrderItems (OrderID,ItemID,Quantity,UnitPrice) VALUES (@O,@I,@Q,@P)", conn, tran);
                        itemCmd.Parameters.AddWithValue("@O", orderId);
                        itemCmd.Parameters.AddWithValue("@I", item.ItemID);
                        itemCmd.Parameters.AddWithValue("@Q", item.Quantity);
                        itemCmd.Parameters.AddWithValue("@P", item.Price);
                        itemCmd.ExecuteNonQuery();
                    }

                    // Clear cart
                    var clearCmd = new SqlCommand("DELETE FROM Cart WHERE UserID=@U", conn, tran);
                    clearCmd.Parameters.AddWithValue("@U", userId);
                    clearCmd.ExecuteNonQuery();

                    tran.Commit();
                    return orderId;
                }
                catch
                {
                    tran.Rollback();
                    return 0;
                }
            }
        }

        public List<Order> GetOrdersByUser(int userId)
        {
            var list = new List<Order>();
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    "SELECT * FROM Orders WHERE UserID=@U ORDER BY OrderedAt DESC", conn);
                cmd.Parameters.AddWithValue("@U", userId);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                    list.Add(MapOrder(reader));
            }
            return list;
        }

        public List<Order> GetAllOrders(string status = null)
        {
            var list = new List<Order>();
            using (var conn = GetConnection())
            {
                conn.Open();
                var sql = @"SELECT o.*, u.FullName AS StudentName FROM Orders o
                            JOIN Users u ON o.UserID=u.UserID";
                if (!string.IsNullOrEmpty(status)) sql += " WHERE o.Status=@S";
                sql += " ORDER BY o.OrderedAt DESC";
                var cmd = new SqlCommand(sql, conn);
                if (!string.IsNullOrEmpty(status)) cmd.Parameters.AddWithValue("@S", status);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var o = MapOrder(reader);
                    o.StudentName = reader["StudentName"].ToString();
                    list.Add(o);
                }
            }
            return list;
        }

        public Order GetOrderById(int orderId)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    @"SELECT o.*, u.FullName AS StudentName FROM Orders o
                      JOIN Users u ON o.UserID=u.UserID WHERE o.OrderID=@ID", conn);
                cmd.Parameters.AddWithValue("@ID", orderId);
                var reader = cmd.ExecuteReader();
                if (!reader.Read()) return null;
                var order = MapOrder(reader);
                order.StudentName = reader["StudentName"].ToString();
                reader.Close();

                // Get order items
                var itemCmd = new SqlCommand(
                    @"SELECT oi.*, m.ItemName FROM OrderItems oi
                      JOIN MenuItems m ON oi.ItemID=m.ItemID WHERE oi.OrderID=@ID", conn);
                itemCmd.Parameters.AddWithValue("@ID", orderId);
                var itemReader = itemCmd.ExecuteReader();
                while (itemReader.Read())
                    order.Items.Add(new OrderItem
                    {
                        OrderItemID = (int)itemReader["OrderItemID"],
                        OrderID     = (int)itemReader["OrderID"],
                        ItemID      = (int)itemReader["ItemID"],
                        ItemName    = itemReader["ItemName"].ToString(),
                        Quantity    = (int)itemReader["Quantity"],
                        UnitPrice   = (decimal)itemReader["UnitPrice"]
                    });
                return order;
            }
        }

        public bool UpdateOrderStatus(int orderId, string status)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    "UPDATE Orders SET Status=@S, UpdatedAt=GETDATE() WHERE OrderID=@ID", conn);
                cmd.Parameters.AddWithValue("@S", status);
                cmd.Parameters.AddWithValue("@ID", orderId);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        private Order MapOrder(SqlDataReader r) => new Order
        {
            OrderID     = (int)r["OrderID"],
            UserID      = (int)r["UserID"],
            TokenNumber = r["TokenNumber"].ToString(),
            TotalAmount = (decimal)r["TotalAmount"],
            Status      = r["Status"].ToString(),
            PickupTime  = r["PickupTime"] == DBNull.Value ? "" : r["PickupTime"].ToString(),
            OrderedAt   = (DateTime)r["OrderedAt"],
            UpdatedAt   = (DateTime)r["UpdatedAt"]
        };

        // ========================
        // DASHBOARD STATS
        // ========================
        public DashboardViewModel GetDashboardStats()
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var sql = @"
                    SELECT
                        (SELECT COUNT(*) FROM Orders WHERE CAST(OrderedAt AS DATE)=CAST(GETDATE() AS DATE)) AS TotalOrders,
                        (SELECT COUNT(*) FROM Orders WHERE Status='Pending') AS Pending,
                        (SELECT COUNT(*) FROM Orders WHERE Status='Preparing') AS Preparing,
                        (SELECT COUNT(*) FROM Orders WHERE Status='Ready') AS Ready,
                        (SELECT COUNT(*) FROM Orders WHERE Status='Completed' AND CAST(OrderedAt AS DATE)=CAST(GETDATE() AS DATE)) AS Completed,
                        (SELECT ISNULL(SUM(TotalAmount),0) FROM Orders WHERE CAST(OrderedAt AS DATE)=CAST(GETDATE() AS DATE)) AS Revenue,
                        (SELECT COUNT(*) FROM MenuItems WHERE IsAvailable=1) AS MenuCount,
                        (SELECT COUNT(*) FROM Users WHERE Role='Student') AS Students";

                var reader = new SqlCommand(sql, conn).ExecuteReader();
                if (!reader.Read()) return new DashboardViewModel();

                return new DashboardViewModel
                {
                    TotalOrders     = (int)reader["TotalOrders"],
                    PendingOrders   = (int)reader["Pending"],
                    PreparingOrders = (int)reader["Preparing"],
                    ReadyOrders     = (int)reader["Ready"],
                    CompletedOrders = (int)reader["Completed"],
                    TodayRevenue    = (decimal)reader["Revenue"],
                    TotalMenuItems  = (int)reader["MenuCount"],
                    TotalStudents   = (int)reader["Students"]
                };
            }
        }
    }
}
