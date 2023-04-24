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
    public class EmailTemplateRepository : IEmailTemplateRepository
    {
        public DALResult<EmailTemplateDTO> CreateEmailTemplate(EmailTemplateDTO emailTemplateDTO)
        {
            try
            {
                using (var gt = new AddantEntities1())
                {
                    EmailTemplate _data = new EmailTemplate
                    {
                        CreatedDate = emailTemplateDTO.CreatedDate != null ? emailTemplateDTO.CreatedDate : DateTime.Now,
                        Body = emailTemplateDTO.Body,
                        HeaderImageUrl = emailTemplateDTO.HeaderImageUrl,
                        IdEmailTemplate = emailTemplateDTO.IdEmailTemplate,
                        IdTemplateType = emailTemplateDTO.IdTemplateType,
                        IsDeleted = emailTemplateDTO.IsActive,
                        Deleted=false
                    };
                    var existingEnquiry = gt.EmailTemplates.Where(x => x.IdEmailTemplate == emailTemplateDTO.IdEmailTemplate).FirstOrDefault();
                    if (existingEnquiry != null)
                        gt.Entry(existingEnquiry).CurrentValues.SetValues(_data);
                    else
                        gt.EmailTemplates.Add(_data);
                    gt.SaveChanges();
                    emailTemplateDTO.IdEmailTemplate = _data.IdEmailTemplate;
                    return new DALResult<EmailTemplateDTO>(Status.Created, emailTemplateDTO, null, null);
                }
            }
            catch (Exception ex) { return new DALResult<EmailTemplateDTO>(Status.Exception, null, ex.Message.ToString(), null); }
        }

        public DALResult<bool> Delete(int emailTemplateById, bool isActive)
        {
            try
            {
                using (var gt = new AddantEntities1())
                {
                    EmailTemplate res = gt.EmailTemplates.Where(x => x.IdEmailTemplate == emailTemplateById).FirstOrDefault();
                    List<EmailTemplate> otherEmailWithSameType = gt.EmailTemplates.Where(t => t.IdTemplateType == res.IdTemplateType)
                        .ToList();
                    if(isActive)
                    {
                        otherEmailWithSameType.ForEach(y =>
                      {
                          y.IsDeleted = !isActive;
                      });
                        foreach (var item in otherEmailWithSameType)
                        {
                            gt.Entry(item).CurrentValues.SetValues(item);
                        }
                    }
                    res.IsDeleted = isActive;
                    gt.Entry(res).CurrentValues.SetValues(res);
                    gt.SaveChanges();
                    //List<EmailTemplate> afterEmailUpdate = gt.EmailTemplates.Where(t => t.IdTemplateType == res.IdTemplateType && t.IsDeleted == true).ToList();
                    //if (afterEmailUpdate != null)
                    //{
                    //    if (afterEmailUpdate.Count <= 0)
                    //    {
                    //        EmailTemplate emailTemplate = gt.EmailTemplates.OrderByDescending(x => x.IdEmailTemplate).ToList().FirstOrDefault();
                    //        emailTemplate.IsDeleted = true;
                    //        gt.Entry(emailTemplate).CurrentValues.SetValues(emailTemplate);
                    //    }
                    //}
                    //gt.SaveChanges();
                    return new DALResult<bool>(Status.Deleted, true, null, null);
                }
            }
            catch (Exception ex) { return new DALResult<bool>(Status.Exception, false, null, null); }

        }

        public DALResult<List<EmailTemplateDTO>> GetAllEmailTemplate(DateTime? startDate = null, DateTime? endDate = null, string searchText = "", bool isAdminCall = false)
        {
            try
            {
                var result = new List<EmailTemplateDTO>();
                using (var gt = new AddantEntities1())
                {
                    if (startDate != null && endDate != null)
                        result = gt.EmailTemplates.Where(x => System.Data.Entity.DbFunctions.TruncateTime(x.CreatedDate) >= startDate && System.Data.Entity.DbFunctions.TruncateTime(x.CreatedDate) <= endDate)
                            .Select(s => new EmailTemplateDTO
                            {
                                CreatedDate = s.CreatedDate ,
                                Body = s.Body,
                                HeaderImageUrl = s.HeaderImageUrl,
                                IdEmailTemplate = s.IdEmailTemplate,
                                IdTemplateType = s.IdTemplateType,
                                IsActive = s.IsDeleted,
                                TemplateName = s.TemplateType.Name,
                                Deleted=s.Deleted
                            }).ToList();
                    else
                        result = gt.EmailTemplates.Select(s => new EmailTemplateDTO
                        {
                            CreatedDate = s.CreatedDate,
                            Body = s.Body,
                            HeaderImageUrl = s.HeaderImageUrl,
                            IdEmailTemplate = s.IdEmailTemplate,
                            IdTemplateType = s.IdTemplateType,
                            IsActive = s.IsDeleted,
                            TemplateName = s.TemplateType.Name,
                            Deleted = s.Deleted

                        }).ToList();
                    result = result.Where(a => (a.Body != null ? (a.Body.ToUpper().Contains(searchText != null ? searchText?.ToUpper() : "") || string.IsNullOrWhiteSpace(searchText)) : string.IsNullOrWhiteSpace(searchText))
                                  || (a.TemplateName != null ? (a.TemplateName.ToUpper().Contains(searchText != null ? searchText?.ToUpper() : "") || string.IsNullOrWhiteSpace(searchText)) : string.IsNullOrWhiteSpace(searchText))
                       )
                                   .OrderByDescending(c => c.IdEmailTemplate).OrderBy(x => x.IsActive)
                          .ToList();
                    result = result.Where(x => x.Deleted != true).ToList();

                    if (!isAdminCall)
						result = result.Where(x => x.IsActive == true).ToList();
				}
                return new DALResult<List<EmailTemplateDTO>>(Status.Found, result, null, null);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return new DALResult<List<EmailTemplateDTO>>(Status.Exception, null, ex.Message.ToString(), null); }

        }

        public DALResult<EmailTemplateDTO> GetAllEmailTemplateById(int emailTemplateById, string templateType="")
        {
            try
            {
                var result = new EmailTemplateDTO();
                using (var gt = new AddantEntities1())
                {
                    result = gt.EmailTemplates.Where(x => x.IdEmailTemplate == emailTemplateById).Select(s => new EmailTemplateDTO
                    {
                        CreatedDate = s.CreatedDate,
                        Body = s.Body,
                        HeaderImageUrl = s.HeaderImageUrl,
                        IdEmailTemplate = s.IdEmailTemplate,
                        IdTemplateType = s.IdTemplateType,
                        IsActive = s.IsDeleted,
                        TemplateName = s.TemplateType.Name,
                        Deleted=s.Deleted
                    }).OrderByDescending(c => c.IdEmailTemplate).FirstOrDefault();
                    if (!string.IsNullOrEmpty(templateType))
                    {
                        result = gt.EmailTemplates.Where(x => x.TemplateType.Name == templateType && x.IsDeleted == true).Select(s => new EmailTemplateDTO
                        {
                            CreatedDate = s.CreatedDate,
                            Body = s.Body,
                            HeaderImageUrl = s.HeaderImageUrl,
                            IdEmailTemplate = s.IdEmailTemplate,
                            IdTemplateType = s.IdTemplateType,
                            IsActive = s.IsDeleted,
                            TemplateName = s.TemplateType.Name,
                            Deleted = s.Deleted

                        }).OrderByDescending(c => c.IdEmailTemplate).FirstOrDefault();
                    }
  
                }
                return new DALResult<EmailTemplateDTO>(Status.Found, result, null, null);
            }
            catch (Exception ex) { return new DALResult<EmailTemplateDTO>(Status.Exception, null, ex.Message.ToString(), null); }

        }
        public DALResult<List<TemplateTypeDTO>> GetTemplatetype()
        {
            try
            {
                var result = new List<TemplateTypeDTO>();
                using (var gt = new AddantEntities1())
                {
                        result = gt.TemplateTypes
                            .Select(s => new TemplateTypeDTO
                            {
								IdTemplateType = s.IdTemplateType,
								IsDeleted = s.IsDeleted,
								Description = s.Description,
								Name = s.Name,

							}).ToList();
                }
                return new DALResult<List<TemplateTypeDTO>>(Status.Found, result, null, null);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return new DALResult<List<TemplateTypeDTO>>(Status.Exception, null, ex.Message.ToString(), null); }

        }
        public DALResult<bool> UpdateDeletedStatus(int IdEmailTemplate, bool isDeleted)
        {
            try
            {
                using (var gt = new AddantEntities1())
                {
                    EmailTemplate res = gt.EmailTemplates.Where(x => x.IdEmailTemplate == IdEmailTemplate).FirstOrDefault();
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
