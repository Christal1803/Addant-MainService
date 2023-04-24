using AddantSDAL.DTO;
using AddantService.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddantSDAL.DAL
{
	public interface IEventCategoryRepository
	{
		DALResult<EventCategoryDTO> CreateEventCategory(EventCategoryDTO eventCategoryData);
		DALResult<List<EventCategoryDTO>> GetAllEventCategory( bool isAdminCall = false, bool isMaincategory = false);
		DALResult<EventCategoryDTO> GetAllEventCategoryById(int IdEventCategory);
		DALResult<bool> Delete(int IdEnquiry, bool isDelete);
		DALResult<EventCategoryDTO> GetAllEventByIdMainCategory(int IdMainCategory);
		DALResult<List<EventCategoryDTO>> EventCategoryByCategory(int IdMainCategory);
	}
}
