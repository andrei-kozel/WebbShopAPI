using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebbShopApi.Database;
using WebbShopApi.Models;

namespace WebbShopApi.Helpers
{
    public static class WebbShopAPI
    {
        private static MyContext context = new MyContext();

        /// <summary>
        /// Login the user
        /// </summary>
        /// <param name="username">used to specify username</param>
        /// <param name="password">used to specify password</param>
        /// <returns>user id if success; 0 if user doesn't exist</returns>
        public static int Login(string username, string password)
        {
            var user = context.Users.FirstOrDefault(u => u.Name == username && u.Password == password);
            try
            {
                user.LastLogin = DateTime.Now;
                user.SessionTimer = DateTime.Now;
                context.Users.Update(user);
                context.SaveChanges();
                return user.UserId;
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine("[Login] Can't find the user.");
            }
            return 0;
        }

        /// <summary>
        /// Logout the user
        /// </summary>
        /// <param name="userId">used to specify user id</param>
        public static void Logout(int userId)
        {
            var user = context.Users.FirstOrDefault(u => u.UserId == userId && u.SessionTimer > DateTime.Now.AddMinutes(-15));
            try
            {
                user.SessionTimer = DateTime.MinValue;
                context.Users.Update(user);
                context.SaveChanges();
            }
            catch
            {
                Console.WriteLine("[Logout] Can't find the user.");
            }

        }

        /// <summary>
        /// List all categories
        /// </summary>
        /// <returns>List<BookCategory></returns>
        public static List<BookCategory> GetCategories()
        {
            return context.BookCategories.OrderBy(c => c.Name).ToList();
        }

        /// <summary>
        /// List of categories matching keyword
        /// </summary>
        /// <param name="keyword">used to specifie keyword</param>
        /// <returns>List<BookCategory></returns>
        public static List<BookCategory> GetCategories(string keyword)
        {
            return context.BookCategories.Where(c => c.Name.Contains(keyword)).OrderBy(c => c.Name).ToList();
        }

        /// <summary>
        /// List of books in category
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns>List<Book></returns>
        public static List<Book> GetCategory(int categoryId)
        {
            return context.Books.Where(b => b.BookCategoryId == categoryId).OrderBy(b => b.Name).ToList();
        }

        /// <summary>
        /// List of books with amount>0
        /// </summary>
        /// <param name="categoryId">used to specify category id</param>
        /// <returns>List<Book></returns>
        public static List<Book> GetAvailableBooks(int categoryId)
        {
            return context.Books.Where(b => b.BookId == categoryId && b.Amount > 0).OrderBy(b => b.Name).ToList();
        }

        /// <summary>
        /// Get information about book
        /// </summary>
        /// <param name="bookId">used to specify book id</param>
        /// <returns>Book object</returns>
        public static Book GetBook(int bookId)
        {
            return context.Books.FirstOrDefault(b => b.BookId == bookId);
        }

        /// <summary>
        /// List of books matching keyword
        /// </summary>
        /// <param name="keyword">used to specifie keyword</param>
        /// <returns>List<Book></returns>
        public static List<Book> GetBooks(string keyword)
        {
            return context.Books.Where(b => b.Name.Contains(keyword)).OrderBy(b => b.Name).ToList();
        }

        /// <summary>
        /// List of books matching keyword in author
        /// </summary>
        /// <param name="keyword">used to specify keyword</param>
        /// <returns>List<Book></returns>
        public static List<Book> GetAuthors(string keyword)
        {
            return context.Books.Where(b => b.Author.Contains(keyword)).OrderBy(b => b.Name).ToList();
        }

        /// <summary>
        /// Buy a book from the store. 
        /// </summary>
        /// <param name="userId">user id</param>
        /// <param name="bookId">book id</param>
        /// <returns>true or fale</returns>
        public static bool BuyBook(int userId, int bookId)
        {
            var user = context.Users.FirstOrDefault(u => u.UserId == userId);
            if (user == null) { return false; };
            var book = context.Books.FirstOrDefault(b => b.BookId == bookId);
            if (book == null) { return false; }
            if (book.Amount <= 0) { return false; }

            context.SoldBooks.Add(
                new SoldBook
                {
                    Author = book.Author,
                    BookCategoryId = book.BookCategoryId,
                    Price = book.Price,
                    PurchaseDate = DateTime.Now,
                    Title = book.Name,
                    UserId = userId
                });
            book.Amount--;
            context.Books.Update(book);
            context.SaveChanges();

            return true;
        }

