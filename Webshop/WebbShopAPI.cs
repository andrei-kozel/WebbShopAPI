using System;
using System.Collections.Generic;
using System.Linq;
using WebbShopApi.Database;
using WebbShopApi.Models;

namespace WebbShopApi.Helpers
{
    /// <summary>
    /// Defines the <see cref="WebbShopAPI" />.
    /// </summary>
    public static class WebbShopAPI
    {
        /// <summary>
        /// Defines the context[<see cref="MyContext"/>]
        /// </summary>
        private static MyContext context = new MyContext();

        /// <summary>
        /// Defines the temporary <see cref="User"/>
        /// </summary>
        private static User tempUser;

        /// <summary>
        /// Login the <see cref="User"/>
        /// </summary>
        /// <param name="username">used to specify username <see cref="User.Name"/></param>
        /// <param name="password">used to specify password <see cref="User.Password"/></param>
        /// <returns>user id if success; 0 if user doesn't exist [<see cref="int"/>]</returns>
        public static int Login(string username, string password)
        {
            var user = context.Users.FirstOrDefault(u => u.Name == username && u.Password == password);
            tempUser = user;
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
        /// Logout the <see cref="User"/>
        /// </summary>
        /// <param name="userId">used to specify user id <see cref="User.UserId"/></param>
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
        /// List all <see cref="BookCategory"/>
        /// </summary>
        /// <returns><see cref="List{BookCategory}"/></returns>
        public static List<BookCategory> GetCategories()
        {
            UpdateSession();
            return context.BookCategories.OrderBy(c => c.Name).ToList();
        }

        /// <summary>
        /// List of <see cref="BookCategory"/>s matching keyword
        /// </summary>
        /// <param name="keyword">used to specifie keyword <see cref="string"/></param>
        /// <returns><see cref="List{BookCategory}"/></returns>
        public static List<BookCategory> GetCategories(string keyword)
        {
            UpdateSession();
            return context.BookCategories.Where(c => c.Name.Contains(keyword)).OrderBy(c => c.Name).ToList();
        }

        /// <summary>
        /// List of <see cref="Book"/>s in <see cref="BookCategory"/>
        /// </summary>
        /// <param name="categoryId">Category Id <see cref="int"/></param>
        /// <returns><see cref="List{Book}"/></returns>
        public static List<Book> GetCategory(int categoryId)
        {
            UpdateSession();
            return context.Books.Where(b => b.BookCategoryId == categoryId).OrderBy(b => b.Name).ToList();
        }

        /// <summary>
        /// List of <see cref="Book"/>s with amount>0
        /// </summary>
        /// <param name="categoryId">used to specify category id <see cref="int"/></param>
        /// <returns><see cref="List{Book}"/></returns>
        public static List<Book> GetAvailableBooks(int categoryId)
        {
            UpdateSession();
            return context.Books.Where(b => b.BookId == categoryId && b.Amount > 0).OrderBy(b => b.Name).ToList();
        }

        /// <summary>
        /// Get information about  <see cref="Book"/>
        /// </summary>
        /// <param name="bookId">used to specify book id <see cref="int"/></param>
        /// <returns>Book object <see cref="Book"/></returns>
        public static Book GetBook(int bookId)
        {
            UpdateSession();
            return context.Books.FirstOrDefault(b => b.BookId == bookId);
        }

        /// <summary>
        /// List of <see cref="Book"/>s matching keyword
        /// </summary>
        /// <param name="keyword">used to specifie keyword <see cref="string"/></param>
        /// <returns><see cref="List{Book}"/></returns>
        public static List<Book> GetBooks(string keyword)
        {
            UpdateSession();
            return context.Books.Where(b => b.Name.Contains(keyword)).OrderBy(b => b.Name).ToList();
        }

        /// <summary>
        /// List of <see cref="Book"/>s matching keyword in author
        /// </summary>
        /// <param name="keyword">used to specify keyword <see cref="string"/></param>
        /// <returns><see cref="List{Book}"/></returns>
        public static List<Book> GetAuthors(string keyword)
        {
            UpdateSession();
            return context.Books.Where(b => b.Author.Contains(keyword)).OrderBy(b => b.Name).ToList();
        }

        /// <summary>
        /// Buy a <see cref="Book"/> from the store. 
        /// </summary>
        /// <param name="userId">user id <see cref="int"/></param>
        /// <param name="bookId">book id <see cref="int"/></param>
        /// <returns><see cref="bool"/></returns>
        public static bool BuyBook(int userId, int bookId)
        {
            UpdateSession();
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
        /// Check if <see cref="User"/> is online
        /// </summary>
        /// <param name="userId">user id <see cref="int"/></param>
        /// <returns><see cref="string"/></returns>
        public static string Ping(int userId)
        {
            UpdateSession();
            var user = context.Users.FirstOrDefault(u => u.UserId == userId);
            if (user.SessionTimer > DateTime.Now.AddMinutes(-15))
            {
                return String.Empty;
            }
            return "Pong";
        }

        /// <summary>
        /// Register a new <see cref="User"/>
        /// </summary>
        /// <param name="name">Customer name <see cref="string"/></param>
        /// <param name="password">Password <see cref="string"/></param>
        /// <param name="passwordVerify">Repeat password  <see cref="string"/></param>
        /// <returns> <see cref="bool"/></returns>
        public static bool Register(string name, string password, string passwordVerify)
        {
            UpdateSession();
            if (password == passwordVerify)
            {
                context.Users.Add(new User { Name = name, Password = password });
                return true;
            }
            return false;
        }

        // ADMIN FUNCTIONS BELOW

        /// <summary>
        /// Add a new <see cref="Book"/>.
        /// Change the amount if the <see cref="Book"/> exist.
        /// </summary>
        /// <param name="adminId">user id  <see cref="int"/></param>
        /// <param name="Id">book id  <see cref="int"/></param>
        /// <param name="title">book title  <see cref="string"/></param>
        /// <param name="author">author  <see cref="string"/></param>
        /// <param name="price">price <see cref="int"/></param>
        /// <param name="amount">amount <see cref="int"/></param>
        /// <returns> <see cref="bool"/></returns>
        public static bool AddBook(int adminId, int Id, string title, string author, int price, int amount)
        {
            UpdateSession();
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
        /// Set amount of available <see cref="Book"/>s
        /// </summary>
        /// <param name="adminId">user id  <see cref="int"/></param>
        /// <param name="bookId">book id  <see cref="int"/></param>
        /// <param name="amount">amount  <see cref="int"/></param>
        public static void SetAmount(int adminId, int bookId, int amount)
        {
            UpdateSession();
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
        /// List all <see cref="User"/>s by name
        /// </summary>
        /// <param name="adminId">user id  <see cref="int"/></param>
        /// <returns> <see cref="List{User}"/></returns>
        public static List<User> ListUsers(int adminId)
        {
            UpdateSession();
            if (IsAdmin(adminId))
            {
                return context.Users.OrderBy(u => u.Name).ToList();
            }
            return new List<User>();
        }

        /// <summary>
        /// Find <see cref="User"/> by keyword
        /// </summary>
        /// <param name="adminId">user(admin) id <see cref="int"/></param>
        /// <param name="keyword">kyword used to find users <see cref="string"/></param>
        /// <returns><see cref="List{User}"/></returns>
        public static List<User> FindUser(int adminId, string keyword)
        {
            UpdateSession();
            if (IsAdmin(adminId))
            {
                return context.Users.Where(u => u.Name.Contains(keyword)).OrderBy(u => u.Name).ToList();
            }
            return new List<User>();
        }

        /// <summary>
        /// Update the <see cref="Book"/>
        /// </summary>
        /// <param name="adminId">user id <see cref="int"/></param>
        /// <param name="title">new book title <see cref="string"/></param>
        /// <param name="author">new author <see cref="string"/></param>
        /// <param name="price">new price <see cref="int"/></param>
        /// <returns><see cref="bool"/></returns>
        public static bool UpdateBook(int adminId, int bookId, string title, string author, int price)
        {
            UpdateSession();
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
        /// Delete the <see cref="Book"/>
        /// </summary>
        /// <param name="adminId">user id <see cref="int"/></param>
        /// <param name="bookId">book id <see cref="int"/></param>
        /// <returns><see cref="bool"/></returns>
        public static bool DeleteBook(int adminId, int bookId)
        {
            UpdateSession();
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
        /// Add a new <see cref="BookCategory"/>
        /// </summary>
        /// <param name="adminId">user id <see cref="int"/></param>
        /// <param name="name">category name <see cref="string"/></param>
        /// <returns><see cref="bool"/></returns>
        public static bool AddCategory(int adminId, string name)
        {
            UpdateSession();
            try
            {
                var category = context.BookCategories.FirstOrDefault(c => c.Name == name);
                if (IsAdmin(adminId) && category == null)
                {
                    context.BookCategories.Add(new BookCategory { Name = name });
                    context.SaveChanges();
                    return true;
                }
                else
                {
                    Console.WriteLine("Category already exist");
                    return false;
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
        /// Add <see cref="Book"/> to <see cref="BookCategory"/>
        /// </summary>
        /// <param name="adminId">user id <see cref="int"/></param>
        /// <param name="bookId">book id <see cref="int"/></param>
        /// <param name="categoryId">category id <see cref="int"/></param>
        /// <returns><see cref="bool"/></returns>
        public static bool AddBookToCategory(int adminId, int bookId, int categoryId)
        {
            UpdateSession();
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
        /// Update <see cref="BookCategory"/> name
        /// </summary>
        /// <param name="adminId">user id <see cref="int"/></param>
        /// <param name="categoryId">category id <see cref="int"/></param>
        /// <param name="name">category name <see cref="string"/></param>
        /// <returns><see cref="bool"/></returns>
        public static bool UpdateCategory(int adminId, int categoryId, string name)
        {
            UpdateSession();
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
        /// Delete <see cref="BookCategory"/>
        /// </summary>
        /// <param name="adminId">admin id <see cref="int"/></param>
        /// <param name="categoryId">category id <see cref="int"/></param>
        /// <returns><see cref="bool"/></returns>
        public static bool DeleteCategory(int adminId, int categoryId)
        {
            UpdateSession();
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
        /// Add a new <see cref="User"/>
        /// </summary>
        /// <param name="adminId">admin id <see cref="int"/></param>
        /// <param name="name">new user name <see cref="string"/></param>
        /// <param name="password">new user password <see cref="string"/></param>
        /// <returns><see cref="bool"/></returns>
        public static bool AddUser(int adminId, string name, string password)
        {
            UpdateSession();
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
        /// Checks if a <see cref="User"/> with a given id is an administrator
        /// </summary>
        /// <param name="adminId">used to specify user id <see cref="int"/></param>
        /// <returns><see cref="bool"/></returns>
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

        /// <summary>
        /// Updates user session.
        /// Logout if the session more than 15 min old.
        /// </summary>
        private static void UpdateSession()
        {
            //Console.WriteLine($"[UpdateSession] Before {tempUser.Name} : {tempUser.SessionTimer}");
            if (tempUser.SessionTimer > DateTime.Now.AddMinutes(-15))
            {
                Logout(tempUser.UserId);
            }
            else
            {
                tempUser.SessionTimer = DateTime.Now;
                context.Users.Update(tempUser);
                context.SaveChanges();
            }
            //Console.WriteLine($"[UpdateSession] After {tempUser.Name} : {context.Users.FirstOrDefault(u => u.UserId == tempUser.UserId).SessionTimer}");
        }
    }
}
