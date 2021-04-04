using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace VaporStore.DataProcessor.Dto.Export
{
    [XmlType("User")]
    public class UsersXmlExportModel
    {
        [XmlAttribute("username")]
        public string Username { get; set; }

        [XmlArray("Purchases")]
        public PurchasesXmlOutputModel[] Purchases { get; set; }

        public decimal TotalSpent { get; set; }

    }

    [XmlType("Purchase")]
    public class PurchasesXmlOutputModel
    {
        public string Card { get; set; }
        public string Cvc { get; set; }
        public string Date { get; set; }
        public GameXmlOutputModel Game { get; set; }
    }

    [XmlType("Game")]
    public class GameXmlOutputModel
    {
        [XmlAttribute("title")]
        public string Title { get; set; }

        public string Genre { get; set; }
        public decimal Price { get; set; }
    }
}
