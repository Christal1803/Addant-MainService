using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AddantService.Models
{
    public class EmbedCodeModel
    {
        public int IdEmbedCode { get; set; }
        public int IdCodeType { get; set; }
        [Required(ErrorMessage = "{0} is required")]
        public string Code { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}