using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AddantService.Models
{
    public class BlogModel
    {
     
        public int IdBlog { get; set; }
      
        public string ConverImgUrl { get; set; }
        public byte[] CoverImg { get; set; }
       
        public string BannerImgUrl { get; set; }
        public byte[] BannerImg { get; set; }
       
        public string ContentUrl { get; set; }
        public Byte[] content { get; set; }
        
        public DateTime? CreatedOn { get; set; }
        
        public bool? IsDeleted { get; set; }
        
        public string MainHeader { get; set; }

        public byte[] ProfilePic { get; set; }
        public string ProfilePicUrl { get; set; }
        public string CreatedBy { get; set; }
        public int MinReadTime { get; set; }
        public string BlogContent { get; set; }
        public IFormFile FormProfilePic{ get; set; }
    }
}