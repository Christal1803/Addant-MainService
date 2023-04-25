using AddantSDal;
using AddantSDAL.DTO;
using AddantService;
using AddantService.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.PeerToPeer;
using System.Text;
using System.Threading.Tasks;

namespace AddantSDAL.DAL
{
    public class EnquiryRepository : IEnquiryRepository
    {
        public DALResult<EnquiryDTO> CreateEnquiry(EnquiryDTO enquiryData)
        {
            try
            {
                using (var gt = new AddantEntities1())
                {
                    Enquiry _data = new Enquiry
                    {
                        CreatedOn = enquiryData.CreatedDate != null ? enquiryData.CreatedDate : DateTime.Now,
                        FirstName = enquiryData.FirstName,
                        LastName = enquiryData.LastName,
                        Email = enquiryData.Email,
                        IdEnquiry = enquiryData.IdEnquiry,
                        Message = enquiryData.Message,
                        Mobile = enquiryData.Mobile,
                        Status = enquiryData.Status,
                        Subject = enquiryData.Subject,
                        IsDeleted = false,
                        Deleted = false,
                        CategoryId = enquiryData.CategoryId
                    };
                    var existingEnquiry = gt.Enquiries.Where(x => x.IdEnquiry == enquiryData.IdEnquiry).FirstOrDefault();
                    if (existingEnquiry != null)
                        gt.Entry(existingEnquiry).CurrentValues.SetValues(_data);
                    else
                        gt.Enquiries.Add(_data);
                    gt.SaveChanges();
                    enquiryData.IdEnquiry = _data.IdEnquiry;
                    return new DALResult<EnquiryDTO>(Status.Created, enquiryData, null, null);
                }
            }
            catch (Exception ex) { return new DALResult<EnquiryDTO>(Status.Exception, null, ex.Message.ToString(), null); }
        }

        public DALResult<List<EnquiryDTO>> GetAllEnquiry(DateTime? StartDate = null, DateTime? EndDate = null, string searchText = "", bool isAdminCall = false, string status = "")
        {
            try
            {
                var result = new List<EnquiryDTO>();
                using (var gt = new AddantEntities1())
                {
                    if (StartDate != null && EndDate != null)
                        result = gt.Enquiries.Where(x => System.Data.Entity.DbFunctions.TruncateTime(x.CreatedOn) >= StartDate && System.Data.Entity.DbFunctions.TruncateTime(x.CreatedOn) <= EndDate
                        //&& x.Deleted==false
                        )


                            // result = gt.Enquiries.Where(x => System.Data.Entity.DbFunctions.TruncateTime(x.CreatedOn) >= StartDate && System.Data.Entity.DbFunctions.TruncateTime(x.CreatedOn) <= EndDate)
                            .Select(s => new EnquiryDTO
                            {

                                Email = s.Email,
                                FirstName = s.FirstName,
                                IdEnquiry = s.IdEnquiry,
                                LastName = s.LastName,
                                Message = s.Message,
                                Status = s.Status,
                                Subject = s.Subject,
                                Mobile = s.Mobile,
                                IsDeleted = s.IsDeleted,
                                CreatedDate = s.CreatedOn,
                                CategoryId = s.CategoryId,
                                Deleted = s.Deleted,

                                CategoryName = s.EnquiryCategory == null ? "Business" : s.EnquiryCategory.CategoryName
                            }).ToList();
                    else
                        result = gt.Enquiries.Select(s => new EnquiryDTO
                        {
                            Email = s.Email,
                            FirstName = s.FirstName,
                            IdEnquiry = s.IdEnquiry,
                            LastName = s.LastName,
                            Message = s.Message,
                            Status = s.Status,
                            Subject = s.Subject,
                            Mobile = s.Mobile,
                            IsDeleted = s.IsDeleted,
                            CreatedDate = s.CreatedOn,
                            CategoryId = s.CategoryId,
                            Deleted = s.Deleted,
                            CategoryName = s.EnquiryCategory == null ? "Business" : s.EnquiryCategory.CategoryName.Trim()
                            //  CategoryName = gt.EnquiryCategories.Where(c => c.CategoryId == s.CategoryId).Select(x=>x.CategoryName).FirstOrDefault()
                        }).ToList();
                    result = result.Where(a => (a.FirstName != null ? (a.FirstName.ToUpper().Contains(searchText != null ? searchText?.ToUpper() : "") || string.IsNullOrWhiteSpace(searchText)) : string.IsNullOrWhiteSpace(searchText))
                                    || (a.LastName != null ? (a.LastName.ToUpper().Contains(searchText != null ? searchText?.ToUpper() : "") || string.IsNullOrWhiteSpace(searchText)) : string.IsNullOrWhiteSpace(searchText))
                                    || (a.Mobile != null ? (a.Mobile.Contains(searchText != null ? searchText?.ToUpper() : "") || string.IsNullOrWhiteSpace(searchText)) : string.IsNullOrWhiteSpace(searchText))
                                    || (a.Subject != null ? (a.Subject.ToUpper().Contains(searchText != null ? searchText?.ToUpper() : "") || string.IsNullOrWhiteSpace(searchText)) : string.IsNullOrWhiteSpace(searchText))
                                    || (a.Email != null ? (a.Email.ToUpper().Contains(searchText != null ? searchText?.ToUpper() : "") || string.IsNullOrWhiteSpace(searchText)) : string.IsNullOrWhiteSpace(searchText))
                                    )
                                   .OrderByDescending(c => c.IdEnquiry).OrderBy(x => x.IsDeleted)
                          .ToList();
                    if (!string.IsNullOrWhiteSpace(status))
                        result = result.Where(x => (x.Status != null ? (x.Status.ToUpper().Contains(status != null ? status?.ToUpper() : "") || string.IsNullOrWhiteSpace(status)) : string.IsNullOrWhiteSpace(status))).ToList();
                    result = result.Where(x => x.Deleted != true).ToList();

                    if (!isAdminCall)
                        result = result.Where(x => x.IsDeleted == false).ToList();
                    result = result.Where(x => x.Deleted != true).ToList();

                }
                return new DALResult<List<EnquiryDTO>>(Status.Found, result, null, null);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return new DALResult<List<EnquiryDTO>>(Status.Exception, null, ex.Message.ToString(), null); }
        }

