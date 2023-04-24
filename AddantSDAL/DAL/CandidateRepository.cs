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
	public class CandidateRepository : ICandidateRepository
	{
		public DALResult<List<CandidateDTO>> GetAllCandidate(DateTime? startDate = null, DateTime? endDate = null, bool isAdminCall = false, string searchText = "", string status = "")
		{
			var res = new List<CandidateDTO>();
			try
			{
				using (var gt = new AddantEntities1())
				{
					if (startDate != null && endDate != null)
						res = gt.Candidates.Where(x => System.Data.Entity.DbFunctions.TruncateTime(x.CreatedOn) >= startDate && System.Data.Entity.DbFunctions.TruncateTime(x.CreatedOn) <= endDate
						// && x.Deleted == false
						).Select(x => new CandidateDTO
						{
							IsDeleted = x.IsDeleted,
							CreatedOn = x.CreatedOn,
							Description = x.Description,
							Email = x.Email,
							FirstName = x.FirstName,
							IdCandidate = x.IdCandidate,
							IdPosition = x.IdPosition,
							LastName = x.LastName,
							Mobile = x.Mobile,
							ResumeUrl = x.ResumeUrl,
							Position = x.Position.Name,
							Status = x.Status,
							Deleted=x.Deleted,
						}).OrderByDescending(c => c.CreatedOn).ToList();
					else
						res = gt.Candidates
							//.Where( && x.Deleted == false)
							.Select(x => new CandidateDTO
							{
								IsDeleted = x.IsDeleted,
								CreatedOn = x.CreatedOn,
								Description = x.Description,
								Email = x.Email,
								FirstName = x.FirstName,
								IdCandidate = x.IdCandidate,
								IdPosition = x.IdPosition,
								LastName = x.LastName,
								Mobile = x.Mobile,
								ResumeUrl = x.ResumeUrl,
								Position = x.Position.Name,
								Status = x.Status,
                                Deleted = x.Deleted

                            }).OrderByDescending(c => c.CreatedOn).ToList();

					res = res.Where(a => (a.FirstName != null ? (a.FirstName.ToUpper().Contains(searchText != null ? searchText?.ToUpper() : "") || string.IsNullOrWhiteSpace(searchText)) : string.IsNullOrWhiteSpace(searchText))
							 || (a.LastName != null ? (a.LastName.ToUpper().Contains(searchText != null ? searchText?.ToUpper() : "") || string.IsNullOrWhiteSpace(searchText)) : string.IsNullOrWhiteSpace(searchText))
							 || (a.Mobile != null ? (a.Mobile.Contains(searchText != null ? searchText?.ToUpper() : "") || string.IsNullOrWhiteSpace(searchText)) : string.IsNullOrWhiteSpace(searchText))
							 || (a.Description != null ? (a.Description.ToUpper().Contains(searchText != null ? searchText?.ToUpper() : "") || string.IsNullOrWhiteSpace(searchText)) : string.IsNullOrWhiteSpace(searchText))
							 || (a.Email != null ? (a.Email.ToUpper().Contains(searchText != null ? searchText?.ToUpper() : "") || string.IsNullOrWhiteSpace(searchText)) : string.IsNullOrWhiteSpace(searchText))
							 || (a.Position != null ? (a.Position.ToUpper().Contains(searchText != null ? searchText?.ToUpper() : "") || string.IsNullOrWhiteSpace(searchText)) : string.IsNullOrWhiteSpace(searchText))
							 || (a.IdPosition != null ? (a.IdPosition.ToString().Contains(searchText != null ? searchText?.ToUpper() : "") || string.IsNullOrWhiteSpace(searchText)) : string.IsNullOrWhiteSpace(searchText))

							 )

					 .ToList();
					if (!string.IsNullOrWhiteSpace(status))
						res = res.Where(x => (x.Status != null ? (x.Status.ToUpper().Contains(status != null ? status?.ToUpper() : "") || string.IsNullOrWhiteSpace(status)) : string.IsNullOrWhiteSpace(status))).ToList();
                    res = res.Where(c => c.Deleted != true).ToList();

                    if (!isAdminCall)
						res = res.Where(c => c.IsDeleted == isAdminCall).ToList();
                    res = res.Where(c => c.Deleted != true).ToList();
					res.OrderByDescending(c => c.IdCandidate);
				}
				return new DALResult<List<CandidateDTO>>(Status.Found, res, null, null);
			}
			catch (Exception ex) { return null; }
		}
		public DALResult<CandidateDTO> Create(CandidateDTO candidateDto)
		{
			try
			{
				using (var gt = new AddantEntities1())
				{
					Candidate _data = new Candidate
					{
						CreatedOn = candidateDto.CreatedOn,
						Description = candidateDto.Description,
						Email = candidateDto.Email,
						FirstName = candidateDto.FirstName,
						IdPosition = candidateDto.IdPosition,
						IsDeleted = candidateDto.IsDeleted,
						LastName = candidateDto.LastName,
						Mobile = candidateDto.Mobile,
						ResumeUrl = candidateDto.ResumeUrl,
						IdCandidate = candidateDto.IdCandidate,
						Deleted=false,
						Status = string.IsNullOrWhiteSpace(candidateDto.Status) ? "Processing" : candidateDto.Status
					};
					var existing = gt.Candidates.Where(x => x.IdCandidate == candidateDto.IdCandidate).FirstOrDefault();
					if (existing != null)
					{
						_data.ResumeUrl = string.IsNullOrEmpty(candidateDto.ResumeUrl) ? existing.ResumeUrl : candidateDto.ResumeUrl;
						gt.Entry(existing).CurrentValues.SetValues(_data);
					}
					else
						gt.Candidates.Add(_data);
					gt.SaveChanges();
					candidateDto.IdCandidate = (int)_data?.IdCandidate;
					candidateDto.IdPosition = _data?.IdPosition;
					candidateDto.ResumeUrl = _data?.ResumeUrl;
					candidateDto.Position = gt.Positions.Where(x => x.IdPosition == _data.IdPosition)?.FirstOrDefault().Name;
					candidateDto.Status = _data?.Status;
					return new DALResult<CandidateDTO>(Status.Created, candidateDto, null, null);
				}
			}
			catch (Exception ex) { return null; }
		}
		public DALResult<CandidateDTO> GetCandidateById(int candidateId)
		{
			var candidateDto = new CandidateDTO();
			try
			{
				using (var gt = new AddantEntities1())
				{
					candidateDto = gt.Candidates.Where(x => x.IdCandidate == candidateId
					//   && x.Deleted == false
					).Select(x => new CandidateDTO
					{
						IdCandidate = x.IdCandidate,
						CreatedOn = x.CreatedOn,
						Description = x.Description,
						Email = x.Email,
						FirstName = x.FirstName,
						IdPosition = x.IdPosition,
						ResumeUrl = x.ResumeUrl,
						LastName = x.LastName,
						IsDeleted = x.IsDeleted,
						Deleted=x.Deleted
					}).FirstOrDefault();
					return new DALResult<CandidateDTO>(Status.Found, candidateDto, null, null);
				}
			}
			catch (Exception ex) { return new DALResult<CandidateDTO>(Status.Exception, candidateDto, null, null); }
		}
		public DALResult<bool> DisableCandidate(int IdCandidate = 0, bool isDeleted = false)
		{
			try
			{
				Candidate Candidates = new Candidate();
				using (var gt = new AddantEntities1())
				{
					var existingRes = gt.Candidates.Where(x => x.IdCandidate == IdCandidate).FirstOrDefault();
					if (existingRes != null)
					{
						Candidates = existingRes;
						Candidates.IsDeleted = isDeleted;
					}
					gt.Entry(existingRes).CurrentValues.SetValues(Candidates);
					gt.SaveChanges();
					return new DALResult<bool>(Status.Deleted, true, null, null);
				}
			}
			catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return new DALResult<bool>(Status.Exception, false, null, null); }
		}

		public DALResult<bool> Delete(int IdCandidate = 0)
		{
			try
			{
				Candidate Candidates = new Candidate();
				using (var gt = new AddantEntities1())
				{
					var existingRes = gt.Candidates.Where(x => x.IdCandidate == IdCandidate).FirstOrDefault();
					if (existingRes != null)
					{
						Candidates = existingRes;
						//Candidates.Deleted = true;
					}
					gt.Entry(existingRes).CurrentValues.SetValues(Candidates);
					gt.SaveChanges();
					return new DALResult<bool>(Status.Deleted, true, null, null);
				}
			}
			catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return new DALResult<bool>(Status.Exception, false, null, null); }
		}


		public DALResult<List<CandidateDTO>> GetCandidateByPositionId(int positionId)
		{
			var candidateDto = new List<CandidateDTO>();
			try
			{
				using (var gt = new AddantEntities1())
				{
					var candidateList = gt.Candidates.Where(x => x.IdPosition == positionId).ToList();
					candidateList = gt.Candidates.Where(x => x.Deleted != true).ToList();
                    if (candidateList != null && candidateList?.Count > 0)
					{
						foreach (var candidate in candidateList)
						{
							candidateDto.Add(new CandidateDTO
							{
								IdCandidate = candidate.IdCandidate,
								CreatedOn = candidate.CreatedOn,
								Description = candidate.Description,
								Email = candidate.Email,
								FirstName = candidate.FirstName,
								IdPosition = candidate.IdPosition,
								ResumeUrl = candidate.ResumeUrl,
								LastName = candidate.LastName,
								IsDeleted = candidate.IsDeleted,
								Mobile=candidate.Mobile,
								Status=candidate.Status,
								Deleted=candidate.Deleted

							});
						}
					}
					return new DALResult<List<CandidateDTO>>(Status.Found, candidateDto, null, null);
				}
			}
			catch (Exception ex) { return new DALResult<List<CandidateDTO>>(Status.Exception, candidateDto, null, null); }
		}


        public DALResult<bool> UpdateDeletedStatus(int IdCandidate, bool isDeleted)
        {
            try
            {
                using (var gt = new AddantEntities1())
                {
                    Candidate res = gt.Candidates.Where(x => x.IdCandidate == IdCandidate).FirstOrDefault();
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
