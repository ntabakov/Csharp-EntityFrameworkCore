using System.Globalization;
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
            Console.WriteLine(GetMostRecentBooks(db));
        }

        public static int RemoveBooks(BookShopContext context)
        {
            int removedBooks = 0;
            var books = context.Books.Where(x => x.Copies < 4200);
            foreach (var book in books)
            {
                context.Books.Remove(book);
                removedBooks++;
            }

            context.SaveChanges();
            return removedBooks;
        }

        public static void IncreasePrices(BookShopContext context)
        {
            var books = context.Books.Where(x => x.ReleaseDate.Value.Year < 2010);
            foreach (var book in books)
            {
                book.Price += 5;

            }

            context.SaveChanges();
        }
        public static string GetMostRecentBooks(BookShopContext context)
        {
            var categories = context.Categories.Select(x => new
            {
                x.Name,
                Books = x.CategoryBooks.Select(c => new
                {
                    c.Book.Title,
                    c.Book.ReleaseDate
                }).OrderByDescending(r => r.ReleaseDate).Take(3)
            }).ToList().OrderBy(x=>x.Name);


            var sb = new StringBuilder();

            foreach (var c in categories)
            {
                sb.AppendLine($"--{c.Name}");
                foreach (var book in c.Books)
                {
                    sb.AppendLine($"{book.Title} ({book.ReleaseDate.Value.Year})");
                }
            }


            return sb.ToString().TrimEnd();
        }
        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            var categories = context.Categories.Select(x => new
            {
                x.Name,
                Profit = x.CategoryBooks
                    .Sum(cb => cb.Book.Copies * cb.Book.Price)

            })
                .ToList()
                .OrderByDescending(x=>x.Profit)
                .ThenBy(x=>x.Name);


            var sb = new StringBuilder();

            foreach (var book in categories)
            {
                sb.AppendLine($"{book.Name} ${book.Profit:f2}");
            }


            return sb.ToString().TrimEnd();
        }

        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var authors = context.Authors.Select(x => new
            {
                Copies = x.Books.Select(x => x.Copies).Sum(),
                FullName = x.FirstName + ' ' + x.LastName,
            })
                .ToList()
                .OrderByDescending(x => x.Copies);
            var sb = new StringBuilder();

            foreach (var a in authors)
            {
                sb.AppendLine($"{a.FullName} - {a.Copies}");
            }


            return sb.ToString().TrimEnd();
        }






        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            var books = context.Books.Where(x => x.Title.Length > lengthCheck);


            return books.Count();
        }

        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            var books = context.Books.Where(x => x.Author.LastName.ToLower().StartsWith(input.ToLower())).Select(x => new
            {
                x.BookId,
                x.Title,
                FullName = x.Author.FirstName + ' ' + x.Author.LastName
            })
                .ToList()
                .OrderBy(x => x.BookId);


            var sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title} ({book.FullName})");
            }


            return sb.ToString().TrimEnd();
        }



        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            var books = context.Books.Where(x => x.Title.ToLower().Contains(input.ToLower())).OrderBy(x => x.Title);


            var sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title}");
            }


            return sb.ToString().TrimEnd();
        }
        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            var authors = context.Authors.Where(x => x.FirstName.EndsWith(input)).Select(x => new
            {
                FullName = x.FirstName + ' ' + x.LastName,
            }).ToList()
                .OrderBy(x => x.FullName);


            var sb = new StringBuilder();

            foreach (var author in authors)
            {
                sb.AppendLine($"{author.FullName}");
            }


            return sb.ToString().TrimEnd();
        }

        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            DateTime givenDate = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            var books = context.Books.Where(x => x.ReleaseDate < givenDate).OrderByDescending(x => x.ReleaseDate);


            var sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title} - {book.EditionType} - ${book.Price:f2}");
            }


            return sb.ToString().TrimEnd();
        }

        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            var inputTokens = input.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(x => x.ToLower());

            var books = context.BooksCategories.Select(x => new
            {
                Title = x.Book.Title,
                Name = x.Category.Name
            })
                .Where(x => inputTokens.Contains(x.Name.ToLower()))
                .OrderBy(x => x.Title);
            var sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title}");
            }


            return sb.ToString().TrimEnd();
        }


        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            var books = context.Books.Where(x => x.ReleaseDate.Value.Year != year).OrderBy(x => x.BookId);
            var sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title}");
            }


            return sb.ToString().TrimEnd();
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
            var books = context.Books.Where(x => x.AgeRestriction == ar).OrderBy(x => x.Title);
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
            var books = context.Books.Where(x => x.Copies < 5000 && x.EditionType == EditionType.Gold).OrderBy(x => x.BookId);
            var sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title}");
            }


            return sb.ToString().TrimEnd();
        }
    }
}
