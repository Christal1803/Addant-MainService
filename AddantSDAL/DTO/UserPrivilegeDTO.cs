using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AddantSDAL.DTO
{
	[DataContract]
	public class UserPrivilegeDTO
	{
		[DataMember]
		public int IdUserPrivilege { get; set; }
		[DataMember]
		public bool? CreatePage { get; set; }
		[DataMember]
		public bool? Edit { get; set; }
		[DataMember]
		public bool? ViewPage { get; set; }
		[DataMember]
		public bool? DisablePage { get; set; }
		[DataMember]
		public int? IdPage { get; set; }
		[DataMember]
		public int? IdUserRole { get; set; }
		[DataMember]
		public bool? IsDeleted { get; set; }
		[DataMember]
		public DateTime? CreatedDate { get; set; }
        [DataMember]
        public bool? Deleted { get; set; } 
    }
}
