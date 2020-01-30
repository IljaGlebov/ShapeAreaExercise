CREATE TABLE Category (Id integer PRIMARY KEY, Name varchar(255));
CREATE TABLE Product (Id integer PRIMARY KEY, Name varchar(255));
CREATE TABLE ProductXCategory (CategoryId integer NOT NULL,
                               ProductId integer NOT NULL,
                               FOREIGN KEY (CategoryId) REFERENCES Category(Id),
                               FOREIGN KEY (ProductId) REFERENCES Product(Id));
                               
INSERT INTO Category (Id, Name) VALUES (1, "Инструменты");
INSERT INTO Category (Id, Name) VALUES (2, "Косметика");
INSERT INTO Category (Id, Name) VALUES (3, "Для мужчин");
INSERT INTO Category (Id, Name) VALUES (4, "Для женщин");

INSERT INTO Product (Id, Name) VALUES (1, "Молоток");
INSERT INTO Product (Id, Name) VALUES (2, "Крем для бритья");
INSERT INTO Product (Id, Name) VALUES (3, "Крем для рук");
INSERT INTO Product (Id, Name) VALUES (4, "Колесо");

INSERT INTO ProductXCategory (ProductId, CategoryId) VALUES (1, 1);
INSERT INTO ProductXCategory (ProductId, CategoryId) VALUES (1, 3); 
INSERT INTO ProductXCategory (ProductId, CategoryId) VALUES (2, 2);
INSERT INTO ProductXCategory (ProductId, CategoryId) VALUES (2, 3); 
INSERT INTO ProductXCategory (ProductId, CategoryId) VALUES (3, 4); 



SELECT Product.Name, Category.Name FROM Product 
      LEFT JOIN ProductXCategory ON ProductXCategory.ProductId = Product.Id
      LEFT JOIN Category ON Category.Id = ProductXCategory.CategoryId
