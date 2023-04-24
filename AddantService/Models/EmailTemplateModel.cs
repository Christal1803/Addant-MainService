using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AddantService.Models
{

    public class EmailTemplateModel
    {
        public int IdEmailTemplate { get; set; }
        [Required(ErrorMessage ="{0} is required")]
        public int IdTemplateType { get; set; }
        public string HeaderImageUrl { get; set; }
        [Required(ErrorMessage ="{0} is required")]
        public string Body { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public bool? Deleted { get; set; }
    }
}