        public DALResult<List<EnquiryDTO>> GetAllEnquiryById(int IdEnquiry)
        {
            try
            {
                var result = new List<EnquiryDTO>();
                using (var gt = new AddantEntities1())
                {
                    result = gt.Enquiries.Where(x => x.IdEnquiry == IdEnquiry
                    //  && x.Deleted==false
                    ).Select(s => new EnquiryDTO
                    {
                        Email = s.Email,
                        FirstName = s.FirstName,
                        IdEnquiry = s.IdEnquiry,
                        LastName = s.LastName,
                        Message = s.Message,
                        Status = s.Status,
                        Subject = string.IsNullOrEmpty(s.Subject) ? "" : s.Subject,
                        Mobile = s.Mobile,
                        IsDeleted = s.IsDeleted,
                        CreatedDate = s.CreatedOn,
                        CategoryId = s.CategoryId,
                        CategoryName = s.EnquiryCategory.CategoryName,
                        Deleted = s.Deleted

                    }).OrderByDescending(c => c.IdEnquiry).ToList();
                }
                return new DALResult<List<EnquiryDTO>>(Status.Found, result, null, null);
            }
            catch (Exception ex) { return new DALResult<List<EnquiryDTO>>(Status.Exception, null, ex.Message.ToString(), null); }
        }
        public DALResult<bool> Delete(int IdEnquiry, bool isDelete)
        {
            try
            {
                using (var gt = new AddantEntities1())
                {
                    Enquiry res = gt.Enquiries.Where(x => x.IdEnquiry == IdEnquiry).FirstOrDefault();
                    res.IsDeleted = isDelete;
                    gt.Entry(res).CurrentValues.SetValues(res);
                    gt.SaveChanges();
                    return new DALResult<bool>(Status.Deleted, true, null, null);
                }
            }
            catch (Exception ex) { return new DALResult<bool>(Status.Exception, false, null, null); }
        }

        //Getting Enquiries in a particular Category
        ///
        public DALResult<List<EnquiryDTO>> GetAllEnquiriesByCategory(int CategoryId)
        {
            try
            {
                var result = new List<EnquiryDTO>();
                using (var gt = new AddantEntities1())
                {
                    result = gt.Enquiries.Where(x => x.CategoryId == CategoryId
                    //  && x.Deleted==false
                    ).Select(s => new EnquiryDTO
                    {
                        Email = s.Email,
                        FirstName = s.FirstName,
                        IdEnquiry = s.IdEnquiry,
                        LastName = s.LastName,
                        Message = s.Message,
                        Status = s.Status,
                        Subject = string.IsNullOrEmpty(s.Subject) ? "" : s.Subject,
                        Mobile = s.Mobile,
                        IsDeleted = s.IsDeleted,
                        CreatedDate = s.CreatedOn,
                        CategoryId = s.CategoryId,
                        CategoryName = s.EnquiryCategory.CategoryName,
                        Deleted = s.Deleted

                    }).OrderByDescending(c => c.IdEnquiry).ToList();
                    result = result.Where(x => x.Deleted != true).ToList();

                }
                return new DALResult<List<EnquiryDTO>>(Status.Found, result, null, null);
            }
            catch (Exception ex) { return new DALResult<List<EnquiryDTO>>(Status.Exception, null, ex.Message.ToString(), null); }
        }

