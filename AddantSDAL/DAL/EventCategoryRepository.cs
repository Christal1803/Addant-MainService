using AddantSDAL.DTO;
using AddantService;
using AddantService.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddantSDAL.DAL
{
	public class EventCategoryRepository : IEventCategoryRepository
	{
		public DALResult<EventCategoryDTO> CreateEventCategory(EventCategoryDTO eventCategoryData)
		{
            try
            {
                using (var gt = new AddantEntities1())
                {
                    EventCategory _data = new EventCategory
                    {
                         IdEventCategory = eventCategoryData.IdEventCategory,
                         Name = eventCategoryData.Name,
                         Description = eventCategoryData.Description,
                        IsActive = eventCategoryData.IsActive,
                         IdMainCategory = eventCategoryData.IdMainCategory
                    };
                    var existingEnquiry = gt.EventCategories.Where(x => x.IdEventCategory == eventCategoryData.IdEventCategory).FirstOrDefault();
                    if (existingEnquiry != null)
                    {
                        _data.BannerImgUrl = eventCategoryData.BannerImgUrl != string.Empty ? eventCategoryData.BannerImgUrl : existingEnquiry.BannerImgUrl;
                        gt.Entry(existingEnquiry).CurrentValues.SetValues(_data);
                    }
                    else
                    {
                        _data.BannerImgUrl = eventCategoryData.BannerImgUrl;
                        gt.EventCategories.Add(_data);
                    }
                    gt.SaveChanges();
                    eventCategoryData.IdEventCategory = _data.IdEventCategory;
                    return new DALResult<EventCategoryDTO>(Status.Created, eventCategoryData, null, null);
                }
            }
            catch (Exception ex) { return new DALResult<EventCategoryDTO>(Status.Exception, null, ex.Message.ToString(), null); }
        }

		public DALResult<bool> Delete(int idEventCategory, bool isDelete)
		{
            try
            {
                using (var gt = new AddantEntities1())
                {
                    EventCategory res = gt.EventCategories.Where(x => x.IdEventCategory == idEventCategory).FirstOrDefault();
                    res.IsActive = isDelete;
                    gt.Entry(res).CurrentValues.SetValues(res);
                    gt.SaveChanges();
                    return new DALResult<bool>(Status.Deleted, true, null, null);
                }
            }
            catch (Exception ex) { return new DALResult<bool>(Status.Exception, false, null, null); }
        }

		public DALResult<List<EventCategoryDTO>> GetAllEventCategory(bool isAdminCall = true, bool isMaincategory = false)
		{
            try
            {
                var result = new List<EventCategoryDTO>();
                using (var gt = new AddantEntities1())
                {
           
                        result = gt.EventCategories.Select(s => new EventCategoryDTO
                        {
                            IdEventCategory = s.IdEventCategory,
                            Name = s.Name,
                            Description = s.Description,
                            IsActive = (bool)s.IsActive,
                            BannerImgUrl = s.BannerImgUrl,
                            IdMainCategory = s.IdMainCategory
                        }).ToList();
                  
                        result = isMaincategory? result.Where(x=>x.IdMainCategory == null).ToList() : result.Where(x => x.IdMainCategory != null).ToList();
                  if (!isAdminCall)
                        result = result.Where(x => x.IsActive == true).ToList();
                }
                return new DALResult<List<EventCategoryDTO>>(Status.Found, result, null, null);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return new DALResult<List<EventCategoryDTO>>(Status.Exception, null, ex.Message.ToString(), null); }
        }

		public DALResult<EventCategoryDTO> GetAllEventCategoryById(int IdEventCategory)
		{
            try
            {
                var result = new EventCategoryDTO();
                using (var gt = new AddantEntities1())
                {
                    result = gt.EventCategories.Where(x => x.IdEventCategory == IdEventCategory).Select(s => new EventCategoryDTO
                    {
                        IdEventCategory = s.IdEventCategory,
                        Name = s.Name,
                        Description = s.Description,
                        IsActive = (bool)s.IsActive,
                         BannerImgUrl = s.BannerImgUrl,
                          IdMainCategory = s.IdMainCategory
                    }).OrderByDescending(c => c.IdEventCategory).FirstOrDefault();
                }
                return new DALResult<EventCategoryDTO>(Status.Found, result, null, null);
            }
            catch (Exception ex) { return new DALResult<EventCategoryDTO>(Status.Exception, null, ex.Message.ToString(), null); }
        }
        public DALResult<EventCategoryDTO> GetAllEventByIdMainCategory(int IdMainCategory)
        {
            try
            {
                var result = new EventCategoryDTO();
                using (var gt = new AddantEntities1())
                {
                    result = gt.EventCategories.Where(x => x.IdMainCategory == IdMainCategory).Select(s => new EventCategoryDTO
                    {
                        IdEventCategory = s.IdEventCategory,
                        Name = s.Name,
                        Description = s.Description,
                        IsActive = (bool)s.IsActive,
                        BannerImgUrl = s.BannerImgUrl,
                        IdMainCategory = s.IdMainCategory
                    }).OrderByDescending(c => c.IdEventCategory).FirstOrDefault();
                }
                return new DALResult<EventCategoryDTO>(Status.Found, result, null, null);
            }
            catch (Exception ex) { return new DALResult<EventCategoryDTO>(Status.Exception, null, ex.Message.ToString(), null); }
        }

		public DALResult<List<EventCategoryDTO>> EventCategoryByCategory(int IdMainCategory)
		{
			try
			{
				var result = new List<EventCategoryDTO>();
				using (var gt = new AddantEntities1())
				{
					result = gt.EventCategories.Where(x => x.IdMainCategory == IdMainCategory).Select(s => new EventCategoryDTO
					{
						IdEventCategory = s.IdEventCategory,
						Name = s.Name,
						Description = s.Description,
						IsActive = (bool)s.IsActive,
						BannerImgUrl = s.BannerImgUrl,
						IdMainCategory = s.IdMainCategory
					}).OrderByDescending(c => c.IdEventCategory).ToList();
				}
				return new DALResult<List<EventCategoryDTO>>(Status.Found, result, null, null);
			}
			catch (Exception ex) { return new DALResult<List<EventCategoryDTO>>(Status.Exception, null, ex.Message.ToString(), null); }
		}

		 
	}
}
