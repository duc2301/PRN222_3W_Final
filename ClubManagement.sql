-- ============================================
-- Recipe Sharing Platform Database Schema
-- SQL Server - Structure Only
-- ============================================

USE master;
GO

-- Drop database if exists
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'RecipeSharingDB')
BEGIN
    ALTER DATABASE RecipeSharingDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE RecipeSharingDB;
END
GO

-- Create database
CREATE DATABASE RecipeSharingDB;
GO

USE RecipeSharingDB;
GO

-- Drop tables if exists (for clean setup)
IF OBJECT_ID('CommentLikes', 'U') IS NOT NULL DROP TABLE CommentLikes;
IF OBJECT_ID('Comments', 'U') IS NOT NULL DROP TABLE Comments;
IF OBJECT_ID('MealPlanRecipes', 'U') IS NOT NULL DROP TABLE MealPlanRecipes;
IF OBJECT_ID('MealPlans', 'U') IS NOT NULL DROP TABLE MealPlans;
IF OBJECT_ID('ShoppingListItems', 'U') IS NOT NULL DROP TABLE ShoppingListItems;
IF OBJECT_ID('ShoppingLists', 'U') IS NOT NULL DROP TABLE ShoppingLists;
IF OBJECT_ID('SavedRecipes', 'U') IS NOT NULL DROP TABLE SavedRecipes;
IF OBJECT_ID('RecipeLikes', 'U') IS NOT NULL DROP TABLE RecipeLikes;
IF OBJECT_ID('RecipeSteps', 'U') IS NOT NULL DROP TABLE RecipeSteps;
IF OBJECT_ID('RecipeIngredients', 'U') IS NOT NULL DROP TABLE RecipeIngredients;
IF OBJECT_ID('RecipeImages', 'U') IS NOT NULL DROP TABLE RecipeImages;
IF OBJECT_ID('Recipes', 'U') IS NOT NULL DROP TABLE Recipes;
IF OBJECT_ID('Followers', 'U') IS NOT NULL DROP TABLE Followers;
IF OBJECT_ID('Users', 'U') IS NOT NULL DROP TABLE Users;
IF OBJECT_ID('Categories', 'U') IS NOT NULL DROP TABLE Categories;
GO

-- ============================================
-- 1. Users Table
-- ============================================
CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    Password NVARCHAR(255) NOT NULL,
    FullName NVARCHAR(100),
    Bio NVARCHAR(500),
    ProfileImage NVARCHAR(255),
    Role Varchar(255) default 'User',
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    UpdatedAt DATETIME2 DEFAULT GETDATE(),
    IsActive BIT DEFAULT 1
);
GO

-- ============================================
-- 2. Categories Table
-- ============================================
CREATE TABLE Categories (
    CategoryID INT PRIMARY KEY IDENTITY(1,1),
    CategoryName NVARCHAR(50) NOT NULL UNIQUE,
    Description NVARCHAR(255),
    CreatedAt DATETIME2 DEFAULT GETDATE()
);
GO

-- ============================================
-- 3. Recipes Table
-- ============================================
CREATE TABLE Recipes (
    RecipeID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT NOT NULL,
    CategoryID INT,
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(1000),
    PrepTime INT,
    CookTime INT,
    Servings INT DEFAULT 4,
    Difficulty NVARCHAR(20),
    IsPublic BIT DEFAULT 1,
    ViewCount INT DEFAULT 0,
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    UpdatedAt DATETIME2 DEFAULT GETDATE(),
    CONSTRAINT FK_Recipes_Users FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE,
    CONSTRAINT FK_Recipes_Categories FOREIGN KEY (CategoryID) REFERENCES Categories(CategoryID) ON DELETE SET NULL
);
GO

-- ============================================
-- 4. Recipe Images Table
-- ============================================
CREATE TABLE RecipeImages (
    ImageID INT PRIMARY KEY IDENTITY(1,1),
    RecipeID INT NOT NULL,
    ImageUrl NVARCHAR(500) NOT NULL,
    ImageOrder INT DEFAULT 0,
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    CONSTRAINT FK_RecipeImages_Recipes FOREIGN KEY (RecipeID) REFERENCES Recipes(RecipeID) ON DELETE CASCADE
);
GO

-- ============================================
-- 5. Recipe Ingredients Table
-- ============================================
CREATE TABLE RecipeIngredients (
    IngredientID INT PRIMARY KEY IDENTITY(1,1),
    RecipeID INT NOT NULL,
    IngredientName NVARCHAR(100) NOT NULL,
    Quantity DECIMAL(10,2) NOT NULL,
    Unit NVARCHAR(50) NOT NULL,
    OrderIndex INT DEFAULT 0,
    CONSTRAINT FK_RecipeIngredients_Recipes FOREIGN KEY (RecipeID) REFERENCES Recipes(RecipeID) ON DELETE CASCADE
);
GO

-- ============================================
-- 6. Recipe Steps Table
-- ============================================
CREATE TABLE RecipeSteps (
    StepID INT PRIMARY KEY IDENTITY(1,1),
    RecipeID INT NOT NULL,
    StepNumber INT NOT NULL,
    StepDescription NVARCHAR(1000) NOT NULL,
    ImageUrl NVARCHAR(500),
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    CONSTRAINT FK_RecipeSteps_Recipes FOREIGN KEY (RecipeID) REFERENCES Recipes(RecipeID) ON DELETE CASCADE
);
GO

-- ============================================
-- 7. Recipe Likes Table
-- ============================================
CREATE TABLE RecipeLikes (
    LikeID INT PRIMARY KEY IDENTITY(1,1),
    RecipeID INT NOT NULL,
    UserID INT NOT NULL,
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    CONSTRAINT FK_RecipeLikes_Recipes FOREIGN KEY (RecipeID) REFERENCES Recipes(RecipeID) ON DELETE CASCADE,
    CONSTRAINT FK_RecipeLikes_Users FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE NO ACTION,
    CONSTRAINT UQ_RecipeLikes_RecipeUser UNIQUE(RecipeID, UserID)
);
GO

-- ============================================
-- 8. Saved Recipes Table
-- ============================================
CREATE TABLE SavedRecipes (
    SaveID INT PRIMARY KEY IDENTITY(1,1),
    RecipeID INT NOT NULL,
    UserID INT NOT NULL,
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    CONSTRAINT FK_SavedRecipes_Recipes FOREIGN KEY (RecipeID) REFERENCES Recipes(RecipeID) ON DELETE CASCADE,
    CONSTRAINT FK_SavedRecipes_Users FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE NO ACTION,
    CONSTRAINT UQ_SavedRecipes_RecipeUser UNIQUE(RecipeID, UserID)
);
GO

