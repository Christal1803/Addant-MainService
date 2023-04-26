using AddantSDal.DTO;
using AddantSDAL;
using AddantSDAL.DAL;
using AddantSDAL.DTO;
using AddantService;
using AddantService.DAL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AddantSDal.DAL
{
    public class LoginRepository : ILoginRepository
    {
        public DALResult<LoginDTO> CheckLogin(LoginDTO user)
        {
            try
            {
                using (var context = new AddantEntities1())
                {
                    if (user.Password != null)
                    {
                        LoginDTO _data = context.Users.Where(x => x.Username.Replace(" ", "").ToLower() == user.UserName.Replace("", "").ToLower() && x.Password == user.Password && x.IsActive == true&&x.Deleted!=true).Select(
                            x => new LoginDTO
                            {
                                UserName = x.Username,
                                Password = x.Password,
                                UserId = x.UserId,
                                IsActive = x.IsActive,
                                Deleted=x.Deleted

                            }).FirstOrDefault();
                        if (_data != null && IsPasswordCaseSentive(user.Password, _data.UserId)&& IsUserNameCaseSentive(user.UserName, _data.UserId))
                            return new DALResult<LoginDTO>(Status.Found, _data, null, null);
                        else
                            return new DALResult<LoginDTO>(Status.Conflict, null,"Credential Mismatch" , null);
                    }
                    else
                    {
                        LoginDTO _data = context.Users.Where(x => x.Email.Replace(" ", "").ToLower().Trim() == user.UserName.Replace("", "").ToLower().Trim() && x.IsActive == true && x.Deleted != true).Select(
                                x => new LoginDTO
                                {
                                    UserName = x.Username,
                                    Password = x.Password,
                                    UserId = x.UserId,
                                    IsActive = x.IsActive,
                                    Deleted = x.Deleted


                                }).FirstOrDefault();
                        //if (_data != null && IsPasswordCaseSentive(user.Password))
                        return new DALResult<LoginDTO>(Status.Found, _data, null, null);
                    }
                }
            }
            catch (Exception ex)
            {
                return new DALResult<LoginDTO>(Status.Exception, null, ex.Message.ToString(), null);
            }
        }
        public DALResult<List<CandidateDTO>> GetShortListed()
        {
            try
            {
                using (var gt = new AddantEntities1())
                {
                    // gt.Candidates.Where(x => x.Status.Trim().ToLower() == "shortlisted").ToList().Count;

                    List<CandidateDTO> _data = gt.Candidates.Where(x => x.Status.Trim().ToLower() == "shortlisted").Select(
                                  x => new CandidateDTO
                                  {
                                      CreatedOn = x.CreatedOn,
                                      Description = x.Description,
                                      FirstName = x.FirstName,
                                      LastName = x.LastName,
                                      Email = x.Email,
                                      IdCandidate = x.IdCandidate,
                                      IdPosition = x.IdPosition,
                                      Mobile = x.Mobile,
                                      IsDeleted = x.IsDeleted,
                                      Position = x.Position.Name,
                                      Status = x.Status

                                  }).ToList();
                    //if (_data != null)
                    return new DALResult<List<CandidateDTO>>(Status.Found, _data, null, null);

                }
            }
            catch (Exception ex)
            {
                return new DALResult<List<CandidateDTO>>(Status.Exception, null, ex.Message.ToString(), null);
            }

        }
        public DALResult<List<DashBoardBlockData>> GetDashBoardBlockData(int Category)
        {
            // new DashBoardBlockData { Name = "ResumeCount", Total = countResume(false) , Today = countResume(true)}
            int _todayCount = 0;
            List<DashBoardBlockData> dashBoardBlockData = new List<DashBoardBlockData>();
            var _dataToday = (AddantSDAL.DAL.GoogleConsoleSearch.GetWebsiteVisit(Category, "today", "today")?.Object);
            if (_dataToday != null)
                _todayCount = Convert.ToInt32(_dataToday.ViewCount);
            try
            {
                using (var gt = new AddantEntities1())
                {
                    //int BlogAndPositioncount = gt.Positions.Where(x => x.IsDeleted == true).ToList().Count + gt.Blogs.Where(x => x.IsDeleted == true).ToList().Count;
                    int BlogAndPositioncount = gt.Blogs.Where(x => x.IsDeleted == true&& x.Deleted !=true).ToList().Count;



                    dashBoardBlockData = new List<DashBoardBlockData>() { new DashBoardBlockData { Name = "Enquiry", Total = gt.Enquiries.Where(x => x.Deleted !=true).ToList().Count , BlogToReview =gt.Enquiries.Where(x => x.Deleted !=true && System.Data.Entity.DbFunctions.TruncateTime(x.CreatedOn) == DateTime.Today)
              .ToList().Count,TotalToReview=BlogAndPositioncount },
              //      gt.Enquiries.Where(e => e.Deleted == false &&
              //             System.Data.Entity.DbFunctions.TruncateTime(e.CreatedOn) == DateTime.Today)
              //.ToList().Count


                       new DashBoardBlockData { Name = "Candidate", Total = gt.Candidates.Where(x => x.Deleted != true).ToList().Count, BlogToReview = gt.Candidates.Where(x=>x.Deleted != true &&System.Data.Entity.DbFunctions.TruncateTime(x.CreatedOn) == DateTime.Today&& x.Status.Trim().ToLower()=="processing").ToList().Count ,ShortListed= gt.Candidates.Where(x=>x.Status.Trim().ToLower()=="shortlisted"&& x.Deleted!=true).ToList().Count},
                                                                           new DashBoardBlockData { Name = "Blog", Total = gt.Blogs.Where(x => x.Deleted != true).ToList().Count , BlogToReview = gt.Blogs.Where(x=>x.Deleted != true &&System.Data.Entity.DbFunctions.TruncateTime(x.CreatedOn) == DateTime.Today && x.IsDeleted==false).ToList().Count,BlogPublished = gt.Blogs.Where(x=>x.IsDeleted==false&&x.Deleted!=true).ToList().Count,TotalToReview=BlogAndPositioncount},
                                                                        //   new DashBoardBlockData{ Name="VisitorCount", Total=Convert.ToInt32((AddantSDAL.DAL.GoogleConsoleSearch.GetWebsiteVisit(Category,"2022-01-01", "today")?.Object).ViewCount) ,BlogToReview=_todayCount,BlogPublished =0,TotalToReview=0},
                                                                         // new  DashBoardBlockData{ Name="CountrywiseVisitorCount", Total=Convert.ToInt32((AddantSDAL.DAL.GoogleConsoleSearch.GetWebsiteVisit(Category,"2022-01-01", "today")?.Object)) ,Today=_todayCount,Weekly =0,Monthly=0}
                                                                          new DashBoardBlockData{ Name="Job", Total= gt.Candidates.Where(x=>x.Deleted != true &&System.Data.Entity.DbFunctions.TruncateTime(x.CreatedOn) == DateTime.Today).ToList().Count,BlogToReview=0,BlogPublished =0,TotalToReview=0},
                };

                }
                return new DALResult<List<DashBoardBlockData>>(Status.Found, dashBoardBlockData, null, null);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return new DALResult<List<DashBoardBlockData>>(Status.Exception, null, ex.Message.ToString(), null); }
        }
        public int countResume(bool countToday)
        {
            try
            {
                return countToday ? Directory.GetFiles(HttpContext.Current.Server.MapPath("~/Uploads/Resume"))
                 .Where(x => new FileInfo(x).CreationTime.Date == DateTime.Today.Date).ToList().Count : Directory.GetFiles(HttpContext.Current.Server.MapPath("~/Uploads/Resume")).Length;
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return 0; }
        }
        private static bool IsPasswordCaseSentive(string name = "", Guid? userId = null)
        {
            try
            {
                char[] savedName = new char[] { };
                char[] nameRequired = name.ToCharArray();
                bool isMatch = false;
                using (AddantEntities1 dbo = new AddantEntities1())
                {
                    var user = dbo.Users.Where(x => x.Password == name && x.UserId == userId).FirstOrDefault();
                    if (user != null)
                    {
                        savedName = user.Password.ToCharArray();

                        if (savedName.SequenceEqual(nameRequired))
                        {
                            isMatch = true;
                        }
                        else
                            isMatch = false;
                    }
                }
                return isMatch;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private static bool IsUserNameCaseSentive(string name = "", Guid? userId = null)
        {
            try
            {
                char[] savedName = new char[] { };
                char[] nameRequired = name.ToCharArray();
                bool isMatch = false;
                using (AddantEntities1 dbo = new AddantEntities1())
                {
                    var user = dbo.Users.Where(x => x.Username == name && x.UserId == userId).FirstOrDefault();
                    if (user != null)
                    {
                        savedName = user.Username.ToCharArray();

                        if (savedName.SequenceEqual(nameRequired))
                        {
                            isMatch = true;
                        }
                        else
                            isMatch = false;
                    }
                }
                return isMatch;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DALResult<UserDTO> SendOTP(string UserName)
        {
            var candidateDto = new UserDTO();

            try
            {
                using (var gt = new AddantEntities1())
                {

                    LoginDTO _data = gt.Users.Where(x => x.Username.Replace(" ", "").ToLower() == UserName.Replace("", "").ToLower() && x.IsActive == true).Select(
                   x => new LoginDTO
                   {
                       UserName = x.Username,
                       Password = x.Password,
                       UserId = x.UserId,
                       IsActive = x.IsActive,

                   }).FirstOrDefault();

                    candidateDto = gt.Users.Where(x => x.Username.Replace(" ", "").ToLower() == UserName.Replace(" ", "").ToLower())
                     .Select(x => new UserDTO
                     {
                         UserId = x.UserId,
                         FirstName = x.FirstName,
                         LastName = x.LastName,
                         Email = x.Email,
                         Mobile = x.Mobile,
                         BloodGroup = x.BloodGroup,
                         Dob = x.Dob,
                         Designation = x.Designation,
                         Role = x.Role,
                         EmployeeID = x.EmployeeID,
                         ProfileImageUrl = x.ProfileImageUrl,
                         Username = x.Username,
                         Password = x.Password,
                         IsActive = x.IsActive,
                         CreatedOn = x.CreatedOn,
                         IdUserRole = x.IdUserRole
                     }).Where(x => x.Username == UserName).FirstOrDefault();
                    return new DALResult<UserDTO>(AddantService.DAL.Status.Found, candidateDto, null, null);

                }
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return new DALResult<UserDTO>(Status.Exception, candidateDto, null, null); }

        }
        public DALResult<OTPDto> CreateOTP(OTPDto otp)
        {
            OTPDto OData = new OTPDto();
            try
            {
                using (var context = new AddantEntities1())
                {
                    OTPHistory _data = context.OTPHistories.Add(new OTPHistory
                    {
                        UserId = otp.UserName,
                        OTP = otp.OTP,
                        TimeStamp = otp.TimeStamp,
                        Status = otp.Status
                    });

                    context.SaveChanges();
                    OData.UserName = _data.UserId;
                    OData.OTP = _data.OTP;
                    OData.TimeStamp = _data.TimeStamp;
                    OData.Status = _data.Status;

                    return new DALResult<OTPDto>(Status.Found, OData, null, null);
                }
            }
            catch (Exception ex)
            {
                return new DALResult<OTPDto>(Status.Exception, OData, ex.Message.ToString(), null);
            }
        }

        public DALResult<OTPDto> ValidateOTP(string UserName, string otp)
        {
            OTPDto _data = new OTPDto();
            try
            {
                using (var context = new AddantEntities1())
                {

                    DateTime dt = Convert.ToDateTime(context.OTPHistories.OrderByDescending(y => y.TimeStamp).Where(x => x.UserId.Trim().ToLower().ToString() == UserName.ToLower().Trim() && x.OTP.ToLower().Trim() == otp.ToLower().Trim()).FirstOrDefault()?.TimeStamp.Value.ToString()).AddHours(1);
                    var data = context.OTPHistories.Where(x => x.UserId.Trim().ToLower().ToString() == UserName.ToLower().Trim()).OrderByDescending(y => y.TimeStamp).FirstOrDefault();
                    if (data != null && data.OTP == otp)
                    {
                        Logger.WriteLog($"ValidateOTP Case1 DateTime.Now - {DateTime.Now} dt - {dt}");
                        bool isValid = DateTime.Now <= dt;
                        Logger.WriteLog($"ValidateOTP Case2 {isValid}");
                        if (isValid)
                            _data = new OTPDto()
                            {
                                OTP = data.OTP,
                                Status = data.Status,
                                TimeStamp = data.TimeStamp,
                                UserName = data.UserId
                            };
                    }

                }
                return new DALResult<OTPDto>(Status.Found, _data, null, null);
            }
            catch (Exception ex)
            {
                return new DALResult<OTPDto>(Status.Exception, _data, ex.Message.ToString(), null);
            }
        }

        public DALResult<bool> ResetPassword(UserDTO userDTO)
        {
            try
            {
                User Candidates = new User();
                using (var gt = new AddantEntities1())
                {
                    var existingRes = gt.Users.Where(x => x.Username.ToString().ToLower().Trim() == userDTO.Username.ToString().ToLower().Trim()).FirstOrDefault();
                    if (existingRes != null)
                    {
                        Candidates = existingRes;
                        Candidates.Password = userDTO.Password;

                        gt.Entry(existingRes).CurrentValues.SetValues(Candidates);
                        gt.SaveChanges();
                        return new DALResult<bool>(Status.Deleted, true, null, null);
                    }
                    else
                        return new DALResult<bool>(Status.Deleted, false, null, null);
                }
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return new DALResult<bool>(Status.Exception, false, null, null); }
        }

    }
}
