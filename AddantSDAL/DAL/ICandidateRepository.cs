using AddantSDAL.DTO;
using AddantService.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddantSDAL.DAL
{
    public interface ICandidateRepository
    {
        DALResult<List<CandidateDTO>> GetAllCandidate(DateTime? startDate, DateTime? endDate, bool isadminCall, string searchText ="",string status="");
        DALResult<CandidateDTO> Create(CandidateDTO candidateDto);
        DALResult<CandidateDTO> GetCandidateById(int candidateId);
        DALResult<bool> DisableCandidate(int IdCandidate = 0, bool isDeleted = false);
        DALResult<bool> Delete(int IdCandidate = 0);
        DALResult<List<CandidateDTO>> GetCandidateByPositionId(int positionId);
        DALResult<bool> UpdateDeletedStatus(int IdCandidate, bool isDeleted);


    }
}
