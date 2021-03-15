using System;
using System.Collections.Generic;
using System.Text;
using WebbShopApi.Models;

namespace WebbShopApi.Helpers
{
    public static class WebbShopAPI
    {
        public static int Login(string username, string password)
        {
            return 0;
        }

        public static void Logout(int userId)
        {
        }

        public static List<BookCategory> GetCategories()
        {
            return new List<BookCategory>();
        }

        public static List<BookCategory> GetCategories(string keyword)
        {
            return new List<BookCategory>();
        }

        public static string GetCategory(int categoryId)
        {
            return String.Empty;
        }

        public static string GetBook(int bookId)
        {
            return String.Empty;
        }

        public static List<Book> GetBooks(string keyword)
        {
            return new List<Book>();
        }

        public static List<Book> GetAuthors(string keyword)
        {
            return new List<Book>();
        }

        public static bool BuyBook(int userId, int bookId)
        {
            return false;
        }

        public static string Ping(int userId) {
            return "Pong";
        }

        public static bool Register(string name, string password, string passwordVerify)
        {
            return false;
        }

        // ADMIN FUNCTIONS
        public static bool AddBook(int adminId, int Id, string title, string author, int price, int amount )
        {
            return false;
        }

        public static void SetAmount(int adminId, int bookId)
        {

        }

        public static List<User> ListUsers(int adminId)
        {
            return new List<User>();
        }

        public static List<User> FindUser(int adminId, string keyword)
        {
            return new List<User>();
        }

        public static bool UpdateBook(int adminId, int Id, string title, string author, int price)
        {
            return false;
        }

        public static bool DeleteBook(int adminId, int bookId)
        {
            return false;
        }

        public static bool AddCategory(int adminId, string name)
        {
            return false;
        }

        public static bool AddBookToCategory(int adminId, int bookId, int categoryId)
        {
            return false;
        }

        public static bool UpdateCategory(int adminId, int categoryId, string name)
        {
            return false;
        }

        public static bool DeleteCategory(int adminId, int categoryId)
        {
            return false;
        }

        public static bool AddUser(int adminId, string name, string password)
        {
            return false;
        }
    }
}
