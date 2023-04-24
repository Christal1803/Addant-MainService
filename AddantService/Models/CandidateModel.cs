using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AddantService.Models
{
    public class CandidateModel
    {
        public int IdCandidate { get; set; }

        public int IdPosition { get; set; }
        public DateTime CreatedOn { get; set; }
        [Required(ErrorMessage = "{0} is required")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "{0} is required")]
        public string LastName { get; set; }
        
        public string Email { get; set; }
        [Required(ErrorMessage = "{0} is required")]
        public string Mobile { get; set; }
    
        public string Description { get; set; }
     
        public string ResumeUrl { get; set; }
    
        public bool IsDeleted { get; set; }

        public byte[] resumeContent { get; set; }
        public string format { get; set; }
        public string Status { get; set; }
        public bool? Deleted { get; set; }
    }
}