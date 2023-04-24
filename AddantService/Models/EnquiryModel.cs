using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AddantService.Models
{
    public class EnquiryModel
    {
        public int IdEnquiry { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "{0} is required")]

        public string Email { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        public string Mobile { get; set; }
      
        public string Subject { get; set; }
     
        public string Message { get; set; }
     
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CategoryId { get; set; }
        public bool? Deleted { get; set; }

    }
}