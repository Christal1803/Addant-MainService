using AddantSDAL.DTO;
using AddantService.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddantSDAL.DAL
{
	public interface IUserRoleRepository
	{
		DALResult<UserRoleDTO> CreateUserRole(UserRoleDTO userRoleDTO);
		DALResult<List<UserRoleDTO>> GetAllUserRole(string searchText = "");
		DALResult<List<UserRoleDTO>> GetAllUserRoleById(int IdUserRole);
		DALResult<bool> Delete(int IdUserRole, bool isDelete);
		DALResult<bool> DeleteUserRole(int IdUserRole);
		DALResult<List<UserDTO>> GetAllUsersByRole(int IdUserRole);
        DALResult<bool> UpdateDeletedStatus(int idUserRole, bool isDeleted);

    }
}
