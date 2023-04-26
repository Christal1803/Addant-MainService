using AddantSDAL.DTO;
using AddantService.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AddantService;
using System.IO;
using Microsoft.PowerBI.Api.Models;
using Status = AddantService.DAL.Status;

namespace AddantSDAL.DAL
{
    public class AddantLifeRepository : IAddantLifeRepository
    {
        public DALResult<int> ComposeAddantLife(AddantLifeDTO addantLifeDTO)
        {
            try
            {
                AddantLife addantLife = new AddantLife
                {
                    CoverImgUrl = addantLifeDTO.CoverImgUrl,
                    InnerImgURl = addantLifeDTO.InnerImgURl,
                    ThumbNailInnerUrl = addantLifeDTO.ThumbNailInnerUrl,
                    IsDeleted = addantLifeDTO.IsDeleted,
                    Title = addantLifeDTO.Title,
                    IdAddantLife = addantLifeDTO.IdAddantLife,
                    Description = addantLifeDTO.Description,
                    BannerImgUrl = addantLifeDTO.BannerImgUrl,
                    IdEventCategory = addantLifeDTO.IdEventCategory,
                    CreatedDate = addantLifeDTO.CreatedDate != null ? addantLifeDTO.CreatedDate : DateTime.Now,
                    AddantLifeDetails = addantLifeDTO.addantLifeDetials?.Select(x=> new AddantLifeDetail {
                        IdAddantLife = x.IdAddantLife,
                        InnerCaption = x.InnerCaption,
                        IdAddantLifeImage = x.IdAddantLifeImage,
                        InnerImageUrl = x.InnerImageUrl,
                        IsDeleted = x.IsDeleted,
                        Deleted=false
                    }).ToList()
                };
                using (var gt = new AddantEntities1())
                {
                    AddantLife res = gt.AddantLives.Where(x=> x.IdAddantLife == addantLifeDTO.IdAddantLife).FirstOrDefault();
                    if (res != null)
                    {
                        addantLife.CoverImgUrl = string.IsNullOrEmpty(addantLife.CoverImgUrl) ? res.CoverImgUrl : addantLife.CoverImgUrl;
                        addantLife.BannerImgUrl = string.IsNullOrEmpty(addantLife.BannerImgUrl) ? res.BannerImgUrl : addantLife.BannerImgUrl;
                        gt.Entry(res).CurrentValues.SetValues(addantLife);
                        if (addantLife.AddantLifeDetails != null)
                        {
                            foreach (var item in addantLife.AddantLifeDetails)
                            {
                                AddantLifeDetail existingAddantLifeDetial = gt.AddantLifeDetails.Where(x => x.IdAddantLife == addantLifeDTO.IdAddantLife && x.IdAddantLifeImage == item.IdAddantLifeImage).FirstOrDefault();
                                AddantLifeDetail _addantLifeDetail  = new AddantLifeDetail
                                {
                                    IdAddantLife = addantLife.IdAddantLife,
                                    IdAddantLifeImage = item.IdAddantLifeImage,
                                    InnerCaption = item.InnerCaption,
                                    InnerImageUrl = item.InnerImageUrl.Contains("http")? "": item.InnerImageUrl,
                                    IsDeleted = item.IsDeleted,
                                    Deleted = false

                                };
                                if (existingAddantLifeDetial != null)
                                {
                                    _addantLifeDetail.InnerImageUrl = string.IsNullOrEmpty(_addantLifeDetail.InnerImageUrl) ? existingAddantLifeDetial.InnerImageUrl : _addantLifeDetail.InnerImageUrl;
                                    gt.Entry(existingAddantLifeDetial).CurrentValues.SetValues(_addantLifeDetail); }
                                else
                                    gt.AddantLifeDetails.Add(_addantLifeDetail);
                            }
                        }
                    }
                    else
                    {
                        gt.AddantLives.Add(addantLife);
                        gt.SaveChanges();
                    }
                    gt.SaveChanges();
                }
                return new DALResult<int>(Status.Created, addantLife.IdAddantLife , null,null);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return new DALResult<int>(Status.Exception, -1, null,null); }
        }
        public DALResult<List<AddantLifeDTO>> GetAllAddantLife(int idAddantLife = 0, bool isAdminCall = false, string searchText = "" , DateTime? startDate = null, DateTime? endDate = null)
        {
            List<AddantLifeDTO> addantLifeDTOs = new List<AddantLifeDTO>();
            try
            {
                using (var gt = new AddantEntities1())
                {
                    if (startDate != null && endDate != null)
                        addantLifeDTOs = gt.AddantLives.Where(x => x.IdAddantLife == (idAddantLife > 0 ? idAddantLife : x.IdAddantLife) && System.Data.Entity.DbFunctions.TruncateTime(x.CreatedDate) >= startDate && System.Data.Entity.DbFunctions.TruncateTime(x.CreatedDate) <= endDate).Select(s => new AddantLifeDTO
                    {
                        CoverImgUrl = s.CoverImgUrl,
                        IsDeleted = (bool)s.IsDeleted,
                        IdAddantLife = s.IdAddantLife,
                        InnerImgURl = s.InnerImgURl,
                        ThumbNailInnerUrl = s.ThumbNailInnerUrl,
                        Title = s.Title,
                        Description = s.Description,
                        BannerImgUrl = s.BannerImgUrl,
                        CreatedDate = s.CreatedDate,
                        Deleted=s.Deleted,
                            IdEventCategory = (int)s.IdEventCategory,
                            addantLifeDetials = s.AddantLifeDetails.Select(x => new AddantLifeDetialDTO
                        {
                            IdAddantLife = (int)x.IdAddantLife,
                            InnerImageUrl = x.InnerImageUrl,
                            IdAddantLifeImage = x.IdAddantLifeImage,
                            InnerCaption = x.InnerCaption,
                             
                                IsDeleted = (bool)x.IsDeleted,
                                Deleted=x.Deleted
                        }).ToList()
                    }).OrderByDescending(c => c.IdAddantLife).ToList();
                    else

                        addantLifeDTOs = gt.AddantLives.Where(x => x.IdAddantLife == (idAddantLife > 0 ? idAddantLife : x.IdAddantLife)).Select(s => new AddantLifeDTO
                        {
                            CoverImgUrl = s.CoverImgUrl,
                            IsDeleted = (bool)s.IsDeleted,
                            IdAddantLife = s.IdAddantLife,
                            InnerImgURl = s.InnerImgURl,
                            ThumbNailInnerUrl = s.ThumbNailInnerUrl,
                            Title = s.Title,
                            Description = s.Description,
                            BannerImgUrl = s.BannerImgUrl,
                            CreatedDate = s.CreatedDate,
                            IdEventCategory = (int)s.IdEventCategory,
                            Deleted = s.Deleted,

                            addantLifeDetials = s.AddantLifeDetails.Select(x => new AddantLifeDetialDTO
                            {
                                IdAddantLife = (int)x.IdAddantLife,
                                InnerImageUrl = x.InnerImageUrl,
                                IdAddantLifeImage = x.IdAddantLifeImage,
                                InnerCaption = x.InnerCaption,
                                IsDeleted = (bool)x.IsDeleted,
                                Deleted = x.Deleted

                            }).ToList()
                        }).OrderByDescending(c => c.IdAddantLife).ToList();

                    addantLifeDTOs = addantLifeDTOs.Where(a => (a.Title != null ? (a.Title.ToUpper().Contains(searchText != null ? searchText?.ToUpper() : "") || string.IsNullOrWhiteSpace(searchText)) : string.IsNullOrWhiteSpace(searchText))
                                                           || (a.Description != null ? (a.Description.ToUpper().Contains(searchText != null ? searchText?.ToUpper() : "") || string.IsNullOrWhiteSpace(searchText)) : string.IsNullOrWhiteSpace(searchText))
                              )
                              .OrderByDescending(c => c.IdAddantLife).OrderBy(d=>d.IsDeleted)
                    .ToList();
                    addantLifeDTOs = addantLifeDTOs.Where(x => x.Deleted != true).ToList();


                    if (!isAdminCall)
                        addantLifeDTOs= addantLifeDTOs.Where(x=>x.IsDeleted == false && x.Deleted != true).ToList();

                }
                return new DALResult<List<AddantLifeDTO>>(Status.Found, addantLifeDTOs, null,null);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return new DALResult<List<AddantLifeDTO>>(Status.Exception, null, null,null); }
        }
        public DALResult<List<AddantLifeDetialDTO>> GetAllInnerImage(int idAddantLife = 0)
        {
            List<AddantLifeDetialDTO> addantLifeDTOs = new List<AddantLifeDetialDTO>();
            try
            {
                using (var gt = new AddantEntities1())
                {
                    addantLifeDTOs = gt.AddantLifeDetails.Where(x => x.IdAddantLife == (idAddantLife > 0 ? idAddantLife : x.IdAddantLife) && x.IsDeleted == false).Select(x => new AddantLifeDetialDTO
                    {
                        IdAddantLife = (int)x.IdAddantLife,
                        InnerImageUrl = x.InnerImageUrl,
                        IdAddantLifeImage = x.IdAddantLifeImage,
                        InnerCaption = x.InnerCaption,
                        IsDeleted = (bool)x.IsDeleted,
                        Deleted=x.Deleted

                    }).OrderByDescending(c=>c.IdAddantLifeImage).ToList();
                }
                return new DALResult<List<AddantLifeDetialDTO>>(Status.Found, addantLifeDTOs, null, null);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return new DALResult<List<AddantLifeDetialDTO>>(Status.Exception, null, null, null); }
        }
        public DALResult<bool> DisableAddantLife(int idAddantLife = 0, bool isDeleted = false) {
            try
            {
                AddantLife addantLifeDTO = new AddantLife();
                using (var gt = new AddantEntities1())
                {
                    var exstingRes = gt.AddantLives.Where(x=>x.IdAddantLife == idAddantLife).FirstOrDefault();
                    if (exstingRes != null)
                    { 
                        addantLifeDTO = exstingRes;
                        addantLifeDTO.IsDeleted = isDeleted;
                    }
                    gt.Entry(exstingRes).CurrentValues.SetValues(addantLifeDTO);
                    gt.SaveChanges();
                    return new DALResult<bool>(Status.Deleted, true, null, null);
                }
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return new DALResult<bool>(Status.Exception, false ,null,null); }
        }
        public DALResult<bool> DisableAddantLifeDetail(int idAddantLifeDetail = 0, bool isDeleted  = false) {
            try
            {
                AddantLifeDetail addantLifeDTO = new AddantLifeDetail();
                using (var gt = new AddantEntities1())
                {
                    var existingRes = gt.AddantLifeDetails.Where(x => x.IdAddantLifeImage == idAddantLifeDetail).FirstOrDefault();
                    if (existingRes != null)
                    { addantLifeDTO = existingRes;
                        addantLifeDTO.IsDeleted = isDeleted; }
                    gt.Entry(existingRes).CurrentValues.SetValues(addantLifeDTO);
                    gt.SaveChanges();
                    return new DALResult<bool>(Status.Deleted, true, null, null);
                }
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return new DALResult<bool>(Status.Exception , false, null ,null); }
        }

