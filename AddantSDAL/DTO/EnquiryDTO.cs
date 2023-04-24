using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AddantSDAL.DTO
{
    [DataContract(Name = "Enquiry")]
    public class EnquiryDTO
    {
        [DataMember]
        public int IdEnquiry { get; set; }
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string LastName { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string Mobile { get; set; }
        [DataMember]
        public string Subject { get; set; }
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public string Status { get; set; }
        [DataMember]
        public bool? IsDeleted { get; set; }
        [DataMember]
        public DateTime? CreatedDate { get; set; }
        [DataMember]
        public int? CategoryId { get; set; }

        [DataMember]
        public string CategoryName { get; set; }
        [DataMember]
        public bool? Deleted { get; set; }
    }

    public class EnquiryCategoryDTO
    {
        [DataMember]
        public int CategoryId { get; set; }
        [DataMember]
        public string CategoryName { get; set; }
        [DataMember]
        public bool? IsActive { get; set; }
    }
}
