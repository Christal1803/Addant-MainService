using AddantSDAL.DTO;
using AddantService.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddantSDAL.DAL
{
   public interface IAddantLifeRepository
    {
        DALResult<int> ComposeAddantLife(AddantLifeDTO addantLifeDTO);
        DALResult<List<AddantLifeDTO>> GetAllAddantLife(int idAddantLife = 0, bool isAdminCall = false, string searchText = "", DateTime? startDate = null, DateTime? endDate = null);
        DALResult<List<AddantLifeDetialDTO>> GetAllInnerImage(int idAddantLife = 0);
        DALResult<bool> DisableAddantLife(int idAddantLife = 0, bool isDeleted = false);
        DALResult<bool> DisableAddantLifeDetail(int idAddantLife = 0, bool isDeleted = false);
        DALResult<List<IGrouping<int, AddantLifeDTO>>> GetAllAddantLifeByCategory(bool isAdminCall = false, string category = "", string groupBy = "");
        DALResult<bool> UpdateDeletedStatus(int AddantLifeId, bool isDeleted);
        DALResult<bool> DetailUpdateDeletedStatus(int DetailId, bool isDeleted);

    }
}
