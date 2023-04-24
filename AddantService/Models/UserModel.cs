using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AddantService.Models
{
    public class UserModel
    {
            private Guid _userID = System.Guid.NewGuid();
            public Guid UserId { get { return _userID; }
                          set { _userID = value; } }
            [Required]
            public string FirstName { get; set; }
           
            public string LastName { get; set; }
            [DataType(DataType.EmailAddress)]
            [EmailAddress]
            public string Email { get; set; }
           
            public string Mobile { get; set; }
           
            public string BloodGroup { get; set; }
           
            public DateTime Dob { get; set; }
           
            public string Designation { get; set; }
           
            public string Role { get; set; }
           
            public int EmployeeID { get; set; }
           
            public string ProfileImageUrl { get; set; }
            [Required]
            public string Username { get; set; }
            [Required]
            public string Password { get; set; }
           
            public DateTime CreatedOn { get; set; }
           
            public bool IsActive { get; set; }
        [Required]
        public int IdUserRole { get; set; }

    }
    public class UserResetModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string NewPassword { get; set; }
    }
    }