using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AddantSDAL.DTO
{
    [DataContract]
    public class EmbededCodeDTO
    {
        [DataMember]
        public int IdEmbedCode { get; set; }
        [DataMember]
        public int? IdCodeType { get; set; }
        [DataMember]
        public string Code { get; set; }
        [DataMember]
        public bool? IsDeleted { get; set; }
        [DataMember]
        public DateTime? CreatedDate { get; set; }

        [DataMember]
        public bool? Deleted { get; set; }
    }
}
