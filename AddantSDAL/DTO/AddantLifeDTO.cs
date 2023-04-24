using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AddantSDAL.DTO
{
    [DataContract(Name = "AddantLife")]
    public class AddantLifeDTO
    {
        [DataMember]
        public int IdAddantLife { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string CoverImgUrl { get; set; }
        [DataMember]
        public string  BannerImgUrl { get; set; }    
        [DataMember]
        public byte[] coverImg { get; set; }
        [DataMember]
        public string ThumbNailInnerUrl { get; set; }
        [DataMember]
        public byte[] thumbNailInnerImg { get; set; }
        [DataMember]
        public string InnerImgURl { get; set; }
        [DataMember]
        public byte[] innerImg { get; set; }
        [DataMember]
        public bool IsDeleted { get; set; }
        [DataMember]
        public DateTime? CreatedDate { get; set; }
        [DataMember]
        public bool? Deleted { get; set; }
        [DataMember]
        public List<AddantLifeDetialDTO> addantLifeDetials { get; set; }
        [DataMember]
        public int IdEventCategory { get; set; }
        public string EventCategoryName { get; set; }

      
    }
    public class AddantLifeDetialDTO
    {
        public int IdAddantLifeImage { get; set; }
        public int IdAddantLife { get; set; }
        public string InnerCaption { get; set; }
        public string InnerImageUrl { get; set; }
        public byte[] InnerImage { get; set; }
        public bool IsDeleted { get; set; }
        public bool? Deleted { get; set; }
    }
}