-- ============================================
-- 9. Comments Table
-- ============================================
CREATE TABLE Comments (
    CommentID INT PRIMARY KEY IDENTITY(1,1),
    RecipeID INT NOT NULL,
    UserID INT NOT NULL,
    ParentCommentID INT NULL,
    CommentText NVARCHAR(1000) NOT NULL,
    Rating INT,
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    UpdatedAt DATETIME2 DEFAULT GETDATE(),
    CONSTRAINT FK_Comments_Recipes FOREIGN KEY (RecipeID) REFERENCES Recipes(RecipeID) ON DELETE CASCADE,
    CONSTRAINT FK_Comments_Users FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE NO ACTION,
    CONSTRAINT FK_Comments_ParentComment FOREIGN KEY (ParentCommentID) REFERENCES Comments(CommentID) ON DELETE NO ACTION
);
GO

-- ============================================
-- 10. Comment Likes Table
-- ============================================
CREATE TABLE CommentLikes (
    CommentLikeID INT PRIMARY KEY IDENTITY(1,1),
    CommentID INT NOT NULL,
    UserID INT NOT NULL,
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    CONSTRAINT FK_CommentLikes_Comments FOREIGN KEY (CommentID) REFERENCES Comments(CommentID) ON DELETE CASCADE,
    CONSTRAINT FK_CommentLikes_Users FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE NO ACTION,
    CONSTRAINT UQ_CommentLikes_CommentUser UNIQUE(CommentID, UserID)
);
GO

-- ============================================
-- 11. Shopping Lists Table
-- ============================================
CREATE TABLE ShoppingLists (
    ShoppingListID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT NOT NULL,
    ListName NVARCHAR(100) DEFAULT N'My Shopping List',
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    UpdatedAt DATETIME2 DEFAULT GETDATE(),
    CONSTRAINT FK_ShoppingLists_Users FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE
);
GO

-- ============================================
-- 12. Shopping List Items Table
-- ============================================
CREATE TABLE ShoppingListItems (
    ItemID INT PRIMARY KEY IDENTITY(1,1),
    ShoppingListID INT NOT NULL,
    RecipeID INT NULL,
    IngredientName NVARCHAR(100) NOT NULL,
    Quantity DECIMAL(10,2) NOT NULL,
    Unit NVARCHAR(50) NOT NULL,
    Category NVARCHAR(50),
    IsChecked BIT DEFAULT 0,
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    CONSTRAINT FK_ShoppingListItems_ShoppingLists FOREIGN KEY (ShoppingListID) REFERENCES ShoppingLists(ShoppingListID) ON DELETE CASCADE,
    CONSTRAINT FK_ShoppingListItems_Recipes FOREIGN KEY (RecipeID) REFERENCES Recipes(RecipeID) ON DELETE NO ACTION
);
GO

-- ============================================
-- 13. Meal Plans Table
-- ============================================
CREATE TABLE MealPlans (
    MealPlanID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT NOT NULL,
    PlanDate DATE NOT NULL,
    MealType NVARCHAR(20),
    Notes NVARCHAR(500),
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    CONSTRAINT FK_MealPlans_Users FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE
);
GO

-- ============================================
-- 14. Meal Plan Recipes Table
-- ============================================
CREATE TABLE MealPlanRecipes (
    MealPlanRecipeID INT PRIMARY KEY IDENTITY(1,1),
    MealPlanID INT NOT NULL,
    RecipeID INT NOT NULL,
    Servings INT DEFAULT 4,
    CONSTRAINT FK_MealPlanRecipes_MealPlans FOREIGN KEY (MealPlanID) REFERENCES MealPlans(MealPlanID) ON DELETE CASCADE,
    CONSTRAINT FK_MealPlanRecipes_Recipes FOREIGN KEY (RecipeID) REFERENCES Recipes(RecipeID) ON DELETE NO ACTION
);
GO

-- ============================================
-- 15. Followers Table
-- ============================================
CREATE TABLE Followers (
    FollowID INT PRIMARY KEY IDENTITY(1,1),
    FollowerUserID INT NOT NULL,
    FollowingUserID INT NOT NULL,
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    
    CONSTRAINT FK_Followers_FollowerUser FOREIGN KEY (FollowerUserID) REFERENCES Users(UserID) ON DELETE CASCADE,
    
    CONSTRAINT FK_Followers_FollowingUser FOREIGN KEY (FollowingUserID) REFERENCES Users(UserID) ON DELETE NO ACTION,
    
    CONSTRAINT UQ_Followers_FollowerFollowing UNIQUE(FollowerUserID, FollowingUserID)
);
GO

-- ============================================
-- Create Indexes for Performance
-- ============================================

-- Users indexes
CREATE INDEX IX_Users_Email ON Users(Email);
CREATE INDEX IX_Users_Username ON Users(Username);

-- Recipes indexes
CREATE INDEX IX_Recipes_UserID ON Recipes(UserID);
CREATE INDEX IX_Recipes_CategoryID ON Recipes(CategoryID);
CREATE INDEX IX_Recipes_CreatedAt ON Recipes(CreatedAt DESC);
CREATE INDEX IX_Recipes_IsPublic ON Recipes(IsPublic);

-- RecipeImages indexes
CREATE INDEX IX_RecipeImages_RecipeID ON RecipeImages(RecipeID);
CREATE INDEX IX_RecipeImages_ImageOrder ON RecipeImages(RecipeID, ImageOrder);

-- RecipeIngredients indexes
CREATE INDEX IX_RecipeIngredients_RecipeID ON RecipeIngredients(RecipeID);

-- RecipeSteps indexes
CREATE INDEX IX_RecipeSteps_RecipeID ON RecipeSteps(RecipeID);
CREATE INDEX IX_RecipeSteps_StepNumber ON RecipeSteps(RecipeID, StepNumber);

-- RecipeLikes indexes
CREATE INDEX IX_RecipeLikes_RecipeID ON RecipeLikes(RecipeID);
CREATE INDEX IX_RecipeLikes_UserID ON RecipeLikes(UserID);

-- SavedRecipes indexes
CREATE INDEX IX_SavedRecipes_UserID ON SavedRecipes(UserID);
CREATE INDEX IX_SavedRecipes_RecipeID ON SavedRecipes(RecipeID);

-- Comments indexes
CREATE INDEX IX_Comments_RecipeID ON Comments(RecipeID);
CREATE INDEX IX_Comments_UserID ON Comments(UserID);
CREATE INDEX IX_Comments_ParentCommentID ON Comments(ParentCommentID);

-- CommentLikes indexes
CREATE INDEX IX_CommentLikes_CommentID ON CommentLikes(CommentID);
CREATE INDEX IX_CommentLikes_UserID ON CommentLikes(UserID);

