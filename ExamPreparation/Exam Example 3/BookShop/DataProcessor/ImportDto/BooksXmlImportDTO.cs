using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;
using BookShop.Data.Models.Enums;

namespace BookShop.DataProcessor.ImportDto
{
    [XmlType("Book")]
    public class BooksXmlImportDTO
    {
        [Required]
        [MinLength(3)]
        [MaxLength(30)]
        public string Name { get; set; }

        [Required]
        [EnumDataType(typeof(Genre))]
        public string Genre { get; set; }


        //TODO: May have to be changed to decimal.MaxValue!
        [Range(0.01,double.MaxValue)]
        public decimal Price { get; set; }

        [Range(50,5000)]
        public int Pages { get; set; }

        [Required]
        public string PublishedOn { get; set; }

    }
}
