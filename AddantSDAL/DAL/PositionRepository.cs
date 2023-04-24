using AddantSDAL.DTO;
using AddantService;
using AddantService.DAL;
using System;
using System.Activities.Statements;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddantSDAL.DAL
{
    public class PositionRepository : IPositionRepository
    {
        public enum ApplicationStatus { Processing, Shortlisted, Waiting, Hired, Rejected }
        public DALResult<List<PositionDTO>> GetAllPostion(bool IsAdminCall = false, DateTime? startDate = null, DateTime? endDate = null, string SearchText = "")
        {
            try
            {
                var res = new List<PositionDTO>();
                using (var gt = new AddantEntities1())
                {
                    if (startDate != null && endDate != null)
                        res = gt.Positions.Where(x => System.Data.Entity.DbFunctions.TruncateTime(x.CreatedDate) >= startDate && System.Data.Entity.DbFunctions.TruncateTime(x.CreatedDate) <= endDate
                        // && x.Deleted == false
                        ).Select(x => new PositionDTO
                        {
                            CreatedDate = (DateTime)x.CreatedDate,
                            IsDeleted = (bool)x.IsDeleted,
                            IdPosition = x.IdPosition,
                            Name = x.Name,
                            //
                            JobId = x.JobId,
                            JobStatus = (JobStatusEnum?)x.Status,
                            Experience = x.Experience,
                            ClosingDate = x.ClosingDate,
                            Location = x.Location,
                            ReportsTo = x.ReportsTo,
                            AboutCompany = x.AboutCompany,
                            JobOverview = x.JobOverview,
                            KeyResponsibility = x.KeyResponsibility,
                            Qualification = x.Qualification,
                            Deleted=x.Deleted,
                            positionDetailDTOs = x.PositionDetails.Select(p => new PositionDetailDTO
                            {
                                CreatedDate = p.CreatedDate,
                                IdPosition = p.IdPosition,
                                IsDeleted = (bool)p.IsDeleted,
                                IdPositionDetail = p.IdPositionDetail,
                                SubHeader = p.SubHeader,
                                SubHeaderContent = p.SubHeaderContent,
                                Deleted=p.Deleted
                            }).ToList()
                        }).ToList();
                    else
                        res = gt.Positions
                            //  .Where(x.Deleted == false)
                            .Select(x => new PositionDTO
                            {
                                CreatedDate = (DateTime)x.CreatedDate,
                                IsDeleted = (bool)x.IsDeleted,
                                IdPosition = x.IdPosition,
                                Name = x.Name,
                                //
                                JobId = x.JobId,
                                JobStatus = (JobStatusEnum?)x.Status,
                                Experience = x.Experience,
                                ClosingDate = x.ClosingDate,
                                Location = x.Location,
                                ReportsTo = x.ReportsTo,
                                AboutCompany = x.AboutCompany,
                                JobOverview = x.JobOverview,
                                KeyResponsibility = x.KeyResponsibility,
                                Qualification = x.Qualification,
                                Deleted = x.Deleted,

                                positionDetailDTOs = x.PositionDetails.Select(p => new PositionDetailDTO
                                {
                                    CreatedDate = p.CreatedDate,
                                    IdPosition = p.IdPosition,
                                    IsDeleted = (bool)p.IsDeleted,
                                    IdPositionDetail = p.IdPositionDetail,
                                    SubHeader = p.SubHeader,
                                    SubHeaderContent = p.SubHeaderContent,
                                    Deleted=p.Deleted
                                }).ToList()
                            }).ToList();

                    //if (startDate != null && endDate != null)
                    //    res = res.Where(x => System.Data.Entity.DbFunctions.TruncateTime(x.CreatedDate) >= startDate && System.Data.Entity.DbFunctions.TruncateTime(x.CreatedDate) <= endDate).ToList();
                    res = res.Where(a => (a.Name != null ? (a.Name.ToUpper().Contains(SearchText != null ? SearchText.ToUpper() : "") || string.IsNullOrWhiteSpace(SearchText)) : string.IsNullOrWhiteSpace(SearchText))
                    || (a.IdPosition > 0 ? (a.IdPosition.ToString().Contains(SearchText != null ? SearchText : "") || string.IsNullOrWhiteSpace(SearchText)) : string.IsNullOrWhiteSpace(SearchText))
                    )
                        .OrderByDescending(c => c.IdPosition).ToList();
                  
                    res = res.Where(x => x.Deleted!= true && x.Name.ToUpper() != "OTHERS").ToList();

                    if (!IsAdminCall)
                                        
                        res = res.Where(x => x.JobStatus == JobStatusEnum.Active && x.Name.ToUpper() != "OTHERS"&&x.Deleted!=true).ToList();
                }

                return new DALResult<List<PositionDTO>>(Status.Found, res, null, null);
            }
            catch (Exception ex) { return new DALResult<List<PositionDTO>>(Status.Exception, null, null, null); }
        }
        public DALResult<PositionDTO> GetPositionById(int positionId)
        {
            try
            {
                var res = new PositionDTO();
                using (var gt = new AddantEntities1())
                {
                    res = gt.Positions.Where(x => x.IdPosition == positionId
                    // && x.Deleted == false
                    ).Select(cnv => new PositionDTO
                    {
                        JobStatus = (JobStatusEnum?)cnv.Status,
                        IdPosition = cnv.IdPosition,
                        CreatedDate = (DateTime)cnv.CreatedDate,
                        IsDeleted = (bool)cnv.IsDeleted,
                        Name = cnv.Name,
                        JobId = cnv.JobId,
                        Experience = cnv.Experience,
                        ClosingDate = cnv.ClosingDate,
                        Location = cnv.Location,
                        ReportsTo = cnv.ReportsTo,
                        AboutCompany = cnv.AboutCompany,
                        JobOverview = cnv.JobOverview,
                        KeyResponsibility = cnv.KeyResponsibility,
                        Qualification = cnv.Qualification,
                        Deleted=cnv.Deleted,
                        positionDetailDTOs = cnv.PositionDetails.Select(p => new PositionDetailDTO
                        {
                            CreatedDate = p.CreatedDate,
                            SubHeader = p.SubHeader,
                            SubHeaderContent = p.SubHeaderContent,
                            IsDeleted = (bool)p.IsDeleted,
                            IdPosition = p.IdPosition,
                            IdPositionDetail = p.IdPositionDetail,
                            Deleted=p.Deleted
                        }).ToList()
                    }).FirstOrDefault();
                    return new DALResult<PositionDTO>(Status.Found, res, null, null);
                }
            }
            catch (Exception ex) { return new DALResult<PositionDTO>(Status.Exception, null, null, null); }
        }
        /// <summary>   
        /// Add/Edit position 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public DALResult<PositionDTO> ComposePosition(PositionDTO data)
        {
            var existingPosition = new Position();
            try
            {
                using (var gt = new AddantEntities1())
                {
                    Position _data = new Position
                    {
                        Name = data.Name,
                        IsDeleted = data.IsDeleted,
                        CreatedDate = data.CreatedDate != null ? data.CreatedDate : DateTime.Now,
                        IdPosition = data.IdPosition,
                        JobId = data.JobId,
                        Status = (int)data.JobStatus,
                        Experience = data.Experience,
                        ClosingDate = data.ClosingDate,
                        Location = data.Location,
                        ReportsTo = data.ReportsTo,
                        AboutCompany = data.AboutCompany,
                        JobOverview = data.JobOverview,
                        KeyResponsibility = data.KeyResponsibility,
                        Qualification = data.Qualification,
                        Deleted=false,
                        //IdPosition = data.IdPosition,
                        PositionDetails = data.positionDetailDTOs?.Select(x => new PositionDetail
                        {
                            CreatedDate = x.CreatedDate,
                            IdPosition = x.IdPosition,
                            IsDeleted = x.IsDeleted,
                            IdPositionDetail = x.IdPositionDetail,
                            SubHeader = x.SubHeader,
                            SubHeaderContent = x.SubHeaderContent,
                                    Deleted=false

                        }).ToList()
                    };

                    existingPosition = gt.Positions.Where(x => x.IdPosition == data.IdPosition).FirstOrDefault();
                    if (existingPosition != null && existingPosition.IdPosition > 0)
                    {
                        gt.Entry(existingPosition).CurrentValues.SetValues(_data);
                        if (data.positionDetailDTOs?.Count > 0)
                        {
                            foreach (var item in data.positionDetailDTOs)
                            {
                                PositionDetail existingPostionDetail = gt.PositionDetails.Where(x => x.IdPosition == data.IdPosition && x.IdPositionDetail == item.IdPositionDetail).FirstOrDefault();
                                PositionDetail positionDetail = new PositionDetail
                                {
                                    CreatedDate = DateTime.Now,
                                    IdPosition = data.IdPosition,
                                    IdPositionDetail = item.IdPositionDetail,
                                    SubHeader = item.SubHeader,
                                    SubHeaderContent = item.SubHeaderContent,
                                    IsDeleted = item.IsDeleted,
                                    Deleted=false
                                };
                                if (existingPostionDetail != null)
                                    gt.Entry(existingPostionDetail).CurrentValues.SetValues(positionDetail);
                                else
                                {
                                    gt.PositionDetails.Add(positionDetail);
                                    gt.SaveChanges();
                                }
                            }
                        }
                    }
                    else
                    {
                        _data.CreatedDate = DateTime.UtcNow;
                        gt.Positions.Add(_data);
                        gt.SaveChanges();
                    }
                    data.IdPosition = _data.IdPosition;
                    gt.SaveChanges();

                }
                return new DALResult<PositionDTO>(Status.Created, data, null, null);
            }
            catch (Exception ex) { return new DALResult<PositionDTO>(Status.Exception, null, null, null); }
        }
        public DALResult<bool> DeletePosition(int positionId, bool isDeleted = false)
        {
            try
            {
                using (var gt = new AddantEntities1())
                {
                    Position existingPosition = gt.Positions.Where(x => x.IdPosition == positionId).FirstOrDefault();
                    if (existingPosition != null)
                    {
                        existingPosition.IsDeleted = isDeleted;
                        if (existingPosition.PositionDetails.Count > 0)
                        {
                            foreach (var positionDetail in existingPosition.PositionDetails)
                            {
                                positionDetail.IsDeleted = isDeleted;
                                var existingPositionDetail = gt.PositionDetails.Where(ep => ep.IdPosition == positionId && ep.IdPositionDetail == positionDetail.IdPositionDetail).FirstOrDefault();
                                if (existingPositionDetail != null)
                                    gt.Entry(existingPositionDetail).CurrentValues.SetValues(positionDetail);
                            }
                        }
                        gt.Entry(existingPosition).CurrentValues.SetValues(existingPosition);
                    }
                    gt.SaveChanges();
                }
                return new DALResult<bool>(Status.Created, true, null, null);
            }
            catch (Exception ex) { return new DALResult<bool>(Status.Exception, false, null, null); }
        }

        /// <summary>
        /// 
        /// </summary>Delete Position perrmanently from UI 
        /// <param name="positionId"></param>
        ///  
        /// <returns></returns>
        public DALResult<bool> DeletePosition(int positionId)
        {
            try
            {
                using (var gt = new AddantEntities1())
                {
                    Position existingPosition = gt.Positions.Where(x => x.IdPosition == positionId).FirstOrDefault();
                    if (existingPosition != null)
                    {
                        // existingPosition.Deleted = true;
                        if (existingPosition.PositionDetails.Count > 0)
                        {
                            foreach (var positionDetail in existingPosition.PositionDetails)
                            {
                                //  positionDetail.Deleted = true;
                                var existingPositionDetail = gt.PositionDetails.Where(ep => ep.IdPosition == positionId && ep.IdPositionDetail == positionDetail.IdPositionDetail).FirstOrDefault();
                                if (existingPositionDetail != null)
                                    gt.Entry(existingPositionDetail).CurrentValues.SetValues(positionDetail);
                            }
                        }
                        gt.Entry(existingPosition).CurrentValues.SetValues(existingPosition);
                    }
                    gt.SaveChanges();
                }
                return new DALResult<bool>(Status.Created, true, null, null);
            }
            catch (Exception ex) { return new DALResult<bool>(Status.Exception, false, null, null); }
        }


        public DALResult<List<ApplicationBucketDTO>> ApplicationBucket()
        {
            var res = new List<ApplicationBucketDTO>();
            try
            {
                using (var gt = new AddantEntities1())
                {
                    var result = gt.Candidates.Where(t => t.Deleted != true).ToList();
                    if (result != null && result?.Count > 0)
                    {
                        var positionData = gt.Positions.Where(t => t.Deleted != true).ToList();
                        if (positionData != null && positionData?.Count > 0)
                        {
                            foreach (var position in positionData)
                            {
                                var reqReceived = result.Where(t => t.IdPosition == position.IdPosition).ToList();
                                reqReceived = result.Where(t => t.Deleted !=true).ToList();
                                if (reqReceived != null && reqReceived?.Count > 0)
                                {
                                    var shortListedCandidates = reqReceived?.Where(t => t.Deleted!=true&&t.Status == ApplicationStatus.Shortlisted.ToString()).Count() ?? 0;

                                    var newCandidates = reqReceived?.Where(t => t.Deleted!=true&&t.Status == null || t.Status == string.Empty).Count() ?? 0;

                                    var underReviewCandidates = reqReceived?.Where(t => t.Deleted != true && t.Status == ApplicationStatus.Processing.ToString()).Count() ?? 0 +
                                        reqReceived?.Where(t => t.Status == ApplicationStatus.Waiting.ToString()).Count() ?? 0 + newCandidates;

                                    res.Add(new ApplicationBucketDTO()
                                    {
                                        IdPosition = position.IdPosition,
                                        PositionName = position.Name,
                                        TotalCount = reqReceived.Count(),
                                        NewApplicationCount = newCandidates,
                                        UnderReviewCount = underReviewCandidates,
                                        ShortListedCount = shortListedCandidates
                                    });
                                }

                            }
                        }

                    }

                }
                return new DALResult<List<ApplicationBucketDTO>>(Status.Found, res, null, null);
            }
            catch (Exception ex)
            {
                Logger.WriteLog(ex.Message.ToString()); return null;
            }
        }



        public DALResult<bool> ChangeStatus(int positionId, int JobStatus)
        {
            try
            {
                using (var gt = new AddantEntities1())
                {
                    Position existingPosition = gt.Positions.Where(x => x.IdPosition == positionId).FirstOrDefault();
                    if (existingPosition != null)
                    {
                        existingPosition.Status = JobStatus;

                        gt.Entry(existingPosition).State = EntityState.Modified;
                        gt.SaveChanges();

                        return new DALResult<bool>(Status.Created, true, null, null);
                    }
                }

            }
            catch (Exception ex) { return new DALResult<bool>(Status.Exception, false, null, null); }


            return new DALResult<bool>(Status.NotFound, false, null, null);

        }

        public DALResult<bool> UpdateDeletedStatus(int IdPosition, bool isDeleted)
        {
            try
            {
                using (var gt = new AddantEntities1())
                {
                    Position res = gt.Positions.Where(x => x.IdPosition == IdPosition).FirstOrDefault();
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
