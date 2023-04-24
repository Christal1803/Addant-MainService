using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AddantSDAL.DTO
{
    [DataContract(Name ="User")]
   public class UserDTO
    {
        [DataMember]
        public Guid? UserId { get; set; }
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string LastName { get; set; }
        [DataMember] 
        public string Email { get; set; }
        [DataMember] 
        public string Mobile { get; set; }
        [DataMember] 
        public string BloodGroup { get; set; }
        [DataMember] 
        public DateTime? Dob { get; set; }
        [DataMember] 
        public string Designation { get; set; }
        [DataMember] 
        public string Role { get; set; }
        [DataMember] 
        public int? EmployeeID { get; set; }
        [DataMember]
        public string ProfileImageUrl { get; set; }
        [DataMember] 
        public string Username { get; set; }
        [DataMember] 
        public string Password { get; set; }
        [DataMember] 
        public DateTime? CreatedOn { get; set; }
        [DataMember] 
        public bool? IsActive { get; set; }

        [DataMember]
        public int? IdUserRole { get; set; }


        [DataMember]
        public bool? IsDeleted { get; set; }
        [DataMember]
        public bool? Deleted { get; set; }

    }
    public class OTPDto
    {

        [DataMember]
        public string UserName { get; set; }
        [DataMember]
        public string OTP { get; set; }
        [DataMember]
        public DateTime? TimeStamp { get; set; }
        [DataMember]
        public bool? Status { get; set; }
        
       
    }
}
