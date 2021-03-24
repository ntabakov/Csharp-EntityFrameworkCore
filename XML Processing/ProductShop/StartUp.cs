using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using ProductShop.Data;
using ProductShop.Dtos.Import;
using ProductShop.Models;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            var db = new ProductShopContext();
            //db.Database.EnsureDeleted();
            //db.Database.EnsureCreated();
            var xml = File.ReadAllText("../../../Datasets/users.xml");
            Console.WriteLine(ImportUsers(db,xml));
        }

        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(UsersDTO[]), 
                new XmlRootAttribute("Users"));

            var xmlRead = new StringReader(inputXml);

            var usersDTO = xmlSerializer.Deserialize(xmlRead) as UsersDTO[];

            var users = usersDTO.Select(x => new User
            {
                FirstName = x.FirstName,
                LastName = x.LastName,
                Age = x.Age,
            }).ToList();

            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count}";
        }
    }
}