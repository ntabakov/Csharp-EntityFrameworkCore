using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using CarDealer.Data;
using CarDealer.DTO;
using CarDealer.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CarDealer
{
    public class StartUp
    {
        private static string _carsPath = "../../../Datasets/cars.json";
        private static string _customersPath = "../../../Datasets/customers.json";
        private static string _partsPath = "../../../Datasets/parts.json";
        private static string _salesPath = "../../../Datasets/sales.json";
        private static string _suppliersPath = "../../../Datasets/suppliers.json";

        public static void Main(string[] args)
        {
            var db = new CarDealerContext();
            var json = File.ReadAllText(_customersPath);
            Console.WriteLine(ImportCustomers(db, json));

        }

        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            var sales = JsonConvert.DeserializeObject<Sale[]>(inputJson);
            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Length}.";

        }

        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            var customers = JsonConvert.DeserializeObject<Customer[]>(inputJson);
            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Length}.";

        }
        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            var cars = JsonConvert.DeserializeObject<List<CarInputModel>>(inputJson);
            var listOfCars = new List<Car>();
            foreach (var car in cars)
            {
                var currCar = new Car()
                {
                    Make = car.Make,
                    Model = car.Model,
                    TravelledDistance = car.TravelledDistance,
                };

                foreach (var partId in car?.PartsId.Distinct())
                {
                    currCar.PartCars.Add(new PartCar
                    {
                        PartId = partId,
                    });
                }
                listOfCars.Add(currCar);
            }
            context.Cars.AddRange(listOfCars);
            context.SaveChanges();
            return $"Successfully imported {listOfCars.Count}.";

        }

        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            var suppliers = context.Suppliers.Select(x => x.Id).ToList();


            var parts = JsonConvert.DeserializeObject<Part[]>(inputJson)
                .Where(x=>suppliers.Contains(x.SupplierId)).ToList();

            context.Parts.AddRange(parts);
            int count = parts.Count;
            context.SaveChanges();
            return $"Successfully imported {count}.";

        }

        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            var suppliers = JsonConvert.DeserializeObject<Supplier[]>(inputJson);
            int count = suppliers.Length;
            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();
            return $"Successfully imported {count}.";
        }
    }
}