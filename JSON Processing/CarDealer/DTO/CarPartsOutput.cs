using System;
using System.Collections.Generic;
using System.Text;
using CarDealer.Models;

namespace CarDealer.DTO
{
    public class CarPartsOutput
    {
        public string Make { get; set; }
        public string Model { get; set; }

        public long TravelledDistance { get; set; }

        public IEnumerable<CustomPartCar> Parts { get; set; }
    }

    public class CustomPartCar
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}