-- ShoppingLists indexes
CREATE INDEX IX_ShoppingLists_UserID ON ShoppingLists(UserID);

-- ShoppingListItems indexes
CREATE INDEX IX_ShoppingListItems_ShoppingListID ON ShoppingListItems(ShoppingListID);
CREATE INDEX IX_ShoppingListItems_RecipeID ON ShoppingListItems(RecipeID);
CREATE INDEX IX_ShoppingListItems_Category ON ShoppingListItems(Category);

-- MealPlans indexes
CREATE INDEX IX_MealPlans_UserID ON MealPlans(UserID);
CREATE INDEX IX_MealPlans_PlanDate ON MealPlans(PlanDate);
CREATE INDEX IX_MealPlans_UserDate ON MealPlans(UserID, PlanDate);

-- MealPlanRecipes indexes
CREATE INDEX IX_MealPlanRecipes_MealPlanID ON MealPlanRecipes(MealPlanID);
CREATE INDEX IX_MealPlanRecipes_RecipeID ON MealPlanRecipes(RecipeID);

-- Followers indexes
CREATE INDEX IX_Followers_FollowerUserID ON Followers(FollowerUserID);
CREATE INDEX IX_Followers_FollowingUserID ON Followers(FollowingUserID);

GO

-- ============================================
-- INSERT SAMPLE DATA FOR TESTING
-- ============================================

-- ============================================
-- Insert Users (Password: 123456)
-- ============================================
SET IDENTITY_INSERT Users ON;
GO

INSERT INTO Users (UserID, Username, Email, Password, FullName, Bio, ProfileImage, CreatedAt, UpdatedAt, IsActive) VALUES
(1, N'chef_nguyen', N'nguyen@recipeshare.com', N'123456', N'Nguyễn Văn A', N'Đầu bếp chuyên về món Việt, yêu thích nấu ăn từ nhỏ', N'/images/users/user1.jpg', GETDATE(), GETDATE(), 1),
(2, N'bakerlinh', N'linh@recipeshare.com', N'123456', N'Trần Thị Linh', N'Chuyên làm bánh ngọt và bánh mì', N'/images/users/user2.jpg', GETDATE(), GETDATE(), 1),
(3, N'foodie_minh', N'minh@recipeshare.com', N'123456', N'Lê Minh', N'Thích khám phá các món ăn mới', N'/images/users/user3.jpg', GETDATE(), GETDATE(), 1),
(4, N'healthycook', N'trang@recipeshare.com', N'123456', N'Phạm Thị Trang', N'Nấu ăn healthy và organic', N'/images/users/user4.jpg', GETDATE(), GETDATE(), 1),
(5, N'asianchef', N'hung@recipeshare.com', N'123456', N'Hoàng Văn Hùng', N'Đầu bếp món Á chuyên nghiệp', N'/images/users/user5.jpg', GETDATE(), GETDATE(), 1),
(6, N'dessertlover', N'mai@recipeshare.com', N'123456', N'Vũ Thị Mai', N'Yêu thích làm tráng miệng', N'/images/users/user6.jpg', GETDATE(), GETDATE(), 1),
(7, N'homecook', N'thanh@recipeshare.com', N'123456', N'Đỗ Văn Thành', N'Nấu ăn gia đình đơn giản', N'/images/users/user7.jpg', GETDATE(), GETDATE(), 1),
(8, N'veganchef', N'hoa@recipeshare.com', N'123456', N'Ngô Thị Hoa', N'Chuyên món chay và vegan', N'/images/users/user8.jpg', GETDATE(), GETDATE(), 1);
GO

INSERT INTO Users (UserID, Username, Email, Password, FullName, Bio, ProfileImage, Role, CreatedAt, UpdatedAt, IsActive) VALUES
(9, N'admin', N'admin@gmail.com', N'1', N'Quản trị viên', N'', N'https://marketplace.canva.com/wCodY/MAG6WgwCodY/1/tl/canva-male-avatar-illustration-MAG6WgwCodY.png', N'Admin', GETDATE(), GETDATE(), 1)

SET IDENTITY_INSERT Users OFF;
GO

-- ============================================
-- Insert Categories
-- ============================================
SET IDENTITY_INSERT Categories ON;
GO

INSERT INTO Categories (CategoryID, CategoryName, Description, CreatedAt) VALUES
(1, N'Món Việt', N'Các món ăn truyền thống Việt Nam', GETDATE()),
(2, N'Món Á', N'Các món ăn châu Á', GETDATE()),
(3, N'Món Âu', N'Các món ăn châu Âu', GETDATE()),
(4, N'Bánh ngọt', N'Các loại bánh ngọt, bánh kem', GETDATE()),
(5, N'Món chay', N'Các món ăn chay, vegan', GETDATE()),
(6, N'Đồ uống', N'Các loại đồ uống, sinh tố, nước ép', GETDATE()),
(7, N'Tráng miệng', N'Các món tráng miệng', GETDATE()),
(8, N'Món nhanh', N'Các món ăn nhanh, tiện lợi', GETDATE()),
(9, N'Món lẩu', N'Các loại lẩu', GETDATE()),
(10, N'Salad', N'Các món salad tươi mát', GETDATE());
GO

SET IDENTITY_INSERT Categories OFF;
GO

-- ============================================
-- Insert Recipes
-- ============================================
SET IDENTITY_INSERT Recipes ON;
GO

