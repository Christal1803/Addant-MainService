using AddantSDAL.DTO;
using AddantService.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddantSDAL.DAL
{
    public interface IUserRepository
    {
        DALResult<List<UserDTO>> GetAllUser(DateTime? startDate, DateTime? endDate, string searchText = "", string status = "", bool IsActive = true);
        DALResult<UserDTO> Create(UserDTO userDto);
        DALResult<UserDTO> GetUserById(string userId);
        DALResult<bool> DisableUser(string IdUser = "", bool isDeleted = false);
        DALResult<bool> ChangePassword(UserDTO userDTO);

        DALResult<bool> Delete(string IdUser = "");
		DALResult<UserDTO> UploadImage(UserDTO addantLifeDTO);
        DALResult<bool> UpdateDeletedStatus(Guid idUser, bool isDeleted); 
    }
}
