using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AddantSDAL.DTO
{
	[DataContract]
	public class UserRoleDTO
	{
		[DataMember]
	    public int IdUserRole { get; set; }
		[DataMember]
		public string Name { get; set; }
		[DataMember]
		public string Description { get; set; }
		[DataMember]
		public bool IsActive { get; set; }

		[DataMember]
		public int UserCount { get; set; }
        [DataMember]
        public bool? IsFullAccess { get; set; }
        [DataMember]
        public bool? Deleted { get; set; }
    }
}

