using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AddantSDAL.DTO
{
	[DataContract]
	public class EventCategoryDTO
	{
		[DataMember]
		public int IdEventCategory { get; set; }
		[DataMember]
		public string Name { get; set; }
		[DataMember]
		public string Description { get; set; }
		[DataMember]
		public bool IsActive { get; set; }
		[DataMember]
		public string BannerImgUrl { get; set; }
		[DataMember]
		public int? IdMainCategory { get; set; }
	}
}
