﻿using System.ComponentModel.DataAnnotations;
using WebApiAuthors.Validations;

namespace WebApiAuthors.DTOs
{
    public class BookPatchDTO
    {
        [Required(ErrorMessage = "Field {0} is required")]
        [StringLength(maximumLength: 50, ErrorMessage = "Field {0} must be maximum {1} characters")]
        [CapitalLetter]
        public string Title { get; set; }
        public DateTime PublicationDate { get; set; }
    }
}
