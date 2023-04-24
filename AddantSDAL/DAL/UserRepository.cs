
using AddantSDAL.DTO;
using AddantService;
using AddantService.DAL;
using System;
using System.Activities.Expressions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AddantSDAL.DAL
{
    public class UserRepository : IUserRepository
    {
        public DALResult<List<UserDTO>> GetAllUser(DateTime? startDate, DateTime? endDate, string searchText = "", string status = "", bool IsActive = true)
        {
            var res = new List<UserDTO>();
            try
            {
                using (var gt = new AddantEntities1())
                {
                    if (startDate != null && endDate != null)
                        res = gt.Users.Where(f => System.Data.Entity.DbFunctions.TruncateTime(f.CreatedOn) >= startDate && System.Data.Entity.DbFunctions.TruncateTime(f.CreatedOn) <= endDate
                        // && x.Deleted ==false
                        ).Select(x => new UserDTO
                        {
                            UserId = x.UserId,
                            FirstName = x.FirstName,
                            LastName = x.LastName,
                            Email = x.Email,
                            Mobile = x.Mobile,
                            BloodGroup = x.BloodGroup,
                            Dob = x.Dob,
                            Designation = x.Designation,
                            Role = x.UserRole.Name,
                            EmployeeID = x.EmployeeID,
                            ProfileImageUrl = x.ProfileImageUrl,
                            Username = x.Username,
                            Password = x.Password,
                            IsActive = x.IsActive,
                            CreatedOn = x.CreatedOn,
                            IdUserRole = x.IdUserRole,
                            Deleted=x.Deleted,
                        }).OrderByDescending(c => c.CreatedOn).ToList();
                    else
                        res = gt.Users
                            //  .Where(   //y=> y.Deleted ==false)
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
                                Role = x.UserRole.Name,
                                EmployeeID = x.EmployeeID,
                                ProfileImageUrl = x.ProfileImageUrl,
                                Username = x.Username,
                                Password = x.Password,
                                IsActive = x.IsActive,
                                CreatedOn = x.CreatedOn,
                                IdUserRole = x.IdUserRole,
                                Deleted = x.Deleted,

                            }).OrderByDescending(c => c.CreatedOn).ToList();
                    var result = gt.Users.ToList();
                    var RoleData = gt.UserRoles.ToList();
                    if (RoleData != null && RoleData?.Count > 0)
                    {
                        foreach (var role in RoleData)
                        {
                            var reqReceived = result.Where(t => t.IdUserRole == role.IdUserRole).ToList();
                            if (reqReceived != null && reqReceived?.Count > 0)
                            {

                                res.Add(new UserDTO()
                                {
                                    Role = role.Name
                                });
                            }
                        }
                    }
                    res = res.Where(a => (a.FirstName != null ? (a.FirstName.ToUpper().Contains(searchText != null ? searchText?.ToUpper() : "") || string.IsNullOrWhiteSpace(searchText)) : string.IsNullOrWhiteSpace(searchText))
                             || (a.LastName != null ? (a.LastName.ToUpper().Contains(searchText != null ? searchText?.ToUpper() : "") || string.IsNullOrWhiteSpace(searchText)) : string.IsNullOrWhiteSpace(searchText))
                             || (a.Mobile != null ? (a.Mobile.Contains(searchText != null ? searchText?.ToUpper() : "") || string.IsNullOrWhiteSpace(searchText)) : string.IsNullOrWhiteSpace(searchText))
                             || (a.Email != null ? (a.Email.ToUpper().Contains(searchText != null ? searchText?.ToUpper() : "") || string.IsNullOrWhiteSpace(searchText)) : string.IsNullOrWhiteSpace(searchText))
                             || (a.Username != null ? (a.Username.ToUpper().Contains(searchText != null ? searchText?.ToUpper() : "") || string.IsNullOrWhiteSpace(searchText)) : string.IsNullOrWhiteSpace(searchText))
                             || (a.Role != null ? (a.Role.ToUpper().ToString().Contains(searchText != null ? searchText?.ToUpper() : "") || string.IsNullOrWhiteSpace(searchText)) : string.IsNullOrWhiteSpace(searchText))

                             )
                               .OrderByDescending(c => c.CreatedOn)
                     .ToList();
                    if (IsActive == true)
                        res = res.Where(c => c.IsActive == IsActive).ToList();
                    res = res.Where(c => c.Deleted != true).ToList();
                }
                return new DALResult<List<UserDTO>>(Status.Found, res, null, null);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userDto"></param>
        /// <returns></returns>
        /// 
        public DALResult<UserDTO> UploadImage(UserDTO _data)
        {
            try
            {
                using (var gt = new AddantEntities1())
                {
                    var existing = gt.Users.Where(x => x.UserId == _data.UserId).FirstOrDefault();
                    if (existing != null)
                    {
                        _data.ProfileImageUrl = string.IsNullOrEmpty(_data.ProfileImageUrl) ? existing.ProfileImageUrl : _data.ProfileImageUrl;

                        gt.Entry(existing).CurrentValues.SetValues(_data);
                        gt.SaveChanges();
                        return new DALResult<UserDTO>(Status.Created, _data, null, null);
                    }

                    return new DALResult<UserDTO>(Status.Created, _data, null, null);
                }
            }

            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return new DALResult<UserDTO>(Status.Exception, _data, null, null); }
        }

        public DALResult<UserDTO> Create(UserDTO userDto)
        {
            try
            {
                using (var gt = new AddantEntities1())
                {
                    User _data = new User
                    {
                        UserId = string.IsNullOrEmpty(userDto.UserId.ToString()) ? System.Guid.NewGuid() : (Guid)userDto.UserId,
                        FirstName = userDto.FirstName,
                        LastName = userDto.LastName,
                        Email = userDto.Email,
                        Mobile = userDto.Mobile,
                        BloodGroup = userDto.BloodGroup,
                        Dob = userDto.Dob,
                        Designation = userDto.Designation,
                        Role = userDto.Role,
                        EmployeeID = userDto.EmployeeID,
                        ProfileImageUrl = userDto.ProfileImageUrl,
                        Username = userDto.Username,
                        Password = userDto.Password,
                        CreatedOn = userDto.CreatedOn != null ? userDto.CreatedOn : DateTime.Now,
                        IsActive = userDto.IsActive,
                        IdUserRole = userDto.IdUserRole,
                        Deleted=false
                    };
                    var existing = gt.Users.Where(x => x.UserId == userDto.UserId).FirstOrDefault();
                    if (existing != null)
                    {
                        _data.ProfileImageUrl = string.IsNullOrEmpty(userDto.ProfileImageUrl) ? existing.ProfileImageUrl :
                            (userDto.ProfileImageUrl.Contains("https:") || userDto.ProfileImageUrl.Contains("http:")) ? existing.ProfileImageUrl : userDto.ProfileImageUrl;

                        if (userDto.ProfileImageUrl.Contains("https:") || userDto.ProfileImageUrl.Contains("http:"))
                        {
                            _data.ProfileImageUrl = existing.ProfileImageUrl;
                            userDto.ProfileImageUrl = existing.ProfileImageUrl;

                        }
                        gt.Entry(existing).CurrentValues.SetValues(_data);
                    }
                    else
                    {
                        var existingEmail = gt.Users.Where(x => x.Email == userDto.Email).FirstOrDefault();
                        if (existingEmail != null)
                            return new DALResult<UserDTO>(Status.Conflict, null, "already exists", null);
                        else
                            gt.Users.Add(_data);
                    }
                    gt.SaveChanges();

                    return new DALResult<UserDTO>(Status.Created, userDto, null, null);
                }
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
        }
        public DALResult<UserDTO> GetUserById(string userId)
        {
            var candidateDto = new UserDTO();
            try
            {
                using (var gt = new AddantEntities1())
                {
                    candidateDto = gt.Users.Where(x => x.UserId.ToString() == userId
                    // && x.Deleted ==false
                    ).Select(x => new UserDTO
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
                        IdUserRole = x.IdUserRole,
                        Deleted=x.Deleted
                    }).FirstOrDefault();
                    var roles = gt.UserRoles.ToList();
                    candidateDto.Role = roles.Where(t => t.IdUserRole == candidateDto.IdUserRole)?.FirstOrDefault().Name;
                    return new DALResult<UserDTO>(Status.Found, candidateDto, null, null);
                }
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return new DALResult<UserDTO>(Status.Exception, candidateDto, null, null); }
        }
        public DALResult<bool> DisableUser(string IdUser = "", bool IsActive = false)
        {
            try
            {
                User Candidates = new User();
                using (var gt = new AddantEntities1())
                {
                    var existingRes = gt.Users.Where(x => x.UserId.ToString() == IdUser).FirstOrDefault();
                    if (existingRes != null)
                    {
                        Candidates = existingRes;
                        Candidates.IsActive = IsActive;
                    }
                    gt.Entry(existingRes).CurrentValues.SetValues(Candidates);
                    gt.SaveChanges();
                    return new DALResult<bool>(Status.Deleted, true, null, null);
                }
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return new DALResult<bool>(Status.Exception, false, null, null); }
        }

        public DALResult<bool> ChangePassword(UserDTO userDTO)
        {

            ///Getting url from update url and getting change password from password field date
            try
            {
                User Candidates = new User();
                using (var gt = new AddantEntities1())
                {
                    var existingRes = gt.Users.Where(x => x.Username.ToString().ToLower().Trim() == userDTO.Username.ToString().ToLower().Trim() && x.Password.ToString().Trim() == userDTO.Password.ToString().Trim()).FirstOrDefault();
                    if (existingRes != null)
                    {
                        Candidates = existingRes;
                        Candidates.Password = userDTO.LastName;

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

        public DALResult<bool> Delete(string IdUser = "")
        {
            try
            {
                User users = new User();
                using (var gt = new AddantEntities1())
                {
                    var existingRes = gt.Users.Where(x => x.UserId.ToString() == IdUser).FirstOrDefault();
                    if (existingRes != null)
                    {
                        users = existingRes;
                        //  users.Deleted = true;
                    }
                    gt.Entry(existingRes).CurrentValues.SetValues(users);
                    gt.SaveChanges();
                    return new DALResult<bool>(Status.Deleted, true, null, null);
                }
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return new DALResult<bool>(Status.Exception, false, null, null); }
        }
        public DALResult<bool> UpdateDeletedStatus(Guid idUser, bool isDeleted) 
        {
            try
            {
                using (var gt = new AddantEntities1())
                {
                    User res = gt.Users.Where(x => x.UserId == idUser).FirstOrDefault();
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
