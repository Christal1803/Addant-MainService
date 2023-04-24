using AddantSDAL.DTO;
using AddantService.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddantSDAL.DAL
{
	public interface IPositionRepository
	{
		DALResult<List<PositionDTO>> GetAllPostion(bool isAdminCall, DateTime? startDate = null, DateTime? endDate = null, string SearchText = "");
		DALResult<PositionDTO> GetPositionById(int positionId);
		DALResult<PositionDTO> ComposePosition(PositionDTO data);
		DALResult<bool> DeletePosition(int positionId, bool isDeleted = false);

		DALResult<bool> DeletePosition(int positionId);
		DALResult<bool> ChangeStatus(int PositionId, int Status);

		DALResult<List<ApplicationBucketDTO>> ApplicationBucket();
        DALResult<bool> UpdateDeletedStatus(int IdPosition, bool isDeleted);

    }
}
