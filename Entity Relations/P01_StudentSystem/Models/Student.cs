using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace P01_StudentSystem.Models
{
    public class Student
    {
        public int StudentId { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }

        [Column(TypeName = "char")]
        [MaxLength(10)]
        public string PhoneNumber { get; set; }

        public DateTime RegisteredOn { get; set; }
        public DateTime? Birthday { get; set; }
    }
}
