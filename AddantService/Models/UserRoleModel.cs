using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AddantService.Models
{
	public class UserRoleModel
	{
		public int IdUserRole { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public bool IsActive { get; set; }
	}
}