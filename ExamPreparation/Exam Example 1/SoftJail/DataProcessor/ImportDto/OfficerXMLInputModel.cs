using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;
using SoftJail.Data.Models;
using SoftJail.Data.Models.Enums;

namespace SoftJail.DataProcessor.ImportDto
{
    [XmlType("Officer")]
    public class OfficerXMLInputModel
    {
        [Required]
        [MinLength(3)]
        [MaxLength(30)]
        [XmlElement("Name")]
        public string Name { get; set; }

        [Range(0,double.MaxValue)]
        [XmlElement("Money")]
        public decimal Money { get; set; }

        [Required]
        [XmlElement("Position")]
        [EnumDataType(typeof(Position))]
        
        public string Position { get; set; }

        [Required]
        [XmlElement("Weapon")]
        [EnumDataType(typeof(Weapon))]


        public string Weapon { get; set; }

        [XmlElement("DepartmentId")]

        public int DepartmentId { get; set; }
        
        [XmlArray("Prisoners")]
        public PrisonerXMLInputModel[] Prisoners { get; set; }
    }

    [XmlType("Prisoner")]
    public class PrisonerXMLInputModel
    {
        [XmlAttribute("id")]
        public int Id { get; set; }
    }
}