INSERT INTO Recipes (RecipeID, UserID, CategoryID, Title, Description, PrepTime, CookTime, Servings, Difficulty, IsPublic, ViewCount, CreatedAt, UpdatedAt) VALUES
(1, 1, 1, N'Phở Bò Hà Nội', N'Món phở bò truyền thống Hà Nội với nước dùng trong veo, thơm ngon', 30, 120, 4, N'Khó', 1, 250, GETDATE(), GETDATE()),
(2, 1, 1, N'Bún Chả Hà Nội', N'Bún chả nướng thơm phức với nước mắm chua ngọt đặc trưng', 20, 30, 2, N'Trung bình', 1, 180, GETDATE(), GETDATE()),
(3, 2, 4, N'Bánh Mì Sandwich', N'Bánh mì giòn rụm với nhân thịt nguội và rau củ', 15, 0, 2, N'Dễ', 1, 320, GETDATE(), GETDATE()),
(4, 2, 4, N'Bánh Flan Caramel', N'Bánh flan mềm mịn với lớp caramel đắng ngọt', 15, 45, 6, N'Trung bình', 1, 190, GETDATE(), GETDATE()),
(5, 3, 2, N'Pad Thai', N'Món mì xào Thái Lan chua ngọt đặc trưng', 10, 15, 2, N'Trung bình', 1, 210, GETDATE(), GETDATE()),
(6, 4, 10, N'Salad Ức Gà', N'Salad tươi mát với ức gà nướng, giàu protein', 20, 10, 2, N'Dễ', 1, 160, GETDATE(), GETDATE()),
(7, 4, 5, N'Đậu Hũ Sốt Cà', N'Món đậu hũ chiên giòn sốt cà chua thơm ngon', 10, 20, 3, N'Dễ', 1, 140, GETDATE(), GETDATE()),
(8, 5, 2, N'Sushi Cuộn', N'Sushi cuộn Nhật Bản với cá hồi tươi', 30, 0, 4, N'Khó', 1, 280, GETDATE(), GETDATE()),
(9, 5, 2, N'Lẩu Thái Tom Yum', N'Lẩu Thái chua cay đặc trưng', 25, 30, 4, N'Trung bình', 1, 310, GETDATE(), GETDATE()),
(10, 6, 7, N'Chè Khúc Bạch', N'Chè khúc bạch mát lạnh, thanh mát', 30, 0, 4, N'Trung bình', 1, 95, GETDATE(), GETDATE()),
(11, 6, 4, N'Tiramisu', N'Bánh Tiramisu Ý ngọt ngào với vị cà phê đậm đà', 20, 0, 8, N'Trung bình', 1, 220, GETDATE(), GETDATE()),
(12, 7, 8, N'Mì Ý Spaghetti Bò Băm', N'Mì Ý sốt bò băm đơn giản dễ làm', 10, 25, 3, N'Dễ', 1, 175, GETDATE(), GETDATE()),
(13, 7, 1, N'Cơm Tấm Sườn Nướng', N'Cơm tấm với sườn nướng thơm lừng', 15, 25, 2, N'Dễ', 1, 260, GETDATE(), GETDATE()),
(14, 8, 5, N'Gỏi Cuốn Chay', N'Gỏi cuốn chay tươi mát với rau củ', 20, 0, 4, N'Dễ', 1, 130, GETDATE(), GETDATE()),
(15, 8, 5, N'Bún Riêu Chay', N'Bún riêu chay thanh đạm, thơm ngon', 15, 30, 3, N'Trung bình', 1, 110, GETDATE(), GETDATE()),
(16, 1, 1, N'Bánh Xèo Miền Tây', N'Bánh xèo giòn tan với nhân tôm thịt', 20, 20, 3, N'Trung bình', 1, 200, GETDATE(), GETDATE()),
(17, 3, 6, N'Sinh Tố Bơ', N'Sinh tố bơ béo ngậy thơm ngon', 5, 0, 2, N'Dễ', 1, 85, GETDATE(), GETDATE()),
(18, 2, 4, N'Bánh Bông Lan', N'Bánh bông lan mềm xốp thơm phức', 15, 30, 8, N'Trung bình', 1, 145, GETDATE(), GETDATE()),
(19, 5, 9, N'Lẩu Bò Nhúng Dấm', N'Lẩu bò nhúng dấm chua ngọt đặc biệt', 30, 20, 4, N'Trung bình', 1, 195, GETDATE(), GETDATE()),
(20, 4, 10, N'Salad Trộn Dầu Giấm', N'Salad rau củ tươi ngon với dầu giấm', 15, 0, 3, N'Dễ', 1, 120, GETDATE(), GETDATE());
GO

SET IDENTITY_INSERT Recipes OFF;
GO

-- ============================================
-- Insert Recipe Ingredients
-- ============================================
SET IDENTITY_INSERT RecipeIngredients ON;
GO

INSERT INTO RecipeIngredients (IngredientID, RecipeID, IngredientName, Quantity, Unit, OrderIndex) VALUES
-- Phở Bò (RecipeID = 1)
(1, 1, N'Xương bò', 1.5, N'kg', 1),
(2, 1, N'Thịt bò', 500, N'g', 2),
(3, 1, N'Bánh phở', 500, N'g', 3),
(4, 1, N'Hành tây', 2, N'củ', 4),
(5, 1, N'Gừng', 50, N'g', 5),
(6, 1, N'Hoa hồi', 3, N'cái', 6),
(7, 1, N'Quế', 1, N'thanh', 7),
(8, 1, N'Nước mắm', 3, N'thìa', 8),

-- Bún Chả (RecipeID = 2)
(9, 2, N'Thịt ba chỉ', 300, N'g', 1),
(10, 2, N'Bún tươi', 300, N'g', 2),
(11, 2, N'Nước mắm', 4, N'thìa', 3),
(12, 2, N'Đường', 2, N'thìa', 4),
(13, 2, N'Tỏi', 3, N'tép', 5),
(14, 2, N'Rau thơm', 1, N'bó', 6),

-- Bánh Mì (RecipeID = 3)
(15, 3, N'Bánh mì', 2, N'ổ', 1),
(16, 3, N'Pate', 50, N'g', 2),
(17, 3, N'Chả lụa', 100, N'g', 3),
(18, 3, N'Dưa leo', 1, N'quả', 4),
(19, 3, N'Ngò rí', 20, N'g', 5),

-- Bánh Flan (RecipeID = 4)
(20, 4, N'Trứng gà', 4, N'quả', 1),
(21, 4, N'Sữa đặc', 100, N'ml', 2),
(22, 4, N'Sữa tươi', 200, N'ml', 3),
(23, 4, N'Đường', 100, N'g', 4),
(24, 4, N'Vani', 1, N'thìa', 5),

-- Pad Thai (RecipeID = 5)
(25, 5, N'Bánh phở', 200, N'g', 1),
(26, 5, N'Tôm', 150, N'g', 2),
(27, 5, N'Trứng', 2, N'quả', 3),
(28, 5, N'Giá đỗ', 100, N'g', 4),
(29, 5, N'Đậu phộng rang', 50, N'g', 5),
(30, 5, N'Tương ớt', 2, N'thìa', 6),

-- Salad Ức Gà (RecipeID = 6)
(31, 6, N'Ức gà', 200, N'g', 1),
(32, 6, N'Xà lách', 100, N'g', 2),
(33, 6, N'Cà chua', 2, N'quả', 3),
(34, 6, N'Dưa leo', 1, N'quả', 4),
(35, 6, N'Dầu olive', 2, N'thìa', 5),
(36, 6, N'Nước chanh', 1, N'thìa', 6),

-- Đậu Hũ Sốt Cà (RecipeID = 7)
(37, 7, N'Đậu hũ', 300, N'g', 1),
(38, 7, N'Cà chua', 3, N'quả', 2),
(39, 7, N'Tỏi', 2, N'tép', 3),
(40, 7, N'Hành tây', 1, N'củ', 4),
(41, 7, N'Tương ớt', 1, N'thìa', 5),