        public DALResult<List<IGrouping<int, AddantLifeDTO>>> GetAllAddantLifeByCategory( bool isAdminCall = false, string category = "", string groupBy ="year")
        {
            List<AddantLifeDTO> addantLifData = new List<AddantLifeDTO>();
            List<IGrouping<int, AddantLifeDTO>> addantLifeDTOs = new List<IGrouping<int, AddantLifeDTO>>();
            try
            {
                using (var gt = new AddantEntities1())
                {
                    addantLifData = gt.AddantLives.Select(s => new AddantLifeDTO
                        {
                            CoverImgUrl = s.CoverImgUrl,
                            IsDeleted = (bool)s.IsDeleted,
                            IdAddantLife = s.IdAddantLife,
                            InnerImgURl = s.InnerImgURl,
                            ThumbNailInnerUrl = s.ThumbNailInnerUrl,
                            Title = s.Title,
                            Description = s.Description,
                            BannerImgUrl = s.BannerImgUrl,
                            CreatedDate = s.CreatedDate,
                            IdEventCategory = (int)s.IdEventCategory,
                            EventCategoryName = s.EventCategory.Name,
                            Deleted=s.Deleted,
                            addantLifeDetials = s.AddantLifeDetails.Select(x => new AddantLifeDetialDTO
                            {
                                IdAddantLife = (int)x.IdAddantLife,
                                InnerImageUrl = x.InnerImageUrl,
                                IdAddantLifeImage = x.IdAddantLifeImage,
                                InnerCaption = x.InnerCaption,
                                IsDeleted = (bool)x.IsDeleted,
                                Deleted=x.Deleted
                            }).ToList()
                        }).OrderByDescending(c => c.IdAddantLife).ToList();
                    addantLifData = addantLifData.Where(x => x.Deleted != true).ToList();


                    if (!isAdminCall)
                        addantLifData = addantLifData.Where(x => x.IsDeleted == true).ToList();
                    addantLifData = addantLifData.Where(x => x.Deleted != true).ToList();

                    if (addantLifData.Count > 0)
                    {
                        foreach (var item in addantLifData)
                        {
                            if (item.CoverImgUrl != string.Empty)
                                item.CoverImgUrl = Path.Combine((Path.Combine(System.Configuration.ConfigurationManager.AppSettings["HostedDomainName"].ToString() + "/Uploads/Image", "")), item.CoverImgUrl != null ? item.CoverImgUrl : "");
                            if (item.BannerImgUrl != string.Empty)
                                item.BannerImgUrl = Path.Combine((Path.Combine(System.Configuration.ConfigurationManager.AppSettings["HostedDomainName"].ToString() + "/Uploads/Image", "")), item.BannerImgUrl != null ? item.BannerImgUrl : "");
                            foreach (var inneritem in item.addantLifeDetials)
                            {
                                if (inneritem.InnerImageUrl != string.Empty)
                                    inneritem.InnerImageUrl = Path.Combine((Path.Combine(System.Configuration.ConfigurationManager.AppSettings["HostedDomainName"].ToString() + "/Uploads/Image", "")), inneritem.InnerImageUrl != null ? inneritem.InnerImageUrl : "");

                            }
                        }
                    }
                    switch (groupBy)
                    {
                        case "Year": {
                                addantLifeDTOs = addantLifData.Where(a => (a != null ? (a.EventCategoryName.ToUpper().Contains(category != null ? category?.ToUpper() : "") || string.IsNullOrWhiteSpace(category)) : string.IsNullOrWhiteSpace(category))  )
                               .OrderByDescending(x => x.CreatedDate.Value.Year).GroupBy(t => t.CreatedDate.Value.Year)
                         .ToList();

                            }
                            break;
                        case "Month": {
                                addantLifeDTOs = addantLifData.Where(a => (a != null ? (a.EventCategoryName.ToUpper().Contains(category != null ? category?.ToUpper() : "") || string.IsNullOrWhiteSpace(category)) : string.IsNullOrWhiteSpace(category)))
                                .OrderBy(x=>x.CreatedDate.Value.Month).GroupBy(t => t.CreatedDate.Value.Month)
                                .ToList();
                         }
                            break;
                    }



                }
                return new DALResult<List<IGrouping<int, AddantLifeDTO>>>(Status.Found, addantLifeDTOs, null, null);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return new DALResult<List<IGrouping<int, AddantLifeDTO>>>(Status.Exception, null, null, null); }
        }
        public DALResult<bool> UpdateDeletedStatus(int AddantLifeId, bool isDeleted)
        {
            try
            {
                using (var gt = new AddantEntities1())
                {
                    AddantLife res = gt.AddantLives.Where(x => x.IdAddantLife == AddantLifeId).FirstOrDefault();
                    res.Deleted = true;
                    gt.Entry(res).CurrentValues.SetValues(res);
                    gt.SaveChanges();
                    return new DALResult<bool>(Status.Deleted, true, null, null);
                }
            }
            catch (Exception ex) { return new DALResult<bool>(Status.Exception, false, null, null); }
        }
        public DALResult<bool> DetailUpdateDeletedStatus(int DetailId, bool isDeleted)
        {
            try
            {
                using (var gt = new AddantEntities1())
                {
                    AddantLifeDetail res = gt.AddantLifeDetails.Where(x => x.IdAddantLifeImage == DetailId).FirstOrDefault();
                    res.Deleted = true;
                    gt.Entry(res).CurrentValues.SetValues(res);
                    gt.SaveChanges();
                    return new DALResult<bool>(Status.Deleted, true, null, null);
                }
            }
            catch (Exception ex) { return new DALResult<bool>(Status.Exception, false, null, null); }
        }


        public DALResult<bool> EventCategoryUpdateDeletedStatus(int IdEventCategory, bool isDeleted)
        {
            try
            {
                using (var gt = new AddantEntities1())
                {
                    EventCategory res = gt.EventCategories.Where(x => x.IdEventCategory == IdEventCategory).FirstOrDefault();
                    res.Deleted = true;
                    gt.Entry(res).CurrentValues.SetValues(res);
                    gt.SaveChanges();
                    return new DALResult<bool>(Status.Deleted, true, null, null);
                }
            }
            catch (Exception ex) { return new DALResult<bool>(Status.Exception, false, null, null); }
        }
    }
}
