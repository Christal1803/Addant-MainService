using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AddantService.Models
{
    public class AddantLifeModel
    {
        public byte[] CoverImg { get; set; }
        public int IdAddantLife { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CoverImgUrl { get; set; }
        public string BannerImgUrl { get; set; }
        public byte[] ThumbNailInnerImg { get; set; }
        public string ThumbNailInnerUrl { get; set; }
        public string InnerImgURl { get; set; }
        public bool IsDeleted { get; set; }
        public byte[] InnerImg { get; set; }
        public DateTime? CreatedDate { get; set; }
        public List<AddantLifeDetialModel> AddantLifeDetials { get; set; }
        public int IdEventCategory { get; set; }
        public int IdMainCategory { get; set; }

    }

public class AddantLifeDetialModel
    {
        public int IdAddantLifeImage { get; set; }
        public int IdAddantLife { get; set; }
        public string InnerCaption { get; set; }
        public string InnerImageUrl { get; set; }
        public byte[] InnerImage { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}