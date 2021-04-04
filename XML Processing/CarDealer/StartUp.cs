using System;
using System.Collections.Generic;
using System.IO;
using CarDealer.Data;
using CarDealer.Data.Dto.Import;
using CarDealer.Models;
using CarDealer.XmlHelper;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            var db = new CarDealerContext();
            //db.Database.EnsureDeleted();
            //db.Database.EnsureCreated();

            var suppliersXml = File.ReadAllText("../../../Datasets/suppliers.xml");

           // Console.WriteLine(ImportSuppliers(db,suppliersXml));


        }

        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            var xml = XmlConverter.Deserializer<SuppliersImportDTO>(inputXml, "Suppliers");

            var suppliersList = new List<Supplier>();

            foreach (var supplier in xml)
            {
                var supplierToAdd = new Supplier
                {
                    Name = supplier.Name,
                    IsImporter = supplier.IsImporter,
                };

                suppliersList.Add(supplierToAdd);
            }

            context.Suppliers.AddRange(suppliersList);
            context.SaveChanges();

            return $"Successfully imported {suppliersList.Count}";
        }
    }
}