        /// <summary>
        /// Check if customer is online
        /// </summary>
        /// <param name="userId">user id</param>
        /// <returns>string.Empty if offline, "Pong" if online</returns>
        public static string Ping(int userId)
        {
            var user = context.Users.FirstOrDefault(u => u.UserId == userId);
            if (user.SessionTimer > DateTime.Now.AddMinutes(-15))
            {
                return String.Empty;
            }
            return "Pong";
        }

        /// <summary>
        /// Register a new customer
        /// </summary>
        /// <param name="name">Customer name</param>
        /// <param name="password">Password</param>
        /// <param name="passwordVerify">Repeat password</param>
        /// <returns>true if success</returns>
        public static bool Register(string name, string password, string passwordVerify)
        {
            if (password == passwordVerify)
            {
                context.Users.Add(new User { Name = name, Password = password });
                return true;
            }
            return false;
        }

        // ADMIN FUNCTIONS BELOW

        /// <summary>
        /// Add a new book
        /// Change amount if book exist
        /// </summary>
        /// <param name="adminId">user id</param>
        /// <param name="Id">book id</param>
        /// <param name="title">book title</param>
        /// <param name="author">author</param>
        /// <param name="price">price</param>
        /// <param name="amount">amount</param>
        /// <returns>true if success</returns>
        public static bool AddBook(int adminId, int Id, string title, string author, int price, int amount)
        {
            var book = context.Books.FirstOrDefault(b => b.BookId == Id && b.Name == title);
            if (IsAdmin(adminId))
            {
                if (book == null)
                {
                    context.Books.Add(new Book { Name = title, Author = author, Price = price, Amount = amount });
                    return true;
                }
                else if (book != null)
                {
                    book.Amount += amount;
                    context.Books.Update(book);
                    context.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Set amount of available books
        /// </summary>
        /// <param name="adminId">user id</param>
        /// <param name="bookId">book id</param>
        /// <param name="amount">amount</param>
        public static void SetAmount(int adminId, int bookId, int amount)
        {
            if (IsAdmin(adminId))
            {
                try
                {
                    var book = context.Books.FirstOrDefault(b => b.BookId == bookId);
                    book.Amount = amount;
                    context.Books.Update(book);
                    context.SaveChanges();
                }
                catch (NullReferenceException e)
                {
                    Console.WriteLine("[SetAmount] Can't find the book");
                }
                catch (Exception e)
                {
                    Console.WriteLine("[SetAmount] Something went wrong ...");
                    Console.WriteLine(e);
                }
            }
        }

        /// <summary>
        /// List all users by name
        /// </summary>
        /// <param name="adminId">user id</param>
        /// <returns>List<User></returns>
        public static List<User> ListUsers(int adminId)
        {
            if (IsAdmin(adminId))
            {
                return context.Users.OrderBy(u => u.Name).ToList();
            }
            return new List<User>();
        }

        /// <summary>
        /// Find users by keyword
        /// </summary>
        /// <param name="adminId">user(admin) id</param>
        /// <param name="keyword">kyword used to find users</param>
        /// <returns>List<User></returns>
        public static List<User> FindUser(int adminId, string keyword)
        {
            if (IsAdmin(adminId))
            {
                return context.Users.Where(u => u.Name.Contains(keyword)).OrderBy(u => u.Name).ToList();
            }
            return new List<User>();
        }

        /// <summary>
        /// Update the book
        /// </summary>
        /// <param name="adminId">user id</param>
        /// <param name="title">new book title</param>
        /// <param name="author">new author</param>
        /// <param name="price">new price</param>
        /// <returns>true if success</returns>
        public static bool UpdateBook(int adminId, int bookId, string title, string author, int price)
        {
            try
            {
                if (IsAdmin(adminId))
                {
                    var book = context.Books.FirstOrDefault(b => b.BookId == bookId);

                    book.Name = title;
                    book.Author = author;
                    book.Price = price;
                    context.Books.Update(book);
                    context.SaveChanges();
                    return true;
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine("[UpdateBook] Can't find the book");
            }
            catch (Exception e)
            {
                Console.WriteLine("[UpdateBook] Something went wrong ...");
                Console.WriteLine(e);
            }
            return false;
        }

        /// <summary>
        /// Delete the book
        /// </summary>
        /// <param name="adminId">user id</param>
        /// <param name="bookId">book id</param>
        /// <returns>true if success</returns>
        public static bool DeleteBook(int adminId, int bookId)
        {
            try
            {
                if (IsAdmin(adminId))
                {
                    var book = context.Books.FirstOrDefault(b => b.BookId == bookId);
                    context.Books.Remove(book);
                    context.SaveChanges();
                    return true;
                }
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("[DeleteBook] Can't find the book");
            }
            catch (Exception e)
            {
                Console.WriteLine("[DeleteBook] Something went wrong ...");
                Console.WriteLine(e);
            }
            return false;
        }

        /// <summary>
        /// Add a new category
        /// </summary>
        /// <param name="adminId">user id</param>
        /// <param name="name">category name</param>
        /// <returns>true or false</returns>
        public static bool AddCategory(int adminId, string name)
        {
            try
            {
                var category = context.BookCategories.FirstOrDefault(c => c.Name == name);
                if (IsAdmin(adminId) && category == null)
                {
                    context.BookCategories.Add(new BookCategory { Name = name });
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("[AddCategory] Something went wrong ...");
                Console.WriteLine(e);
            }
            return false;
        }

        /// <summary>
        /// Add book to category
        /// </summary>
        /// <param name="adminId">user id</param>
        /// <param name="bookId">book id</param>
        /// <param name="categoryId">category id</param>
        /// <returns>true or false</returns>
        public static bool AddBookToCategory(int adminId, int bookId, int categoryId)
        {
            try
            {
                var book = context.Books.FirstOrDefault(b => b.BookId == bookId);
                var category = context.BookCategories.FirstOrDefault(c => c.BookCategoryId == categoryId);
                if (IsAdmin(adminId))
                {
                    // make sure that the category exists
                    book.BookCategoryId = category.BookCategoryId;
                    context.Books.Update(book);
                    context.SaveChanges();
                    return true;
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine("[AddBookToCategory] Can't find the book or category");
            }
            catch (Exception e)
            {
                Console.WriteLine("[AddBookToCategory] Something went wrong ...");
                Console.WriteLine(e);
            }
            return false;
        }

        /// <summary>
        /// Update category name
        /// </summary>
        /// <param name="adminId">user id</param>
        /// <param name="categoryId">category id</param>
        /// <param name="name">category name</param>
        /// <returns>true or false</returns>
        public static bool UpdateCategory(int adminId, int categoryId, string name)
        {
            try
            {
                var category = context.BookCategories.FirstOrDefault(c => c.BookCategoryId == categoryId);
                if (IsAdmin(adminId))
                {
                    category.Name = name;
                    context.BookCategories.Update(category);
                    context.SaveChanges();
                    return true;
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine("[UpdateCategory] Can't find user or category");
            }
            catch (Exception e)
            {
                Console.WriteLine("[UpdateCategory] Something went wrong ...");
                Console.WriteLine(e);
            }
            return false;
        }

        /// <summary>
        /// Delete category
        /// </summary>
        /// <param name="adminId">admin id</param>
        /// <param name="categoryId">category id</param>
        /// <returns>true or false</returns>
        public static bool DeleteCategory(int adminId, int categoryId)
        {
            try
            {
                var category = context.BookCategories.FirstOrDefault(c => c.BookCategoryId == categoryId);
                if (IsAdmin(adminId))
                {
                    context.BookCategories.Remove(category);
                    context.SaveChanges();
                    return true;
                }
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("[DeleteCategory] Can't find your category");
            }
            catch (Exception e)
            {
                Console.WriteLine("[DeleteCategory] Something went wrong ...");
                Console.WriteLine(e);
            }
            return false;
        }

        /// <summary>
        /// Add a new user
        /// </summary>
        /// <param name="adminId">admin id</param>
        /// <param name="name">new user name</param>
        /// <param name="password">new user password</param>
        /// <returns>true or false</returns>
        public static bool AddUser(int adminId, string name, string password)
        {
            try
            {
                if (IsAdmin(adminId))
                {
                    context.Users.Add(new User { Name = name, Password = password });
                    context.SaveChanges();
                }
            }
            catch { Console.WriteLine("[AddUser] Something went wrong ..."); }
            return false;
        }

        /// <summary>
        /// Checks if a user with a given id is an administrator
        /// </summary>
        /// <param name="adminId">used to specify user id</param>
        /// <returns>true or false</returns>
        private static bool IsAdmin(int adminId)
        {
            try
            {
                if (context.Users.FirstOrDefault(u => u.UserId == adminId).IsAdmin)
                {
                    return true;
                }
                else
                {
                    Console.WriteLine("Sorry. You don't have permissions!");
                }
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("[IsAdmin] Can't find the user ...");
            }
            return false;
        }

        // TODO: UpdateSession method
        private static void UpdateSession()
        {

        }
    }
}
