using System;
using System.Collections.Generic;
using WebbShopApi.Database;
using WebbShopApi.Helpers;

namespace WebbShopApi
{
    class Program
    {
        static void Main(string[] args)
        {
            Seeder.Seed();
            //Example1();
            WebbShopAPI.UpdateBook(2,10,"ad", "aaa", 10);
        }

        private static void Example1()
        {
            LogInAsTestUser();
            GetCategories();
            GetHorrorsBooks();
            GetBook("DrSleep");
            GetBook("Sleep");
            GetAmount(1);
            BuyBook(1);
            GetAmount(1);
        }

        private static void BuyBook(int bookId)
        {
            Console.WriteLine("TEST: Buy book");
            Console.WriteLine();
            int userId = 1;
            if (WebbShopAPI.BuyBook(userId, bookId))
            {
                Console.WriteLine("Success");
            }
            else
            {
                Console.WriteLine("Error");
            }
            Continue();
        }

        private static void GetAmount(int id)
        {
            Console.WriteLine($"TEST: Get amount by id {id}");
            Console.WriteLine();
            var book = WebbShopAPI.GetBook(id);
            Console.WriteLine($"{book.Name} - {book.Amount}st");
            Continue();
        }

        private static void GetBook(string v)
        {
            Console.WriteLine($"TEST: List books contains {v}");
            Console.WriteLine();
            var books = WebbShopAPI.GetBooks(v);
            foreach (var b in books)
            {
                Console.WriteLine(b.Name);
            }
            Continue();
        }

        private static void GetHorrorsBooks()
        {
            Console.WriteLine("TEST: List horror books");
            Console.WriteLine();
            var books = WebbShopAPI.GetCategory(1);
            foreach (var b in books)
            {
                Console.WriteLine(b.Name);
            }
            Continue();
        }

        private static void GetCategories()
        {
            Console.WriteLine("TEST: List all categories");
            Console.WriteLine();
            var categories = WebbShopAPI.GetCategories();
            foreach (var c in categories)
            {
                Console.WriteLine(c.Name);
            }
            Continue();
        }

        private static void Continue()
        {
            Console.WriteLine();
            Console.WriteLine("Press ENTER to continue ...");
            Console.ReadLine();
            Console.Clear();
        }

        private static void LogInAsTestUser()
        {
            Console.WriteLine("TEST: Login as a test user Codic2021");
            Console.WriteLine();
            if (WebbShopAPI.Login("Codic2021", "Codic2021") != 0)
            {
                Console.WriteLine("You are logged in as test user Codic2021;");
            }
            else
            {
                Console.WriteLine("Error: you can't loggin");
            }
            Continue();
        }
    }
}
