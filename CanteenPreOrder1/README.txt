# ============================================================
# NUML Canteen Pre-Order System
# Setup & Run Guide
# ============================================================
# Students: Khuwaja Muhammad Mahdi (609) & Muhammad Ahmad Janjua (623)
# Course   : Advance Programming | BSCS IV-A
# Submitted: Mam Shamim Sharafat
# ============================================================

## REQUIREMENTS
- Visual Studio 2019 or 2022 (Community Edition is fine)
- .NET Framework 4.8
- SQL Server (LocalDB, Express, or full)
- SQL Server Management Studio (SSMS) — optional but recommended

## PROJECT STRUCTURE
CanteenPreOrder/
├── App_Data/
│   └── Database.sql          ← Run this FIRST in SSMS
├── App_Start/
│   ├── FilterConfig.cs
│   └── RouteConfig.cs
├── Controllers/
│   ├── HomeController.cs
│   ├── AccountController.cs
│   ├── StudentController.cs
│   └── AdminController.cs
├── Filters/
│   └── SessionAuthorizeAttribute.cs
├── Helpers/
│   └── DatabaseHelper.cs
├── Models/
│   └── Models.cs
├── Views/
│   ├── Shared/_Layout.cshtml
│   ├── Home/Index.cshtml
│   ├── Account/Login.cshtml
│   ├── Account/Register.cshtml
│   ├── Student/Menu.cshtml
│   ├── Student/Cart.cshtml
│   ├── Student/OrderConfirmation.cshtml
│   ├── Student/OrderHistory.cshtml
│   ├── Student/OrderDetail.cshtml
│   ├── Admin/Dashboard.cshtml
│   ├── Admin/Orders.cshtml
│   ├── Admin/OrderDetail.cshtml
│   ├── Admin/Menu.cshtml
│   ├── Admin/AddMenuItem.cshtml
│   └── Admin/EditMenuItem.cshtml
├── Web.config
├── Global.asax
└── Global.asax.cs

## STEP 1 — CREATE THE PROJECT IN VISUAL STUDIO
1. Open Visual Studio
2. File → New → Project
3. Search: "ASP.NET Web Application (.NET Framework)"
4. Name: CanteenPreOrder
5. Framework: .NET Framework 4.8
6. Click OK → Choose "MVC" template → OK

## STEP 2 — REPLACE/ADD FILES
Copy all the provided files into your project, replacing the defaults:
- Delete the default HomeController, AccountController if they exist
- Copy all files from this zip, maintaining the folder structure

## STEP 3 — SETUP DATABASE
1. Open SQL Server Management Studio (SSMS)
2. Connect to your SQL Server instance
3. Open the file: App_Data/Database.sql
4. Press F5 (or click Execute) to run the script
5. This creates the CanteenDB database with all tables and seed data

## STEP 4 — FIX CONNECTION STRING
Open Web.config and update the connection string if needed:

  For LocalDB (default Visual Studio):
  Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=CanteenDB;Integrated Security=True

  For SQL Server Express:
  Data Source=.\SQLEXPRESS;Initial Catalog=CanteenDB;Integrated Security=True

  For full SQL Server:
  Data Source=(local);Initial Catalog=CanteenDB;Integrated Security=True

## STEP 5 — INSTALL NUGET PACKAGES
In Visual Studio: Tools → NuGet Package Manager → Package Manager Console

Run these commands:
  Install-Package Microsoft.AspNet.Mvc -Version 5.2.9
  Install-Package Microsoft.AspNet.Razor -Version 3.2.9
  Install-Package Microsoft.AspNet.WebPages -Version 3.2.9
  Install-Package Microsoft.Web.Infrastructure -Version 2.0.0

## STEP 6 — BUILD AND RUN
1. Press Ctrl+Shift+B to build
2. Press F5 to run (or Ctrl+F5 without debugger)
3. The site opens in your browser at http://localhost:XXXX/

## LOGIN CREDENTIALS
Admin : admin@canteen.com / Admin@123
Student: Register a new account via the Register page

## FEATURES IMPLEMENTED

### Student Module:
✅ Registration & Login (session-based auth)
✅ Browse menu by categories
✅ Add items to cart / update quantities / remove items
✅ Cart with live subtotal updates (AJAX)
✅ Place order with pickup time selection
✅ Token number generation
✅ Order confirmation page
✅ Order history with status tracking

### Admin Module:
✅ Secure admin login
✅ Dashboard with live stats (today's orders, revenue, counts)
✅ Order pipeline view (Pending / Preparing / Ready / Completed)
✅ Update order status inline (AJAX — no page reload)
✅ Full order details view
✅ Menu management (Add / Edit / Delete items)
✅ Category display

### Technical:
✅ 3-Tier Architecture (Presentation / Business Logic / Data Access)
✅ Full CRUD operations on menu items and orders
✅ Session-based authentication with role checking
✅ SHA1 password hashing
✅ SQL transactions for safe order placement
✅ Anti-CSRF tokens on all forms
✅ Responsive design (mobile friendly)
✅ Professional UI with smooth animations

## ARCHITECTURE
- Presentation Layer  → Views (.cshtml) + Bootstrap 5 + custom CSS
- Business Logic Layer → Controllers (C#)
- Data Access Layer   → DatabaseHelper.cs (ADO.NET with SqlConnection)
- Database            → SQL Server (6 tables)

## DATABASE TABLES
1. Users       — UserID, FullName, Email, Password (hashed), Role
2. Categories  — CategoryID, CategoryName, IconClass
3. MenuItems   — ItemID, CategoryID, ItemName, Description, Price, IsAvailable
4. Cart        — CartID, UserID, ItemID, Quantity
5. Orders      — OrderID, UserID, TokenNumber, TotalAmount, Status, PickupTime
6. OrderItems  — OrderItemID, OrderID, ItemID, Quantity, UnitPrice

## COMMON ISSUES
Q: "Could not open connection" error
A: Check Web.config connection string matches your SQL Server instance

Q: Build errors about missing namespaces
A: Make sure all NuGet packages are installed (Step 5)

Q: "Object reference not set" on first run
A: Make sure the database script ran successfully first (Step 3)

Q: Admin login not working
A: The seed data uses SHA1 hash. If it fails, register a new student
   account and manually update the Role to 'Admin' in the Users table via SSMS.

============================================================
