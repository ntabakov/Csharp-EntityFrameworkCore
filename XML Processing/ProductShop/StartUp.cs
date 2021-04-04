using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using ProductShop.Data;
using ProductShop.Dtos.Export;
using ProductShop.Dtos.Import;
using ProductShop.Models;
using ProductShop.XMLHelper;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            var db = new ProductShopContext();
           //// db.Database.EnsureDeleted();
           // db.Database.EnsureCreated();
            var xmlUsers = File.ReadAllText("../../../Datasets/users.xml");
            var xmlProducts = File.ReadAllText("../../../Datasets/products.xml");
            var xmlCategories = File.ReadAllText("../../../Datasets/categories.xml");
            var xmlCategoriesProducts = File.ReadAllText("../../../Datasets/categories-products.xml");

            // Console.WriteLine(ImportUsers(db, xmlUsers));
            //Console.WriteLine(ImportProducts(db, xmlProducts));
            //Console.WriteLine(ImportCategories(db, xmlCategories));
            //Console.WriteLine(ImportCategoryProducts(db, xmlCategoriesProducts));

           // Console.WriteLine(GetProductsInRange(db));
           // Console.WriteLine(GetSoldProducts(db));
            //Console.WriteLine(GetCategoriesByProductsCount(db));
            Console.WriteLine(GetUsersWithProducts(db));


        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var user = new UsersRootDto()
            {
                Count = context.Users.Count(x=>x.ProductsSold.Any(a=>a.Buyer!=null)),
                Users = context.Users.ToArray()
                    .Where(u => u.ProductsSold.Any(p => p.Buyer != null))
                    .OrderByDescending(u => u.ProductsSold.Count)
                    .Take(10)
                    .Select(u => new UserDto()
                    {
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Age = u.Age.Value,
                        SoldProducts = new SoldProductsDto
                        {
                            Count = u.ProductsSold.Count(ps => ps.Buyer != null),
                            Products = u.ProductsSold
                                .ToArray()
                                .Where(ps => ps.Buyer != null)
                                .Select(ps => new ProductsDto()
                                {
                                    Name = ps.Name,
                                    Price = ps.Price
                                })
                                .OrderByDescending(p => p.Price)
                                .ToArray()
                        }
                    })

                    .ToArray()
            };

            var result = XmlConverter.Serialize(user, "Users");
            return result;
        }

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context.Categories
            .Select(x => new CategoriesOutpudModel
            {
                Name = x.Name,
                Count = x.CategoryProducts.Count,
                AvgPrice = x.CategoryProducts.Average(a => a.Product.Price),
                TotalRevenue = x.CategoryProducts.Sum(s => s.Product.Price)
            }).OrderByDescending(x => x.Count)
            .ThenBy(x=>x.TotalRevenue).ToList();

            var result = XmlConverter.Serialize(categories, "Categories");

            return result;
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            var users = context.Users
                .Where(x => x.ProductsSold.Count > 0)
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .Select(x => new SoldProductsOutputMondel
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    SoldProducts = x.ProductsSold.Select(ps => new SoldProductsModel
                    {
                        Name = ps.Name,
                        Price = ps.Price,
                    }).ToList()
                })
                .Take(5)
                .ToList();

            var xml = XmlConverter.Serialize(users, "Users");

            return xml;
            
            
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context.Products
                .Where(x => x.Price >= 500 && x.Price <= 1000)
                .OrderBy(x => x.Price)
                .Select(x => new ProductsOutputModel
                {
                    Name = x.Name,
                    Price = x.Price,
                    Buyer = x.Buyer.FirstName + ' ' + x.Buyer.LastName,
                })
                .Take(10)
                .ToArray();

            var xml = XmlConverter.Serialize(products, "Products");

            //XmlSerializer xmlSerializer = new XmlSerializer(typeof(ProductsOutputModel[]),
            //    new XmlRootAttribute("Products"));

            //var text = new StringWriter();

            //xmlSerializer.Serialize(text, products);
            return xml;
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            string root = "CategoryProducts";
            var catProdDTO = XmlConverter.Deserializer<CategoryProductInputModel>(inputXml, root);
            
            var products = context.Products.Select(x => x.Id).ToList();
            var categories = context.Categories.Select(x => x.Id).ToList();

            var CatProds = catProdDTO
                .Where(x=>products.Contains(x.ProductId) && categories.Contains(x.CategoryId))
                .Select(x => new CategoryProduct()
            {
                CategoryId = x.CategoryId,
                ProductId = x.ProductId,
            }).ToList();

            context.CategoryProducts.AddRange(CatProds);
            context.SaveChanges();

            return $"Successfully imported {CatProds.Count}";

        }

        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            string root = "Categories";
            var categoriesDTO = XmlConverter.Deserializer<CategoryInputModel>(inputXml, root);

            var categories = categoriesDTO
                .Where(x=>x.Name!= null)
                .Select(x => new Category()
                 {
                     Name = x.Name,
                 })
                .ToList();
            context.Categories.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Count}";

        }

        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            string root = "Products";
            var productDTO = XmlConverter.Deserializer<ProductInputModel>(inputXml, root);

            var products = productDTO.Select(x => new Product()
            {
                Name = x.Name,
                Price = x.Price,
                BuyerId = x.BuyerId,
                SellerId = x.SellerId,
            }).ToList();

            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Count}";
        }

        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(UsersInputModel[]), 
                new XmlRootAttribute("Users"));

            var xmlRead = new StringReader(inputXml);

            var usersDTO = xmlSerializer.Deserialize(xmlRead) as UsersInputModel[];

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