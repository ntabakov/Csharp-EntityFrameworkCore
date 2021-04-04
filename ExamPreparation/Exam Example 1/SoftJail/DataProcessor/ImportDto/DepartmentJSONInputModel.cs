using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SoftJail.DataProcessor.ImportDto
{
    public class DepartmentJSONInputModel
    {
        [Required]
        [MinLength(3)]
        [MaxLength(25)]
        public string Name { get; set; }
        public ICollection<CellInputModel> Cells { get; set; }
    }

    public class CellInputModel
    {
        [Range(1,1000)]
        public int CellNumber { get; set; }
        public bool HasWindow { get; set; }
    }
}
