using AddantSDAL.DTO;
using AddantService.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddantSDAL.DAL
{
	public interface IUserPrivilegeRepository
	{
		DALResult<List<UserPrivilegeDTO>> Create(List<UserPrivilegeDTO> userPrivilegeDTO);
		DALResult<UserPrivilegeDTO> GetUserPrivilegeById(int idUserPrivilege);
		DALResult<List<UserPrivilegeDTO>> GetAllUserPrivilege(bool isAdminCall = false);
		DALResult<bool> Delete(int idUserPrivilege, bool isDeleted);
		DALResult<List<UserPrivilegeDTO>>  CheckUserPrivilegeExist(Guid idUser);
	    DALResult<List<PageDTO>> GetAllPage();
		DALResult<bool> Delete(int idUserPrivilege);
		DALResult<List<UserPrivilegeDTO>> GetUserPrivilegeByRoleId(int roleId);
        DALResult<bool> UpdateDeletedStatus(int RoleId, bool isDeleted);

    }
}
