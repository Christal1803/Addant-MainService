using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AddantSDAL.DTO
{
    [DataContract]
    public class BlogDTO
    {
        [DataMember]
        public int IdBlog { get; set; }
        [DataMember]
        public string ConverImgUrl { get; set; }
        [DataMember]
        public byte[] CoverImg { get; set; }
        [DataMember]
        public string BannerImgUrl { get; set; }
        [DataMember]
        public byte[] BannerImg { get; set; }
        [DataMember]
        public string ContentUrl { get; set; }
        [DataMember]
        public DateTime? CreatedOn { get; set; }
        [DataMember]
        public bool? IsDeleted { get; set; }
        [DataMember]
        public string MainHeader { get; set; }
        [DataMember]
        public byte[] Content { get; set; }
        [DataMember]
        public string ProfilePicUrl { get; set; }
        [DataMember]
        public string CreatedBy { get; set; }
        [DataMember]
        public int? MinReadTime { get; set; }
        [DataMember]
        public byte[] ProfilePic { get; set; }
        [DataMember]
        public string BlogContent { get; set; }

        [DataMember]
        public bool? Deleted { get; set; }

    }
}