-- Sushi (RecipeID = 8)
(42, 8, N'Gạo Nhật', 300, N'g', 1),
(43, 8, N'Cá hồi', 200, N'g', 2),
(44, 8, N'Rong biển', 5, N'lá', 3),
(45, 8, N'Dưa leo', 1, N'quả', 4),
(46, 8, N'Giấm', 2, N'thìa', 5),
(47, 8, N'Wasabi', 10, N'g', 6),

-- Lẩu Thái (RecipeID = 9)
(48, 9, N'Tôm', 300, N'g', 1),
(49, 9, N'Mực', 200, N'g', 2),
(50, 9, N'Nấm', 150, N'g', 3),
(51, 9, N'Sả', 3, N'cây', 4),
(52, 9, N'Ớt', 5, N'quả', 5),
(53, 9, N'Kem chua', 100, N'ml', 6),

-- Chè Khúc Bạch (RecipeID = 10)
(54, 10, N'Thạch rau câu', 200, N'g', 1),
(55, 10, N'Đường phên', 100, N'g', 2),
(56, 10, N'Nước cốt dừa', 200, N'ml', 3),
(57, 10, N'Vải thiều', 100, N'g', 4),

-- Tiramisu (RecipeID = 11)
(58, 11, N'Phô mai Mascarpone', 250, N'g', 1),
(59, 11, N'Trứng gà', 3, N'quả', 2),
(60, 11, N'Đường', 80, N'g', 3),
(61, 11, N'Cà phê espresso', 150, N'ml', 4),
(62, 11, N'Bánh Ladyfinger', 200, N'g', 5),
(63, 11, N'Bột cacao', 20, N'g', 6),

-- Spaghetti (RecipeID = 12)
(64, 12, N'Mì Spaghetti', 300, N'g', 1),
(65, 12, N'Thịt bò băm', 200, N'g', 2),
(66, 12, N'Cà chua', 4, N'quả', 3),
(67, 12, N'Tỏi', 3, N'tép', 4),
(68, 12, N'Hành tây', 1, N'củ', 5),

-- Cơm Tấm (RecipeID = 13)
(69, 13, N'Cơm tấm', 300, N'g', 1),
(70, 13, N'Sườn heo', 300, N'g', 2),
(71, 13, N'Mỡ hành', 2, N'thìa', 3),
(72, 13, N'Đồ chua', 100, N'g', 4),
(73, 13, N'Nước mắm', 2, N'thìa', 5),

-- Gỏi Cuốn Chay (RecipeID = 14)
(74, 14, N'Bánh tráng', 10, N'tờ', 1),
(75, 14, N'Bún tươi', 100, N'g', 2),
(76, 14, N'Xà lách', 50, N'g', 3),
(77, 14, N'Rau thơm', 50, N'g', 4),
(78, 14, N'Dưa leo', 1, N'quả', 5),

-- Bún Riêu Chay (RecipeID = 15)
(79, 15, N'Bún tươi', 300, N'g', 1),
(80, 15, N'Đậu hũ', 200, N'g', 2),
(81, 15, N'Cà chua', 3, N'quả', 3),
(82, 15, N'Mẻ', 2, N'thìa', 4),
(83, 15, N'Rau muống', 100, N'g', 5),

-- Bánh Xèo (RecipeID = 16)
(84, 16, N'Bột bánh xèo', 200, N'g', 1),
(85, 16, N'Tôm', 150, N'g', 2),
(86, 16, N'Thịt ba chỉ', 100, N'g', 3),
(87, 16, N'Giá đỗ', 100, N'g', 4),
(88, 16, N'Nước cốt dừa', 100, N'ml', 5),

-- Sinh Tố Bơ (RecipeID = 17)
(89, 17, N'Bơ', 2, N'quả', 1),
(90, 17, N'Sữa đặc', 50, N'ml', 2),
(91, 17, N'Đá', 100, N'g', 3),

-- Bánh Bông Lan (RecipeID = 18)
(92, 18, N'Trứng gà', 6, N'quả', 1),
(93, 18, N'Bột mì', 200, N'g', 2),
(94, 18, N'Đường', 150, N'g', 3),
(95, 18, N'Bơ', 50, N'g', 4),
(96, 18, N'Vani', 1, N'thìa', 5),

-- Lẩu Bò (RecipeID = 19)
(97, 19, N'Thịt bò', 500, N'g', 1),
(98, 19, N'Dấm', 100, N'ml', 2),
(99, 19, N'Sả', 3, N'cây', 3),
(100, 19, N'Rau ăn lẩu', 300, N'g', 4),

-- Salad Trộn (RecipeID = 20)
(101, 20, N'Xà lách', 100, N'g', 1),
(102, 20, N'Cà chua', 2, N'quả', 2),
(103, 20, N'Dưa leo', 1, N'quả', 3),
(104, 20, N'Dầu olive', 2, N'thìa', 4),
(105, 20, N'Giấm', 1, N'thìa', 5);
GO

SET IDENTITY_INSERT RecipeIngredients OFF;
GO

-- ============================================
-- Insert Recipe Steps
-- ============================================
SET IDENTITY_INSERT RecipeSteps ON;
GO

INSERT INTO RecipeSteps (StepID, RecipeID, StepNumber, StepDescription, ImageUrl, CreatedAt) VALUES
-- Phở Bò (RecipeID = 1)
(1, 1, 1, N'Luộc xương bò sơ qua, rửa sạch để loại bỏ tạp chất', NULL, GETDATE()),
(2, 1, 2, N'Nướng hành, gừng thơm, cho vào nồi nước dùng cùng xương', NULL, GETDATE()),
(3, 1, 3, N'Hầm nước dùng trong 2-3 tiếng lửa nhỏ', NULL, GETDATE()),
(4, 1, 4, N'Thái thịt bò mỏng, trần bánh phở', NULL, GETDATE()),
(5, 1, 5, N'Cho bánh phở, thịt bò vào tô, chan nước dùng nóng, thêm rau thơm', NULL, GETDATE()),

-- Bún Chả (RecipeID = 2)
(6, 2, 1, N'Ướp thịt với nước mắm, đường, tiêu, tỏi băm', NULL, GETDATE()),
(7, 2, 2, N'Nướng thịt trên than hoa cho thơm', NULL, GETDATE()),
(8, 2, 3, N'Pha nước mắm chua ngọt với đường, giấm, tỏi, ớt', NULL, GETDATE()),
(9, 2, 4, N'Bày bún, rau thơm, chả nướng ra đĩa, chan nước mắm', NULL, GETDATE()),

