CREATE TABLE Pizzas (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(50) NOT NULL,
    Description NVARCHAR(255),
    Price DECIMAL(5,2) NOT NULL
);

CREATE TABLE Ingredients (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(50) NOT NULL
);

CREATE TABLE PizzaIngredients (
    PizzaId INT NOT NULL,
    IngredientId INT NOT NULL,
    PRIMARY KEY (PizzaId, IngredientId),
    FOREIGN KEY (PizzaId) REFERENCES Pizzas(Id),
    FOREIGN KEY (IngredientId) REFERENCES Ingredients(Id)
);

CREATE TABLE Users (
    Id int PRIMARY KEY identity (1, 1) NOT NULL,
    Email NVARCHAR(50) NOT NULL unique,
    PasswordHash NVARCHAR(255) NOT NULL
);

CREATE TABLE Roles (
    Id INT IDENTITY(1,1) PRIMARY KEY,    
    Name NVARCHAR(100) NOT NULL UNIQUE    
);

CREATE TABLE UserRoles (
    UserId INT,                           
    RoleId INT,                           
    PRIMARY KEY (UserId, RoleId),         
    FOREIGN KEY (UserId) REFERENCES Users(Id),   
    FOREIGN KEY (RoleId) REFERENCES Roles(Id)  
);

CREATE TABLE Categories (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(50) NOT NULL UNIQUE
);

INSERT INTO Pizzas (Name, Description, Price) VALUES ('Margherita', 'La classica delle classiche', 7.99);
INSERT INTO Pizzas (Name, Description, Price) VALUES ('Diavola', 'La classica ma con quel qualcosa in più', 8.99);
INSERT INTO Pizzas (Name, Description, Price) VALUES ('Vegetariana', 'Per chi non mangia carne', 9.49);

INSERT INTO Ingredients (Name) VALUES ('Salsa di pomodoro');
INSERT INTO Ingredients (Name) VALUES ('Mozzarella');
INSERT INTO Ingredients (Name) VALUES ('Salame piccante');
INSERT INTO Ingredients (Name) VALUES ('Verdure miste');

INSERT INTO PizzaIngredients (PizzaID, IngredientID) VALUES (1, 1);
INSERT INTO PizzaIngredients (PizzaID, IngredientID) VALUES (1, 2);
INSERT INTO PizzaIngredients (PizzaID, IngredientID) VALUES (2, 1);
INSERT INTO PizzaIngredients (PizzaID, IngredientID) VALUES (2, 2);
INSERT INTO PizzaIngredients (PizzaID, IngredientID) VALUES (2, 3);
INSERT INTO PizzaIngredients (PizzaID, IngredientID) VALUES (3, 1);
INSERT INTO PizzaIngredients (PizzaID, IngredientID) VALUES (3, 2);
INSERT INTO PizzaIngredients (PizzaID, IngredientID) VALUES (3, 4);

INSERT INTO Roles (Name) VALUES ('Admin');
INSERT INTO Roles (Name) VALUES ('User');

INSERT INTO UserRoles (UserId, RoleId) VALUES (1, 1);  
INSERT INTO UserRoles (UserId, RoleId) VALUES (1, 2);
