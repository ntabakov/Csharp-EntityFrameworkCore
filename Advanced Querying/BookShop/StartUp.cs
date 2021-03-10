using System.IO;
using System.Linq;
using System.Text;
using BookShop.Models.Enums;

namespace BookShop
{
    using Data;
    using Initializer;
    using System;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
           // DbInitializer.ResetDatabase(db);
           Console.WriteLine(GetBooksByPrice(db));
        }

        public static string GetBooksByPrice(BookShopContext context)
        {
            var books = context.Books.Where(x => x.Price > 40).OrderByDescending(x => x.Price);
            var sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title} - ${book.Price:f2}");
            }


            return sb.ToString().TrimEnd();
        }

        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            AgeRestriction ar = CheckAge(command);
            var books = context.Books.Where(x => x.AgeRestriction == ar).OrderBy(x=>x.Title);
            var sb = new StringBuilder();
            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title}");
            }

            return sb.ToString().TrimEnd();
        }

        private static AgeRestriction CheckAge(string command)
        {
            
            if (command.ToLower() == "minor")
            {
                return AgeRestriction.Minor;
            }

            if (command.ToLower() == "teen")
            {
                return AgeRestriction.Teen;
            } 

            if (command.ToLower() == "adult")
            {
                return AgeRestriction.Adult;
            }

            throw new InvalidDataException();
        }

        public static string GetGoldenBooks(BookShopContext context)
        {
            var books = context.Books.Where(x => x.Copies < 5000 && x.EditionType==EditionType.Gold).OrderBy(x => x.BookId);
            var sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title}");
            }


            return sb.ToString().TrimEnd();
        }
    }
}
