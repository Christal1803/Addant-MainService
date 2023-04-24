using AddantSDAL.DTO;
using AddantService;
using AddantService.DAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace AddantSDAL.DAL
{
    public class UserRoleRepository : IUserRoleRepository
    {
        public DALResult<UserRoleDTO> CreateUserRole(UserRoleDTO userRoleDTO)
        {
            try
            {
                using (var gt = new AddantEntities1())
                {
                    UserRole userRole = new UserRole
                    {
                        Description = userRoleDTO.Description,
                        IdUserRole = userRoleDTO.IdUserRole,
                        IsActive = userRoleDTO.IsActive,
                        Name = userRoleDTO.Name,
                        Deleted=false
                    };
                    var _userRole = gt.UserRoles.Where(x => x.IdUserRole == userRoleDTO.IdUserRole).FirstOrDefault();
                    if (_userRole != null)
                    {
                        gt.Entry(_userRole).CurrentValues.SetValues(userRole);
                    }
                    else
                        gt.UserRoles.Add(userRole);
                    gt.SaveChanges();
                    userRoleDTO.IdUserRole = userRole.IdUserRole;

                    return new DALResult<UserRoleDTO>(Status.Created, userRoleDTO, null, null);
                }
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message); return new DALResult<UserRoleDTO>(Status.Exception, null, null, null); }
        }

        public DALResult<bool> Delete(int IdUserRole, bool isDelete)
        {
            try
            {
                using (var gt = new AddantEntities1())
                {
                    UserRole res = gt.UserRoles.Where(x => x.IdUserRole == IdUserRole).FirstOrDefault();
                    res.IsActive = isDelete;
                    gt.Entry(res).CurrentValues.SetValues(res);
                    gt.SaveChanges();
                    return new DALResult<bool>(Status.Deleted, true, null, null);
                }
            }
            catch (Exception ex) { return new DALResult<bool>(Status.Exception, false, null, null); }
        }

        public DALResult<List<UserRoleDTO>> GetAllUserRole(string searchText = "")
        {
            try
            {
                var result = new List<UserRoleDTO>();
                using (var gt = new AddantEntities1())
                {
                    result = gt.UserRoles
                    //	.Where(x=>x.Deleted==false)
                    .Select(s => new UserRoleDTO
                    {
                        Description = s.Description,
                        IdUserRole = s.IdUserRole,
                        IsActive = (bool)s.IsActive,
                        Name = s.Name,
                        UserCount = s.Users.Count(),
                        Deleted=s.Deleted

                    }).ToList();
                    foreach (var role in result)
                    {
                        var rolePrevileges = gt.UserPrivileges.Where(t => t.IdUserRole == role.IdUserRole).ToList();
                        if (rolePrevileges != null && rolePrevileges?.Count > 0)
                        {
                            var prev = rolePrevileges.Where(t => (t.CreatePage ?? false) && (t.DisablePage ?? false) && (t.ViewPage ?? false) && (t.Edit ?? false)&& (t.Deleted ?? false)).ToList();
                            if (prev != null && prev?.Count == rolePrevileges?.Count)
                                role.IsFullAccess = true;
                            else
                                role.IsFullAccess = false;
                        }
                        else
                            role.IsFullAccess = false;
                    }

                    result = result.Where(a => (a.Name != null ? (a.Name.ToUpper().Contains(searchText != null ? searchText?.ToUpper() : "") || string.IsNullOrWhiteSpace(searchText)) : string.IsNullOrWhiteSpace(searchText))
                                                 || (a.Description != null ? (a.Description.ToUpper().Contains(searchText != null ? searchText?.ToUpper() : "") || string.IsNullOrWhiteSpace(searchText)) : string.IsNullOrWhiteSpace(searchText))
                    ).OrderByDescending(c => c.IdUserRole).OrderByDescending(x => x.IsActive)
                          .ToList();

                    result = result.Where(c => c.Deleted != true).ToList();


                }
                return new DALResult<List<UserRoleDTO>>(Status.Found, result, null, null);
            }
            catch (Exception ex)
            {
                Logger.WriteLog(ex.Message.ToString()); return new DALResult<List<UserRoleDTO>>(Status.Exception, null, ex.Message.ToString(), null);
            }
        }

        public DALResult<List<UserRoleDTO>> GetAllUserRoleById(int IdUserRole)
        {
            try
            {
                var result = new List<UserRoleDTO>();
                using (var gt = new AddantEntities1())
                {
                    result = gt.UserRoles.Where(x => x.IdUserRole == IdUserRole
                    //&& x.Deleted==false
                    ).Select(s => new UserRoleDTO
                    {
                        Description = s.Description,
                        IdUserRole = s.IdUserRole,
                        IsActive = (bool)s.IsActive,
                        Name = s.Name
                    }).OrderByDescending(c => c.IdUserRole).ToList();
                    result = result.Where(c => c.Deleted != true).ToList();

                }
                return new DALResult<List<UserRoleDTO>>(Status.Found, result, null, null);
            }
            catch (Exception ex) { return new DALResult<List<UserRoleDTO>>(Status.Exception, null, ex.Message.ToString(), null); }
        }

        public DALResult<bool> DeleteUserRole(int IdUserRole)
        {
            try
            {
                using (var gt = new AddantEntities1())
                {
                    UserRole res = gt.UserRoles.Where(x => x.IdUserRole == IdUserRole).FirstOrDefault();
                    //	res.Deleted = true;
                    gt.Entry(res).CurrentValues.SetValues(res);
                    gt.SaveChanges();
                    return new DALResult<bool>(Status.Deleted, true, null, null);
                }
            }
            catch (Exception ex) { return new DALResult<bool>(Status.Exception, false, null, null); }
        }

        public DALResult<List<UserDTO>> GetAllUserByRoleId(int IdUserRole)
        {
            try
            {
                var result = new List<UserDTO>();
                using (var gt = new AddantEntities1())
                {
                    result = gt.Users.Where(x => x.IdUserRole == IdUserRole)
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
                         IdUserRole = x.IdUserRole,
                         Deleted=x.Deleted
                     }).ToList();
                    result = result.Where(x => x.Deleted != true).ToList();

                }
                return new DALResult<List<UserDTO>>(Status.Found, result, null, null);
            }
            catch (Exception ex) { return new DALResult<List<UserDTO>>(Status.Exception, null, ex.Message.ToString(), null); }
        }

        public DALResult<List<UserDTO>> GetAllUsersByRole(int IdUserRole)
        {
            try
            {
                var result = new List<UserDTO>();
                using (var gt = new AddantEntities1())
                {
                    result = gt.Users.Where(x => x.IdUserRole == IdUserRole)
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
                         //ProfileImageUrl = x.ProfileImageUrl!=string.Empty || x.ProfileImageUrl != null ? Path.Combine((Path.Combine(System.Configuration.ConfigurationManager.AppSettings["HostedDomainName"].ToString() + "/Uploads/Image", "")), x.ProfileImageUrl != null ? x.ProfileImageUrl : ""):"",
                         ProfileImageUrl = x.ProfileImageUrl,
                         Username = x.Username,
                         Password = x.Password,
                         IsActive = x.IsActive,
                         CreatedOn = x.CreatedOn,
                         IdUserRole = x.IdUserRole,
                         Deleted=x.Deleted

                     }).ToList();
                    result = result.Where(x => x.Deleted != true).ToList();

                }
                return new DALResult<List<UserDTO>>(Status.Found, result, null, null);
            }
            catch (Exception ex) { return new DALResult<List<UserDTO>>(Status.Exception, null, ex.Message.ToString(), null); }
        }

        public DALResult<bool> UpdateDeletedStatus(int idUserRole, bool isDeleted)
        {
            try
            {
                using (var gt = new AddantEntities1())
                {
                    UserRole res = gt.UserRoles.Where(x => x.IdUserRole == idUserRole).FirstOrDefault();
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