-- Bánh Mì (RecipeID = 3)
(10, 3, 1, N'Nướng bánh mì cho giòn', NULL, GETDATE()),
(11, 3, 2, N'Phết pate lên bánh', NULL, GETDATE()),
(12, 3, 3, N'Cho chả lụa, dưa leo, rau thơm vào bánh', NULL, GETDATE()),

-- Bánh Flan (RecipeID = 4)
(13, 4, 1, N'Làm caramel: đun đường với chút nước cho chảy màu vàng nâu', NULL, GETDATE()),
(14, 4, 2, N'Đánh trứng với sữa đặc, sữa tươi, vani', NULL, GETDATE()),
(15, 4, 3, N'Rót hỗn hợp vào khuôn đã có caramel', NULL, GETDATE()),
(16, 4, 4, N'Hấp cách thủy 45 phút lửa nhỏ', NULL, GETDATE()),

-- Pad Thai (RecipeID = 5)
(17, 5, 1, N'Ngâm bánh phở cho mềm', NULL, GETDATE()),
(18, 5, 2, N'Xào tôm với trứng', NULL, GETDATE()),
(19, 5, 3, N'Cho bánh phở vào xào cùng tương ớt, nước mắm', NULL, GETDATE()),
(20, 5, 4, N'Thêm giá đỗ, đậu phộng rang, chanh', NULL, GETDATE()),

-- Salad Ức Gà (RecipeID = 6)
(21, 6, 1, N'Nướng ức gà với chút muối tiêu', NULL, GETDATE()),
(22, 6, 2, N'Thái rau củ', NULL, GETDATE()),
(23, 6, 3, N'Trộn rau với dầu olive, chanh, muối', NULL, GETDATE()),
(24, 6, 4, N'Thái gà, cho lên salad', NULL, GETDATE()),

-- Đậu Hũ Sốt Cà (RecipeID = 7)
(25, 7, 1, N'Chiên đậu hũ vàng giòn', NULL, GETDATE()),
(26, 7, 2, N'Xào tỏi, hành tây thơm', NULL, GETDATE()),
(27, 7, 3, N'Cho cà chua vào xào nhuyễn', NULL, GETDATE()),
(28, 7, 4, N'Cho đậu hũ vào đảo đều với sốt', NULL, GETDATE()),

-- Sushi (RecipeID = 8)
(29, 8, 1, N'Nấu cơm Nhật, trộn giấm', NULL, GETDATE()),
(30, 8, 2, N'Trải rong biển lên thảm tre', NULL, GETDATE()),
(31, 8, 3, N'Phủ cơm lên rong biển', NULL, GETDATE()),
(32, 8, 4, N'Đặt cá hồi, dưa leo lên cơm, cuộn tròn', NULL, GETDATE()),
(33, 8, 5, N'Thái sushi thành miếng vừa ăn', NULL, GETDATE()),

-- Lẩu Thái (RecipeID = 9)
(34, 9, 1, N'Đun nước dùng với sả, ớt, nấm', NULL, GETDATE()),
(35, 9, 2, N'Cho kem chua, nước mắm vào nồi', NULL, GETDATE()),
(36, 9, 3, N'Nhúng tôm, mực vào lẩu khi nước sôi', NULL, GETDATE()),

-- Chè Khúc Bạch (RecipeID = 10)
(37, 10, 1, N'Làm thạch rau câu trắng, cắt hạt lựu', NULL, GETDATE()),
(38, 10, 2, N'Nấu nước đường phên', NULL, GETDATE()),
(39, 10, 3, N'Cho thạch, nước cốt dừa, vải vào ly', NULL, GETDATE()),

-- Tiramisu (RecipeID = 11)
(40, 11, 1, N'Đánh trứng với đường cho bông', NULL, GETDATE()),
(41, 11, 2, N'Trộn phô mai mascarpone với trứng', NULL, GETDATE()),
(42, 11, 3, N'Nhúng bánh ladyfinger vào cà phê', NULL, GETDATE()),
(43, 11, 4, N'Xếp lớp bánh, lớp kem luân phiên', NULL, GETDATE()),
(44, 11, 5, N'Rắc bột cacao lên trên, ướp lạnh 4 giờ', NULL, GETDATE()),

-- Spaghetti (RecipeID = 12)
(45, 12, 1, N'Luộc mì spaghetti chín vừa', NULL, GETDATE()),
(46, 12, 2, N'Xào tỏi, hành tây thơm', NULL, GETDATE()),
(47, 12, 3, N'Cho thịt bò băm xào chín', NULL, GETDATE()),
(48, 12, 4, N'Cho cà chua nghiền vào nấu sốt', NULL, GETDATE()),
(49, 12, 5, N'Trộn mì với sốt, rắc phô mai', NULL, GETDATE()),

-- Cơm Tấm (RecipeID = 13)
(50, 13, 1, N'Ướp sườn với nước mắm, đường, tỏi, sả', NULL, GETDATE()),
(51, 13, 2, N'Nướng sườn trên than hoa', NULL, GETDATE()),
(52, 13, 3, N'Bày cơm tấm, sườn, đồ chua ra đĩa', NULL, GETDATE()),

-- Gỏi Cuốn Chay (RecipeID = 14)
(53, 14, 1, N'Ngâm bánh tráng cho mềm', NULL, GETDATE()),
(54, 14, 2, N'Đặt rau, bún lên bánh tráng', NULL, GETDATE()),
(55, 14, 3, N'Cuộn tròn chặt tay', NULL, GETDATE()),

-- Bún Riêu Chay (RecipeID = 15)
(56, 15, 1, N'Nấu nước dùng từ cà chua, mẻ', NULL, GETDATE()),
(57, 15, 2, N'Chiên đậu hũ giòn', NULL, GETDATE()),
(58, 15, 3, N'Cho bún vào tô, chan nước dùng, thêm rau', NULL, GETDATE()),

-- Bánh Xèo (RecipeID = 16)
(59, 16, 1, N'Pha bột bánh xèo với nước cốt dừa', NULL, GETDATE()),
(60, 16, 2, N'Xào tôm, thịt với giá', NULL, GETDATE()),
(61, 16, 3, N'Chiên bánh xèo giòn với nhân tôm thịt', NULL, GETDATE()),

-- Sinh Tố Bơ (RecipeID = 17)
(62, 17, 1, N'Bỏ bơ, sữa đặc, đá vào máy xay', NULL, GETDATE()),
(63, 17, 2, N'Xay mịn, rót ra ly', NULL, GETDATE()),

-- Bánh Bông Lan (RecipeID = 18)
(64, 18, 1, N'Đánh trứng với đường cho bông', NULL, GETDATE()),
(65, 18, 2, N'Trộn bột mì, bơ vào hỗn hợp trứng', NULL, GETDATE()),
(66, 18, 3, N'Rót vào khuôn, nướng 170°C trong 30 phút', NULL, GETDATE()),

