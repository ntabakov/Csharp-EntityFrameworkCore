using BookShop.Data.Models.Enums;
using BookShop.DataProcessor.ExportDto;
using BookShop.XmlHelper;
using Newtonsoft.Json;

namespace BookShop.DataProcessor
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using Data;
   
    

    public class Serializer
    {
        public static string ExportMostCraziestAuthors(BookShopContext context)
        {
            var authors = context.Authors
                
                .Select(x => new
                {
                    AuthorName = x.FirstName + ' ' + x.LastName,
                    Books = x.AuthorsBooks
                        .OrderByDescending(b => b.Book.Price)
                        .Select(b => new
                    {
                        BookName = b.Book.Name,
                        BookPrice = b.Book.Price.ToString("f2"),
                    })
                        .ToList()
                })
                .ToList()
                .OrderByDescending(x => x.Books.Count)
                .ThenBy(x => x.AuthorName)
                .ToList();

            var result = JsonConvert.SerializeObject(authors, Newtonsoft.Json.Formatting.Indented);

            return result;
        }

        public static string ExportOldestBooks(BookShopContext context, DateTime date)
        {
            var books = context.Books.Where(x => x.PublishedOn < date && x.Genre == Genre.Science)
                .ToList()
                .OrderByDescending(x=>x.Pages)
                .ThenByDescending(x=>x.PublishedOn)
                .Select(x => new BooksXmlOutputDto
                {
                    Pages = x.Pages.ToString(),
                    Name = x.Name,
                    Date = x.PublishedOn.ToString("d",CultureInfo.InvariantCulture),
                }).ToList()
                
                .Take(10)
                .ToList();

            var result = XmlConverter.Serialize(books, "Books");

            return result;
        }
    }
}