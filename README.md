Website Documentation for Bookstore E-Commerce
1. Introduction
•	Website Name: Bookle
•	Purpose: An e-commerce platform for selling books online.
•	Primary Function: Selling books to users and publishers with management features for user accounts, admin, and publisher roles.
________________________________________
2. Front-End
Main Pages:
•	Home Page: Displays featured or best-selling books with links to various categories.
•	Login Page: For users, publishers, and admins.
•	Category Page: Displays books sorted by categories.
•	Book’s Details Page: Contains book details such as title, author, price, description, and reviews.
•	Cart Page: Shows books added to the cart with options to modify or complete the purchase.
•	User Account Page: For users and publishers to manage their accounts.
Technologies Used:
•	Framework: ASP.NET Core MVC
•	Libraries: Bootstrap.
•	Interactions: AJAX for updating content without page reload.
Main Features:
•	Registration: User and publisher account creation.
•	Login: Supports multiple login types (publisher, user, admin).
•	Cart Management: Add/remove books from the cart.
•	Search: Search books by title or category.
________________________________________
3. Back-End
Technologies Used:
•	Framework: ASP.NET Core MVC
•	Database: SQL Server.
•	Authentication: Identity Framework for managing user, publisher, and admin logins.
•	API Endpoints: RESTful API for front-end and back-end communication.
Main Features:
•	User Management: Create, update, and delete user accounts.
•	Book Management: Add, update, and delete books.
•	Category Management: Add, update, and delete book categories.
•	Language Management: Assign languages to books (e.g., Arabic, English).
•	Order Management: Admins can view and manage user orders.
•	Security Settings: Password policies, user roles, and access permissions.
Database Structure:
•	Tables: 
o	Users: Stores information for users, publishers, and admins.
o	Books: Contains book data like title, author, category, price, and rating.
o	Categories: Stores book categories.
o	Orders: Stores order details for users.
o	Carts: Store users carts
________________________________________
4. Authentication
Features:
•	Login: Allows users, publishers, and admins to log in using their credentials.
•	Role-Based Permissions: Admins can assign roles and permissions (e.g., publishers can add books, users can only purchase).
Technologies:
•	Identity Framework: Used for managing user accounts and login functionality.
________________________________________
5. Category and Language Management
Categories:
•	Function: Categorizes books into different sections (e.g., Fiction, History, Educational).
•	Process: Admins can add or modify categories.
Languages:
•	Function: Supports multiple languages for books.
________________________________________
6. Cart and Order Management
Cart:
•	Function: Users can add books to the cart, modify quantities, and remove items.
Orders:
•	Function: Once a purchase is completed, an order is generated with the details of the books bought and each book publisher.
•	Process: Sends an order confirmation and shipping details to the user.
________________________________________
7. Security and Protection
Implemented Measures:
•	Password Encryption: Using hashing techniques like SHA or PBKDF2.
•	Two-Factor Authentication: Can be enabled for users and admins for enhanced security.
________________________________________
Models Used
Book:
•	Fields: Id, Title, Author, Price, Description, StockQuantity, BookLanguage, BookImage, ImageFile, CategoryId, PublisherId, Reviews.
Category:
•	Fields: Id, Name, Description, Books.
Cart:
•	Fields: Id, UserId, TotalAmount, CartItems.
CartItem:
•	Fields: Id, CartId, BookId, Quantity, Price.
Order:
•	Fields: Id, UserId, TotalAmount, OrderDate, OrderItems.
OrderItem:
•	Fields: Id, OrderId, BookId, Quantity, Price.
ContactUs:
•	Fields: Id, UserName, Email, Message, CreatedAt.
________________________________________

