

USE master;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'CanteenDB')
BEGIN
    CREATE DATABASE CanteenDB;
END
GO

USE CanteenDB;
GO


IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Users' AND xtype='U')
CREATE TABLE Users (
    UserID      INT IDENTITY(1,1) PRIMARY KEY,
    FullName    NVARCHAR(100) NOT NULL,
    Email       NVARCHAR(150) NOT NULL UNIQUE,
    Password    NVARCHAR(256) NOT NULL,  
    Role        NVARCHAR(20)  NOT NULL DEFAULT 'Student',
    CreatedAt   DATETIME      NOT NULL DEFAULT GETDATE()
);
GO


IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Categories' AND xtype='U')
CREATE TABLE Categories (
    CategoryID   INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName NVARCHAR(100) NOT NULL,
    IconClass    NVARCHAR(50)  NOT NULL DEFAULT 'fa-utensils',
    IsActive     BIT           NOT NULL DEFAULT 1
);
GO


IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='MenuItems' AND xtype='U')
CREATE TABLE MenuItems (
    ItemID       INT IDENTITY(1,1) PRIMARY KEY,
    CategoryID   INT            NOT NULL FOREIGN KEY REFERENCES Categories(CategoryID),
    ItemName     NVARCHAR(150)  NOT NULL,
    Description  NVARCHAR(500)  NULL,
    Price        DECIMAL(10,2)  NOT NULL,
    ImageUrl     NVARCHAR(300)  NULL,
    IsAvailable  BIT            NOT NULL DEFAULT 1,
    CreatedAt    DATETIME       NOT NULL DEFAULT GETDATE()
);
GO


IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Cart' AND xtype='U')
CREATE TABLE Cart (
    CartID     INT IDENTITY(1,1) PRIMARY KEY,
    UserID     INT           NOT NULL FOREIGN KEY REFERENCES Users(UserID),
    ItemID     INT           NOT NULL FOREIGN KEY REFERENCES MenuItems(ItemID),
    Quantity   INT           NOT NULL DEFAULT 1,
    AddedAt    DATETIME      NOT NULL DEFAULT GETDATE()
);
GO


IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Orders' AND xtype='U')
CREATE TABLE Orders (
    OrderID       INT IDENTITY(1,1) PRIMARY KEY,
    UserID        INT           NOT NULL FOREIGN KEY REFERENCES Users(UserID),
    TokenNumber   NVARCHAR(20)  NOT NULL,
    TotalAmount   DECIMAL(10,2) NOT NULL,
    Status        NVARCHAR(30)  NOT NULL DEFAULT 'Pending', -- Pending, Preparing, Ready, Completed
    PickupTime    NVARCHAR(20)  NULL,
    OrderedAt     DATETIME      NOT NULL DEFAULT GETDATE(),
    UpdatedAt     DATETIME      NOT NULL DEFAULT GETDATE()
);
GO


IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='OrderItems' AND xtype='U')
CREATE TABLE OrderItems (
    OrderItemID INT IDENTITY(1,1) PRIMARY KEY,
    OrderID     INT           NOT NULL FOREIGN KEY REFERENCES Orders(OrderID),
    ItemID      INT           NOT NULL FOREIGN KEY REFERENCES MenuItems(ItemID),
    Quantity    INT           NOT NULL,
    UnitPrice   DECIMAL(10,2) NOT NULL
);
GO



-- Default Admin (Password: Admin@123)
IF NOT EXISTS (SELECT * FROM Users WHERE Email = 'admin@canteen.com')
INSERT INTO Users (FullName, Email, Password, Role)
VALUES ('Admin', 'admin@canteen.com', '240be518fabd2724ddb6f04eeb1da5967448d7e831d9', 'Admin');
GO


IF NOT EXISTS (SELECT * FROM Categories)
BEGIN
    INSERT INTO Categories (CategoryName, IconClass) VALUES
    ('Rice & Biryani',   'fa-bowl-rice'),
    ('Burgers',          'fa-burger'),
    ('Drinks',           'fa-mug-hot'),
    ('Snacks',           'fa-cookie-bite'),
    ('Desi Food',        'fa-fire-burner'),
    ('Sandwiches',       'fa-bread-slice');
END
GO


IF NOT EXISTS (SELECT * FROM MenuItems)
BEGIN
    INSERT INTO MenuItems (CategoryID, ItemName, Description, Price, IsAvailable) VALUES
    (1, 'Chicken Biryani',     'Fragrant rice with tender chicken and spices',       180.00, 1),
    (1, 'Beef Biryani',        'Classic beef biryani with raita',                    200.00, 1),
    (1, 'Plain Rice + Daal',   'Steam rice served with daal',                         80.00, 1),
    (2, 'Chicken Burger',      'Crispy chicken patty with lettuce and sauce',        150.00, 1),
    (2, 'Beef Burger',         'Juicy beef patty with cheese',                       180.00, 1),
    (2, 'Zinger Burger',       'Spicy crispy chicken with special sauce',            170.00, 1),
    (3, 'Soft Drink (Can)',    'Pepsi / 7Up / Mirinda',                               60.00, 1),
    (3, 'Mineral Water',       '500ml chilled water',                                 30.00, 1),
    (3, 'Fresh Juice',         'Seasonal fresh juice',                                80.00, 1),
    (4, 'Samosa (2 pcs)',      'Crispy samosas with chutney',                         40.00, 1),
    (4, 'Pakora Plate',        'Mixed vegetable pakoras',                             60.00, 1),
    (4, 'Spring Roll (2 pcs)', 'Crispy spring rolls',                                 50.00, 1),
    (5, 'Daal Chawal',         'Classic lentil curry with steamed rice',             100.00, 1),
    (5, 'Nihari',              'Slow-cooked beef stew with naan',                    220.00, 1),
    (5, 'Karahi (Half)',       'Spicy chicken karahi',                               250.00, 1),
    (6, 'Chicken Sandwich',    'Grilled chicken with veggies in bun',                120.00, 1),
    (6, 'Club Sandwich',       'Triple-decker sandwich with fries',                  180.00, 1);
END
GO

PRINT 'Database setup complete!';
GO
