using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ProductShop.Dtos.Export
{
    [XmlType("User")]
    public class SoldProductsOutputMondel
    {
        

        [XmlElement("firstName")]
        public string FirstName { get; set; }

        [XmlElement("lastName")]
        public string LastName { get; set; }

        
        [XmlArray("soldProducts")]
        public List<SoldProductsModel> SoldProducts { get; set; }
    }

    [XmlType("Product")]
    public class SoldProductsModel
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("price")]
        public decimal Price { get; set; }
    }
}
