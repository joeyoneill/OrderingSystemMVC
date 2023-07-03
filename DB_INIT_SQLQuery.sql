-- CONNECTED TO A SERVER WHERE 'tempdb' IS A DB
-- ELSE USE:
--      CREATE DATABASE tempdb;

-- CHOOSE TO USE 'tempdb' DB FROM SERVER
USE tempdb;

-- INIT 'Orders' TABLE
CREATE TABLE Orders(
    Id INT PRIMARY KEY IDENTITY,
    orderName NVARCHAR(50) NOT NULL,
    orderAddress NVARCHAR(100) NOT NULL,
    subtotal DECIMAL(10,2),
);

-- INIT 'Items' TABLE
CREATE TABLE Items(
    Id INT PRIMARY KEY IDENTITY,
    itemName NVARCHAR(50) NOT NULL,
    itemPrice DECIMAL(10,2) NOT NULL,
);

-- INIT 'OrdersItems' CONNECTING TABLE
-- RESOLVES MANY-TO-MANY RELATIONSHIP 
CREATE TABLE OrdersItems(
    Id INT PRIMARY KEY IDENTITY,
    OrderId INT,
    ItemId INT,
    FOREIGN KEY (OrderId) REFERENCES Orders(Id),
    FOREIGN KEY (ItemId) REFERENCES Items(Id)
);

INSERT INTO Items (itemName, itemPrice)
VALUES ('Water', 1.99);
INSERT INTO Items (itemName, itemPrice)
VALUES ('Milk', 2.99);

INSERT INTO Orders (orderName, orderAddress, subtotal)
VALUES ('Joey', '355 Test Street', 0.00);
INSERT INTO Orders (orderName, orderAddress, subtotal)
VALUES ('Austin', '356 Test Street', 0.00);

SELECT * FROM Items;
SELECT * FROM Orders;
SELECT * FROM OrdersItems;