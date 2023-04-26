using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AddantService.Models
{
	public class EventCategoryModel
	{
		
		public int IdEventCategory { get; set; }
		[Required(ErrorMessage ="{0} is required")]
		public string Name { get; set; }
		public string Description { get; set; }
		public bool IsActive { get; set; }
        public bool Deleted { get; set; }
		public string BannerImgUrl { get; set; }
		public int? IdMainCategory { get; set; }
	}
}