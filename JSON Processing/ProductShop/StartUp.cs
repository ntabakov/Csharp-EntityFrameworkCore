using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ProductShop.Data;
using ProductShop.Models;

namespace ProductShop
{
    public class StartUp
    {
        private static string _categoriesPath = "../../../Datasets/categories.json";
        private static string _usersPath = "../../../Datasets/users.json";
        private static string _productsPath = "../../../Datasets/products.json";
        private static string _categoriesAndProductsPath = "../../../Datasets/categories-products.json";



        public static void Main(string[] args)
        {
            var db = new ProductShopContext();
            //var jsonFile = File.ReadAllText(_categoriesAndProductsPath);
            // Console.WriteLine(ImportCategoryProducts(db,jsonFile));
            Console.WriteLine(GetUsersWithProducts(db));

        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {

            var users = context.Users
                .Include(x=>x.ProductsSold)
                .ToList()
                .Where(x => x.ProductsSold.Count > 0)

                .Select(x => new
                {
                    x.FirstName,
                    x.LastName,
                    x.Age,
                    SoldProducts = new
                    {
                        Count = x.ProductsSold.Count(p => p.Buyer != null),
                        Products = x.ProductsSold
                            .Where(c => c.Buyer != null)
                            .Select(c => new
                            {
                                Name = c.Name,
                                Price = c.Price,
                            })
                            .ToList()

                    }
                })
                .ToList()
                .OrderByDescending(x => x.SoldProducts.Count)
                .ToList();


            var resultObj = new
            {
                UsersCount = users.Count(),
                Users = users
            };

            var settings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
            };
            var result = JsonConvert.SerializeObject(resultObj, settings);
            return result;
        }
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context.Categories
                .OrderByDescending(c => c.CategoryProducts.Count)
                .Select(c => new
                {
                    category = c.Name,
                    productsCount = c.CategoryProducts.Count,
                    averagePrice = c.CategoryProducts.Average(cp => cp.Product.Price).ToString("f2"),
                    totalRevenue = c.CategoryProducts.Sum(cp => cp.Product.Price).ToString("f2")
                })
                .ToList();

            var settings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
            };
            var result = JsonConvert.SerializeObject(categories, settings);
            return result;
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            var users = context.Users
                .Where(x => x.ProductsSold.Any(c => c.Buyer != null))
                .Where(x => x.ProductsSold.Count >= 1)
                .Select(x => new
                {
                    x.FirstName,
                    x.LastName,
                    SoldProducts = x.ProductsSold

                        .Select(c => new
                        {
                            c.Name,
                            c.Price,
                            BuyerFirstName = c.Buyer.FirstName,
                            BuyerLastName = c.Buyer.LastName,
                        }).ToList()

                })
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .ToList();

            var settings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
            };
            //foreach (var user in users)
            //{
            //    foreach (var soldProduct in user.SoldProducts)
            //    {
            //        if (soldProduct.BuyerFirstName == null || soldProduct.BuyerLastName == null)
            //        {
            //            user.SoldProducts.Remove(soldProduct);
            //        }
            //    }
            //}
            var result = JsonConvert.SerializeObject(users, settings);
            return result;
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context.Products
                .Where(x => x.Price >= 500 && x.Price <= 1000)
                .Select(x => new
                {
                    x.Name,
                    x.Price,
                    Seller = x.Seller.FirstName + " " + x.Seller.LastName
                })
                .OrderBy(x => x.Price)
                .ToList();
            var settings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
            };
            var result = JsonConvert.SerializeObject(products, settings);

            return result;
        }
        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            var catProd = JsonConvert.DeserializeObject<CategoryProduct[]>(inputJson);
            int count = catProd.Length;
            context.AddRange(catProd);
            context.SaveChanges();

            return $"Successfully imported {count}";

        }

        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            var categories = JsonConvert.DeserializeObject<Category[]>(inputJson);
            int count = categories.Length;
            foreach (var category in categories)
            {
                if (category.Name == null)
                {
                    count--;
                    continue;

                }
                context.Categories.Add(category);
            }
            context.SaveChanges();
            return $"Successfully imported {count}";

        }
        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            var users = JsonConvert.DeserializeObject<User[]>(inputJson);
            int count = users.Length;
            context.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {count}";
        }

        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            var products = JsonConvert.DeserializeObject<Product[]>(inputJson);
            int count = products.Length;
            context.AddRange(products);
            context.SaveChanges();


            return $"Successfully imported {count}";
        }
    }
}