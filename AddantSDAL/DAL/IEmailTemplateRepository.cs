using AddantSDAL.DTO;
using AddantService.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddantSDAL.DAL
{
    public interface IEmailTemplateRepository
    {
        DALResult<EmailTemplateDTO> CreateEmailTemplate(EmailTemplateDTO emailTemplateDTO);
        DALResult<List<EmailTemplateDTO>> GetAllEmailTemplate(DateTime? startDate = null, DateTime? endDate = null, string searchText = "", bool isAdminCall = false);
        DALResult<EmailTemplateDTO> GetAllEmailTemplateById(int emailTemplateById,string templateType="");
        DALResult<bool> Delete(int emailTemplateById, bool isDelete);
        DALResult<List<TemplateTypeDTO>> GetTemplatetype();
        DALResult<bool> UpdateDeletedStatus(int IdEmailTemplate, bool isDeleted);
    }
}
