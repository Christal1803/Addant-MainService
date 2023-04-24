using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AddantSDAL.DTO
{
    [DataContract]
    public class EmailTemplateDTO
    {
        [DataMember]
        public int IdEmailTemplate { get; set; }
        [DataMember]
        public int? IdTemplateType { get; set; }
        [DataMember]
        public string HeaderImageUrl { get; set; }
        [DataMember]
        public string Body { get; set; }
        [DataMember]
        public DateTime? CreatedDate { get; set; }
        [DataMember]
        public bool? IsActive { get; set; }  
        [DataMember]
        public string TemplateName { get; set; }
        [DataMember]
        public bool? IsDeleted { get; set; }
        [DataMember]
        public bool? Deleted { get; set; }
    }
}
