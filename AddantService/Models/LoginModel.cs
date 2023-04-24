using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AddantService.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "{0} is required")]
        [Range(1, int.MaxValue, ErrorMessage = "{0} should be between {1} & {2}")]
        public System.Guid UserId { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        public string Password { get; set; }

        public bool? IsActive { get; set; }
        public string Token { get; set; }
    }
    public class DashBoardBlockDataModel
    {
        public string Name { get; set; }
        public int Total { get; set; }
        public int Today { get; set; }
    }
}