        ///Enquiry Category DTO 
        ///GET Method only
        ///
        public DALResult<List<EnquiryCategoryDTO>> GetAllEnquiryCategories()
        {
            try
            {
                var result = new List<EnquiryCategoryDTO>();
                using (var gt = new AddantEntities1())
                {
                    result = gt.EnquiryCategories.Where(x => x.isActive == true).Select(s => new EnquiryCategoryDTO
                    {
                        CategoryId = s.CategoryId,
                        CategoryName = s.CategoryName.Trim(),
                        IsActive = s.isActive

                    }).OrderByDescending(c => c.CategoryId).ToList();
                }
                return new DALResult<List<EnquiryCategoryDTO>>(Status.Found, result, null, null);
            }
            catch (Exception ex) { return new DALResult<List<EnquiryCategoryDTO>>(Status.Exception, null, ex.Message.ToString(), null); }
        }

        ///  
        /// Delete Enquiry  
        ///  setting up a delete flag 
        ///  
        public DALResult<bool> Delete(int IdEnquiry)
        {
            try
            {
                using (var gt = new AddantEntities1())
                {
                    Enquiry res = gt.Enquiries.Where(x => x.IdEnquiry == IdEnquiry).FirstOrDefault();
                    //  res.Deleted = true;
                    gt.Entry(res).CurrentValues.SetValues(res);
                    gt.SaveChanges();
                    return new DALResult<bool>(Status.Deleted, true, null, null);
                }
            }
            catch (Exception ex) { return new DALResult<bool>(Status.Exception, false, null, null); }
        }

        /////
        ///GetEnquiries category = business and  partners <summary>
        /// GetEnquiries category = business and  partners
        /// 
        /// </summary>
        /// <returns></returns>

        public DALResult<List<EnquiryDTO>> GetAllEnquiries()
        {
            try
            {
                var result = new List<EnquiryDTO>();
                using (var gt = new AddantEntities1())
                {
                    result = gt.Enquiries.Where(s => s.CategoryId == 1 || s.CategoryId == 2).Select(s => new EnquiryDTO
                    {
                        Email = s.Email,
                        FirstName = s.FirstName,
                        IdEnquiry = s.IdEnquiry,
                        LastName = s.LastName,
                        Message = s.Message,
                        Status = s.Status,
                        Subject = s.Subject,
                        Mobile = s.Mobile,
                        IsDeleted = s.IsDeleted,
                        CreatedDate = s.CreatedOn,
                        CategoryId = s.CategoryId,
                        Deleted = s.Deleted,
                        CategoryName = s.EnquiryCategory == null ? "Business" : s.EnquiryCategory.CategoryName.Trim()
                        //  CategoryName = gt.EnquiryCategories.Where(c => c.CategoryId == s.CategoryId).Select(x=>x.CategoryName).FirstOrDefault()
                    }).ToList();
                    result = result.OrderByDescending(c => c.IdEnquiry).OrderBy(x => x.IsDeleted)
                    .ToList();

                    result = result.Where(x => x.IsDeleted == false && x.Status?.Trim() != "Closed" && x.Status?.Trim() != "Disqualified").ToList();
                    result = result.Where(x => x.Deleted != true && x.Status?.Trim() != "Closed" && x.Status?.Trim() != "Disqualified").ToList();
                }
                return new DALResult<List<EnquiryDTO>>(Status.Found, result, null, null);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return new DALResult<List<EnquiryDTO>>(Status.Exception, null, ex.Message.ToString(), null); }
        }


        public DALResult<bool> UpdateDeletedStatus(int IdEnquiry, bool isDeleted)
        {
            try
            {
                using (var gt = new AddantEntities1())
                {
                    Enquiry res = gt.Enquiries.Where(x => x.IdEnquiry == IdEnquiry).FirstOrDefault();
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
