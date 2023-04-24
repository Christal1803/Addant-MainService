using AddantSDAL.DTO;
using AddantService.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddantSDAL.DAL
{
    public interface IEmbedCodeRepository
    { 
            DALResult<EmbededCodeDTO> CreateEmbededCode(EmbededCodeDTO embededCodeData);
            DALResult<List<EmbededCodeDTO>> GetAllEmbededCode(DateTime? startDate = null, DateTime? endDate = null, bool isAdminCall = false);
            DALResult<List<EmbededCodeDTO>> GetEmbededCodeById(int idEmbededCode);
            DALResult<bool> Delete(int idEmbededCode, bool isDelete);
           DALResult<bool> UpdateDeletedStatus(int IdEmbedCode, bool isDeleted);

    }
}
