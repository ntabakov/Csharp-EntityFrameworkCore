﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using SoftJail.Data.Models;

namespace SoftJail.DataProcessor.ImportDto
{
    public class PrisonerJSONInputModel
    {

        [Required]
        [MinLength(3)]
        [MaxLength(20)]
        public string FullName { get; set; }

        [Required]
        [RegularExpression(@"The [A-Z][a-z]+")]
        public string Nickname { get; set; }
        [Range(18,65)]
        public int Age { get; set; }

        [Required]
        public string IncarcerationDate { get; set; }
        public string ReleaseDate { get; set; }
        [Range(0,double.MaxValue)]
        public decimal? Bail { get; set; }
        public int? CellId { get; set; }
        
        public ICollection<MailJSONInputModel> Mails { get; set; }
    }

    public class MailJSONInputModel
    {
        [Required]
        public string Description { get; set; }

        [Required]
        public string Sender { get; set; }

        [Required]
        [RegularExpression(@"[A-Za-z0-9\s]+ str\.")]
        public string Address { get; set; }
    }
}
