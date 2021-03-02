using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P01_StudentSystem.Data.Models
{
    public class Resource
    {
        public int ResourceId { get; set; }
        [Required]
        [MaxLength(50)]
        [Column(TypeName = "nvarchar(50)")]

        public string Name { get; set; }      
        [Required]
        [Column(TypeName = "varchar(255)")]

        public string Url { get; set; }

        public ResourceType ResourceType { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }


    }
}