-- Lẩu Bò (RecipeID = 19)
(67, 19, 1, N'Thái thịt bò mỏng', NULL, GETDATE()),
(68, 19, 2, N'Nấu nước dùng với dấm, sả', NULL, GETDATE()),
(69, 19, 3, N'Nhúng bò vào lẩu khi sôi', NULL, GETDATE()),

-- Salad Trộn (RecipeID = 20)
(70, 20, 1, N'Rửa rau sạch, để ráo', NULL, GETDATE()),
(71, 20, 2, N'Thái rau củ', NULL, GETDATE()),
(72, 20, 3, N'Trộn với dầu olive, giấm, muối tiêu', NULL, GETDATE());
GO

SET IDENTITY_INSERT RecipeSteps OFF;
GO

-- ============================================
-- Insert Recipe Likes
-- ============================================
SET IDENTITY_INSERT RecipeLikes ON;
GO

INSERT INTO RecipeLikes (LikeID, RecipeID, UserID, CreatedAt) VALUES
(1, 1, 2, GETDATE()),
(2, 1, 3, GETDATE()),
(3, 1, 4, GETDATE()),
(4, 1, 5, GETDATE()),
(5, 2, 3, GETDATE()),
(6, 2, 4, GETDATE()),
(7, 3, 1, GETDATE()),
(8, 3, 4, GETDATE()),
(9, 3, 5, GETDATE()),
(10, 4, 1, GETDATE()),
(11, 4, 3, GETDATE()),
(12, 5, 2, GETDATE()),
(13, 5, 6, GETDATE()),
(14, 6, 3, GETDATE()),
(15, 6, 7, GETDATE()),
(16, 7, 4, GETDATE()),
(17, 8, 1, GETDATE()),
(18, 8, 3, GETDATE()),
(19, 9, 2, GETDATE()),
(20, 9, 4, GETDATE()),
(21, 10, 5, GETDATE()),
(22, 11, 1, GETDATE()),
(23, 11, 4, GETDATE()),
(24, 12, 2, GETDATE()),
(25, 13, 3, GETDATE()),
(26, 13, 6, GETDATE()),
(27, 14, 4, GETDATE()),
(28, 15, 5, GETDATE()),
(29, 16, 2, GETDATE()),
(30, 17, 6, GETDATE()),
(31, 18, 3, GETDATE()),
(32, 19, 4, GETDATE()),
(33, 20, 5, GETDATE());
GO

SET IDENTITY_INSERT RecipeLikes OFF;
GO

-- ============================================
-- Insert Saved Recipes
-- ============================================
SET IDENTITY_INSERT SavedRecipes ON;
GO

INSERT INTO SavedRecipes (SaveID, RecipeID, UserID, CreatedAt) VALUES
(1, 1, 3, GETDATE()),
(2, 1, 4, GETDATE()),
(3, 2, 4, GETDATE()),
(4, 3, 2, GETDATE()),
(5, 5, 1, GETDATE()),
(6, 6, 2, GETDATE()),
(7, 8, 2, GETDATE()),
(8, 9, 1, GETDATE()),
(9, 11, 3, GETDATE()),
(10, 13, 4, GETDATE()),
(11, 14, 3, GETDATE()),
(12, 16, 5, GETDATE()),
(13, 18, 6, GETDATE()),
(14, 20, 7, GETDATE());
GO

SET IDENTITY_INSERT SavedRecipes OFF;
GO

-- ============================================
-- Insert Comments
-- ============================================
SET IDENTITY_INSERT Comments ON;
GO

INSERT INTO Comments (CommentID, RecipeID, UserID, ParentCommentID, CommentText, Rating, CreatedAt, UpdatedAt) VALUES
(1, 1, 2, NULL, N'Công thức rất chi tiết, phở rất ngon!', 5, GETDATE(), GETDATE()),
(2, 1, 3, NULL, N'Nước dùng trong vắt, thơm lừng!', 5, GETDATE(), GETDATE()),
(3, 1, 4, 1, N'Mình cũng làm theo rất thành công', NULL, GETDATE(), GETDATE()),
(4, 2, 3, NULL, N'Bún chả đậm đà, rất chuẩn Hà Nội', 5, GETDATE(), GETDATE()),
(5, 3, 1, NULL, N'Bánh mì giòn ngon tuyệt vời', 4, GETDATE(), GETDATE()),
(6, 4, 3, NULL, N'Bánh flan mềm mịn, caramel vừa đắng vừa ngọt', 5, GETDATE(), GETDATE()),
(7, 5, 2, NULL, N'Pad Thai rất ngon, đúng vị Thái', 4, GETDATE(), GETDATE()),
(8, 6, 3, NULL, N'Salad tươi mát, ức gà mềm', 5, GETDATE(), GETDATE()),
(9, 7, 5, NULL, N'Món chay ngon, đậu hũ giòn tan', 4, GETDATE(), GETDATE()),
(10, 8, 3, NULL, N'Sushi đẹp mắt và ngon', 5, GETDATE(), GETDATE()),
(11, 9, 4, NULL, N'Lẩu Thái chua cay đúng điệu', 5, GETDATE(), GETDATE()),
(12, 10, 6, NULL, N'Chè mát lạnh, thanh mát', 4, GETDATE(), GETDATE()),
(13, 11, 2, NULL, N'Tiramisu ngon không thua tiệm', 5, GETDATE(), GETDATE()),
(14, 12, 3, NULL, N'Mì Ý đơn giản dễ làm', 4, GETDATE(), GETDATE()),
(15, 13, 5, NULL, N'Cơm tấm thơm ngon, sườn mềm', 5, GETDATE(), GETDATE()),
(16, 14, 6, NULL, N'Gỏi cuốn chay tươi mát lắm', 4, GETDATE(), GETDATE()),
(17, 15, 7, NULL, N'Bún riêu chay thanh đạm nhưng ngon', 4, GETDATE(), GETDATE()),
(18, 16, 3, NULL, N'Bánh xèo giòn rụm, nhân đầy đặn', 5, GETDATE(), GETDATE()),
(19, 17, 4, NULL, N'Sinh tố bơ béo ngậy', 4, GETDATE(), GETDATE()),
(20, 18, 5, NULL, N'Bánh bông lan mềm xốp', 5, GETDATE(), GETDATE());
GO

SET IDENTITY_INSERT Comments OFF;
GO

-- ============================================
-- Insert Comment Likes
-- ============================================
SET IDENTITY_INSERT CommentLikes ON;
GO

