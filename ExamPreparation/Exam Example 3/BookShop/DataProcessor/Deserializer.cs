using BookShop.Data.Models;
using BookShop.Data.Models.Enums;
using BookShop.DataProcessor.ImportDto;
using BookShop.XmlHelper;

namespace BookShop.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedBook
            = "Successfully imported book {0} for {1:F2}.";

        private const string SuccessfullyImportedAuthor
            = "Successfully imported author - {0} with {1} books.";

        public static string ImportBooks(BookShopContext context, string xmlString)
        {
            var books = XmlConverter.Deserializer<BooksXmlImportDTO>(xmlString, "Books");

            var bookList = new List<Book>();

            var sb = new StringBuilder();

            foreach (var book in books)
            {
                if (!IsValid(book))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var bookToAdd = new Book
                {
                    Name = book.Name,
                    Genre = (Genre)Enum.Parse(typeof(Genre), book.Genre),
                    Price = book.Price,
                    Pages = book.Pages,
                    PublishedOn = DateTime.ParseExact(book.PublishedOn, "MM/dd/yyyy", CultureInfo.InvariantCulture)

                };
                sb.AppendLine($"Successfully imported book {book.Name} for {book.Price:f2}.");

                bookList.Add(bookToAdd);
            }

            context.AddRange(bookList);
            context.SaveChanges();


            return sb.ToString().TrimEnd();
        }

        public static string ImportAuthors(BookShopContext context, string jsonString)
        {
            var sb = new StringBuilder();
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,

            };

            var json = JsonConvert.DeserializeObject<AuthorsJsonImportDTO[]>(jsonString, settings);

            var authorsList = new List<Author>();

            foreach (var author in json)
            {
                if (!IsValid(author))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (authorsList.Any(x => x.Email == author.Email))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var authorToAdd = new Author()
                {
                    FirstName = author.FirstName,
                    LastName = author.LastName,
                    Email = author.Email,
                    Phone = author.Phone,
                };


                foreach (var bookDto in author.Books)
                {
                    var book = context.Books.FirstOrDefault(x => x.Id == bookDto.Id);

                    if (book == null)
                    {
                        continue;
                    }

                    authorToAdd.AuthorsBooks.Add(new AuthorBook()
                    {
                        Book = book,
                        Author = authorToAdd
                    });

                }

                if (authorToAdd.AuthorsBooks.Count == 0)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                sb.AppendLine(
                    $"Successfully imported author - {authorToAdd.FirstName+' ' + authorToAdd.LastName } with {authorToAdd.AuthorsBooks.Count} books.");
                authorsList.Add(authorToAdd);
            }

            context.Authors.AddRange(authorsList);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}