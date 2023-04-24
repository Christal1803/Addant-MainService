using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AddantService.Models
{
	public class UserPrivilegeModel
	{
		public int IdUserPrivilege { get; set; }
		public bool CreatePage { get; set; }
		public bool Edit { get; set; }
		public bool ViewPage { get; set; }
		public bool DisablePage { get; set; }
		[Required(ErrorMessage = "{0} is required")]
		public int IdPage { get; set; }
		[Required(ErrorMessage ="{0} is required")]
		public int IdUserRole { get; set; }
		public bool IsDeleted { get; set; }
		public DateTime CreatedDate { get; set; }
		public bool Deleted { get; set; }
	}
}