INSERT INTO CommentLikes (CommentLikeID, CommentID, UserID, CreatedAt) VALUES
(1, 1, 3, GETDATE()),
(2, 1, 4, GETDATE()),
(3, 2, 5, GETDATE()),
(4, 4, 4, GETDATE()),
(5, 5, 2, GETDATE()),
(6, 6, 1, GETDATE()),
(7, 7, 3, GETDATE()),
(8, 8, 4, GETDATE()),
(9, 10, 2, GETDATE()),
(10, 11, 3, GETDATE()),
(11, 13, 4, GETDATE()),
(12, 15, 6, GETDATE()),
(13, 18, 4, GETDATE());
GO

SET IDENTITY_INSERT CommentLikes OFF;
GO

-- ============================================
-- Insert Shopping Lists
-- ============================================
SET IDENTITY_INSERT ShoppingLists ON;
GO

INSERT INTO ShoppingLists (ShoppingListID, UserID, ListName, CreatedAt, UpdatedAt) VALUES
(1, 1, N'Danh sách mua sắm tuần này', GETDATE(), GETDATE()),
(2, 2, N'Nguyên liệu làm bánh', GETDATE(), GETDATE()),
(3, 3, N'Mua sắm cuối tuần', GETDATE(), GETDATE()),
(4, 4, N'Nguyên liệu cho salad', GETDATE(), GETDATE());
GO

SET IDENTITY_INSERT ShoppingLists OFF;
GO

-- ============================================
-- Insert Shopping List Items
-- ============================================
SET IDENTITY_INSERT ShoppingListItems ON;
GO

INSERT INTO ShoppingListItems (ItemID, ShoppingListID, RecipeID, IngredientName, Quantity, Unit, Category, IsChecked, CreatedAt) VALUES
(1, 1, 1, N'Xương bò', 1.5, N'kg', N'Thịt', 0, GETDATE()),
(2, 1, 1, N'Thịt bò', 500, N'g', N'Thịt', 0, GETDATE()),
(3, 1, 1, N'Bánh phở', 500, N'g', N'Tinh bột', 1, GETDATE()),
(4, 2, 4, N'Trứng gà', 4, N'quả', N'Trứng', 0, GETDATE()),
(5, 2, 4, N'Sữa đặc', 100, N'ml', N'Sữa', 0, GETDATE()),
(6, 3, 5, N'Bánh phở', 200, N'g', N'Tinh bột', 0, GETDATE()),
(7, 3, 5, N'Tôm', 150, N'g', N'Hải sản', 0, GETDATE()),
(8, 4, 6, N'Ức gà', 200, N'g', N'Thịt', 1, GETDATE()),
(9, 4, 6, N'Xà lách', 100, N'g', N'Rau', 1, GETDATE()),
(10, 4, 6, N'Cà chua', 2, N'quả', N'Rau', 0, GETDATE());
GO

SET IDENTITY_INSERT ShoppingListItems OFF;
GO

-- ============================================
-- Insert Meal Plans
-- ============================================
SET IDENTITY_INSERT MealPlans ON;
GO

INSERT INTO MealPlans (MealPlanID, UserID, PlanDate, MealType, Notes, CreatedAt) VALUES
(1, 1, CAST(GETDATE() AS DATE), N'Sáng', N'Bữa sáng nhẹ nhàng', GETDATE()),
(2, 1, CAST(GETDATE() AS DATE), N'Trưa', N'Bữa trưa đầy đủ dinh dưỡng', GETDATE()),
(3, 1, CAST(GETDATE() AS DATE), N'Tối', N'Bữa tối nhẹ', GETDATE()),
(4, 2, CAST(DATEADD(DAY, 1, GETDATE()) AS DATE), N'Sáng', N'Sáng mai làm bánh', GETDATE()),
(5, 2, CAST(DATEADD(DAY, 1, GETDATE()) AS DATE), N'Trưa', NULL, GETDATE()),
(6, 3, CAST(GETDATE() AS DATE), N'Trưa', N'Hôm nay ăn Pad Thai', GETDATE()),
(7, 4, CAST(GETDATE() AS DATE), N'Tối', N'Tối nay ăn salad', GETDATE());
GO

SET IDENTITY_INSERT MealPlans OFF;
GO

-- ============================================
-- Insert Meal Plan Recipes
-- ============================================
SET IDENTITY_INSERT MealPlanRecipes ON;
GO

INSERT INTO MealPlanRecipes (MealPlanRecipeID, MealPlanID, RecipeID, Servings) VALUES
(1, 1, 3, 2),
(2, 2, 1, 4),
(3, 3, 6, 2),
(4, 4, 4, 6),
(5, 4, 18, 8),
(6, 5, 12, 3),
(7, 6, 5, 2),
(8, 7, 20, 3);
GO

SET IDENTITY_INSERT MealPlanRecipes OFF;
GO

-- ============================================
-- Insert Followers
-- ============================================
SET IDENTITY_INSERT Followers ON;
GO

INSERT INTO Followers (FollowID, FollowerUserID, FollowingUserID, CreatedAt) VALUES
(1, 2, 1, GETDATE()),
(2, 3, 1, GETDATE()),
(3, 4, 1, GETDATE()),
(4, 5, 1, GETDATE()),
(5, 1, 2, GETDATE()),
(6, 3, 2, GETDATE()),
(7, 4, 2, GETDATE()),
(8, 1, 5, GETDATE()),
(9, 2, 5, GETDATE()),
(10, 3, 5, GETDATE()),
(11, 4, 5, GETDATE()),
(12, 6, 1, GETDATE()),
(13, 7, 1, GETDATE()),
(14, 8, 2, GETDATE()),
(15, 1, 8, GETDATE()),
(16, 2, 8, GETDATE()),
(17, 3, 4, GETDATE()),
(18, 5, 6, GETDATE()),
(19, 6, 7, GETDATE()),
(20, 7, 3, GETDATE());
GO

SET IDENTITY_INSERT Followers OFF;
GO

PRINT 'Database schema created successfully!';
PRINT 'Total tables: 15';
PRINT 'Total indexes: 28';
PRINT '========================================';
PRINT 'Sample data inserted successfully!';
PRINT 'Users: 8 (All passwords: 123456)';
PRINT 'Categories: 10';
PRINT 'Recipes: 20';
PRINT 'Recipe Ingredients: 105';
PRINT 'Recipe Steps: 72';
PRINT 'Recipe Likes: 33';
PRINT 'Saved Recipes: 14';
PRINT 'Comments: 20';
PRINT 'Comment Likes: 13';
PRINT 'Shopping Lists: 4';
PRINT 'Shopping List Items: 10';
PRINT 'Meal Plans: 7';
PRINT 'Meal Plan Recipes: 8';
PRINT 'Followers: 20';
PRINT '========================================';
GO