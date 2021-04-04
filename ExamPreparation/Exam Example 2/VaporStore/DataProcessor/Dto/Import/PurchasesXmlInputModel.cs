using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;
using VaporStore.Data.Models.Enums;

namespace VaporStore.DataProcessor.Dto.Import
{
    [XmlType("Purchase")]
    public class PurchasesXmlInputModel
    {

        //TODO: May be required
        [Required]
        [XmlAttribute("title")]
        public string Title { get; set; }

        [Required]
        [EnumDataType(typeof(PurchaseType))]
        public string Type { get; set; }

        [Required]
        [RegularExpression("[A-Z0-9]{4}-[A-Z0-9]{4}-[A-Z0-9]{4}")]
        public string Key { get; set; }

        [Required]
        [RegularExpression("[0-9]{4} [0-9]{4} [0-9]{4} [0-9]{4}")]
        public string Card { get; set; }

        [Required]
        public string Date { get; set; }
    }
    /*
     *<Purchase title="The Crew 2">
    <Type>Retail</Type>
    <Key>DCU0-S60G-NTQJ</Key>
    <Card>5208 8381 5687 8508</Card>
    <Date>22/01/2017 09:33</Date>
  </Purchase>


    •	Type – enumeration of type PurchaseType, with possible values (“Retail”, “Digital”) (required) 
•	ProductKey – text, which consists of 3 pairs of 4 uppercase Latin letters and digits, separated by dashes (ex. “ABCD-EFGH-1J3L”) (required)
•	Date – Date (required)

    CARD
    •	Number – text, which consists of 4 pairs of 4 digits, separated by spaces (ex. “1234 5678 9012 3456”) (required)
     *
     */
}
