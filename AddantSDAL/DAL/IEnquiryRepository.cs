using AddantSDAL.DTO;
using AddantService.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddantSDAL.DAL
{
    public interface IEnquiryRepository
    {
        DALResult<EnquiryDTO> CreateEnquiry(EnquiryDTO enquiryData);
        DALResult<List<EnquiryDTO>> GetAllEnquiry(DateTime? StartDate = null, DateTime? EndDate = null, string searchText="", bool isAdminCall = false, string status ="");
        DALResult<List<EnquiryDTO>> GetAllEnquiryById(int IdEnquiry);
        DALResult<bool> Delete(int IdEnquiry, bool isDelete);
        DALResult<List<EnquiryDTO>> GetAllEnquiriesByCategory(int CategoryId);
        DALResult<List<EnquiryCategoryDTO>> GetAllEnquiryCategories();
        DALResult<bool> Delete(int IdEnquiry);
        DALResult<List<EnquiryDTO>> GetAllEnquiries();
        DALResult<bool> UpdateDeletedStatus(int IdEnquiry, bool isDeleted);


    }
}
