using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AddantSDal.DTO
{
    [DataContract(Name = "User")]
    public class LoginDTO
    {
        [DataMember]
        public System.Guid? UserId { get; set; }
        [DataMember]
        public string UserName { get; set; }
        [DataMember]
        public string Password { get; set; }
        [DataMember]
        public bool? IsActive { get; set; }
        [DataMember]
        public string Token { get; set; }
    }
    public class DashBoardBlockData
    { 
    [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int Total { get; set; }
        [DataMember]
        public int BlogToReview { get; set; }
        [DataMember]
        public int BlogPublished { get; set; }
        [DataMember]
        public int TotalToReview { get; set; }
        [DataMember]
        public int ShortListed  { get; set; } 

	}
}
