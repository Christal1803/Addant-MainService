using AddantSDAL.DTO;
using AddantService;
using AddantService.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AddantSDAL.DAL
{
   public class EmbedCodeRepository: IEmbedCodeRepository
    {
     

        public DALResult<List<EmbededCodeDTO>> GetEmbededCodeById(int idEmbedCode)
        {
            try
            {
                var result = new List<EmbededCodeDTO>();
                using (var gt = new AddantEntities1())
                {
                    result = gt.EmbedCodes.Where(x => x.IdEmbedCode == idEmbedCode).Select(s => new EmbededCodeDTO
                    {
                        IdEmbedCode = s.IdEmbedCode,
                        Code = s.Code,
                        IsDeleted = s.IsDeleted,
                        CreatedDate = s.CreatedDate,
                        IdCodeType = s.IdEmbedCodeType,
                        Deleted=s.Deleted
                    }).OrderByDescending(c => c.IdEmbedCode).ToList();
                }
                return new DALResult<List<EmbededCodeDTO>>(Status.Found, result, null, null);
            }
            catch (Exception ex) { return new DALResult<List<EmbededCodeDTO>>(Status.Exception, null, ex.Message.ToString(), null); }
        }
        public DALResult<bool> Delete(int idEmbedCode, bool isDelete)
        {
            try
            {
                using (var gt = new AddantEntities1())
                {
                    EmbedCode res = gt.EmbedCodes.Where(x => x.IdEmbedCode == idEmbedCode).FirstOrDefault();
                    res.IsDeleted = isDelete;
                    gt.Entry(res).CurrentValues.SetValues(res);
                    gt.SaveChanges();
                    return new DALResult<bool>(Status.Deleted, true, null, null);
                }
            }
            catch (Exception ex) { return new DALResult<bool>(Status.Exception, false, null, null); }
        }

        public DALResult<EmbededCodeDTO> CreateEmbededCode(EmbededCodeDTO embededCodeDTOData)
        {
            try
            {
                using (var gt = new AddantEntities1())
                {
                    EmbedCode _data = new EmbedCode
                    {
                        IdEmbedCode = embededCodeDTOData.IdEmbedCode,
                        Code = embededCodeDTOData.Code,
                        IsDeleted = embededCodeDTOData.IsDeleted,
                        CreatedDate = embededCodeDTOData.CreatedDate,
                        IdEmbedCodeType = embededCodeDTOData.IdCodeType,
                        Deleted=false
                    };
                    var existingData = gt.EmbedCodes.Where(x => x.IdEmbedCode == embededCodeDTOData.IdEmbedCode).FirstOrDefault();
                    if (_data.IdEmbedCode == 0)
                    { gt.EmbedCodes.Add(_data); }
                    else
                    { if(existingData != null)
                        gt.Entry(existingData).CurrentValues.SetValues(_data); }
                        
                    gt.SaveChanges();
                    embededCodeDTOData.IdEmbedCode = _data.IdEmbedCode;
                    return new DALResult<EmbededCodeDTO>(Status.Created, embededCodeDTOData, null, null);
                }
            }
            catch (Exception ex) { return new DALResult<EmbededCodeDTO>(Status.Exception, null, ex.Message.ToString(), null); }
        }

        public DALResult<List<EmbededCodeDTO>> GetAllEmbededCode(DateTime? StartDate = null, DateTime? EndDate = null, bool isAdminCall = false)
        {
            try
            {
                var result = new List<EmbededCodeDTO>();
                using (var gt = new AddantEntities1())
                {
                    if (StartDate != null && EndDate != null)
                        result = gt.EmbedCodes.Where(x => System.Data.Entity.DbFunctions.TruncateTime(x.CreatedDate) >= StartDate && System.Data.Entity.DbFunctions.TruncateTime(x.CreatedDate) <= EndDate)
                            .Select(s => new EmbededCodeDTO
                            {
                                IdEmbedCode = s.IdEmbedCode,
                                Code = s.Code,
                                IsDeleted = s.IsDeleted,
                                CreatedDate = s.CreatedDate,
                                IdCodeType = s.EmbedCodeType.IdCodeType,
                                Deleted=s.Deleted
                            }).ToList();
                    else
                        result = gt.EmbedCodes.Select(s => new EmbededCodeDTO
                        {
                            IdEmbedCode = s.IdEmbedCode,
                            Code = s.Code,
                            IsDeleted = s.IsDeleted,
                            CreatedDate = s.CreatedDate,
                            IdCodeType = s.EmbedCodeType.IdCodeType,
                            Deleted=s.Deleted
                        }).ToList();
                    if (!isAdminCall)
                        result = result.Where(x => x.IsDeleted != true).ToList();
                }
                return new DALResult<List<EmbededCodeDTO>>(Status.Found, result, null, null);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return new DALResult<List<EmbededCodeDTO>>(Status.Exception, null, ex.Message.ToString(), null); }

        }

        public DALResult<bool> UpdateDeletedStatus(int IdEmbedCode, bool isDeleted)
        {
            try
            {
                using (var gt = new AddantEntities1())
                {
                    EmbedCode res = gt.EmbedCodes.Where(x => x.IdEmbedCode == IdEmbedCode).FirstOrDefault();
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
