using AddantSDAL.DAL;
using AddantSDAL.DTO;
using AddantService;
using AddantService.DAL;
using Microsoft.PowerBI.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Status = AddantService.DAL.Status;

namespace AddantSDAL.DAL
{
    public class UserPrivilegeRepository : IUserPrivilegeRepository
    {
        public DALResult<List<UserPrivilegeDTO>> CheckUserPrivilegeExist(Guid idUser)
        {
            var userPrivilege = new List<UserPrivilegeDTO>();
            try
            {
                using (var gt = new AddantEntities1())
                {
                    userPrivilege = gt.UserPrivileges.Join(gt.Users, up => up.IdUserRole, us => us.IdUserRole, (up, us) => new { up, us }).Where(x => x.us.UserId == idUser).Select(s =>
                        new UserPrivilegeDTO()
                        {
                            CreatedDate = s.up.CreatedDate,
                            DisablePage = s.up.DisablePage,
                            Edit = s.up.Edit,
                            IdUserRole = s.up.IdUserRole,
                            CreatePage = s.up.CreatePage,
                            IdPage = s.up.IdPage,
                            IdUserPrivilege = s.up.IdUserPrivilege,
                            Deleted=s.up.Deleted,
                            DeletePage = s.up.DeletePage,
                        }).ToList();
                }
                return new DALResult<List<UserPrivilegeDTO>>(Status.Found, userPrivilege, null, null);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message); return new DALResult<List<UserPrivilegeDTO>>(Status.Exception, null, null, null); }
        }

        public DALResult<List<UserPrivilegeDTO>> Create(List<UserPrivilegeDTO> lstuserPrivilegeDTO)
        {
            try
            {
                int id = 0;
                List<UserPrivilegeDTO> lstPriviledge = new List<UserPrivilegeDTO>();

                using (var gt = new AddantEntities1())
                {
                    foreach (var item in lstuserPrivilegeDTO)
                    {
                        UserPrivilege userPrivilege = new UserPrivilege()
                        {
                            CreatedDate = item.CreatedDate,
                            CreatePage = item.CreatePage,
                            DisablePage = item.DisablePage,
                            Edit = item.Edit,
                            IdPage = item.IdPage,
                            IdUserPrivilege = item.IdUserPrivilege,
                            IdUserRole = item.IdUserRole,
                            IsDeleted = (bool)item.IsDeleted,
                            ViewPage = item.ViewPage,
                            Deleted = false,
                            DeletePage = item.DeletePage,
                        };
                        var existingUserPrivilege = gt.UserPrivileges.Where(x => x.IdUserPrivilege == item.IdUserPrivilege).FirstOrDefault();
                        if (existingUserPrivilege != null)
                            gt.Entry(existingUserPrivilege).CurrentValues.SetValues(userPrivilege);
                        else
                            gt.UserPrivileges.Add(userPrivilege);
                        gt.SaveChanges();
                        id = item.IdUserRole.Value;
                    }

                    var lstUserPrivilege = gt.UserPrivileges.Where(x => x.IdUserRole == id).ToList();
                    foreach (var item in lstUserPrivilege)
                    {
                        UserPrivilegeDTO userPrivilege = new UserPrivilegeDTO()
                        {
                            CreatedDate = item.CreatedDate,
                            CreatePage = item.CreatePage,
                            DisablePage = item.DisablePage,
                            Edit = item.Edit,
                            IdPage = item.IdPage,
                            IdUserPrivilege = item.IdUserPrivilege,
                            IdUserRole = item.IdUserRole,
                            IsDeleted = (bool)item.IsDeleted,
                            ViewPage = item.ViewPage,
                            Deleted = false,
                            DeletePage = item.DeletePage,
                        };
                        lstPriviledge.Add(userPrivilege);
                    }
                }
                return new DALResult<List<UserPrivilegeDTO>>(Status.Created, lstPriviledge, null, null);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message); return new DALResult<List<UserPrivilegeDTO>>(Status.Exception, null, null, null); }
        }

        public DALResult<bool> Delete(int idUserPrivilege, bool isDeleted)
        {
            try
            {
                using (var gt = new AddantEntities1())
                {
                    var userPrivilegeDTO = gt.UserPrivileges.Where(x => x.IdUserPrivilege == idUserPrivilege).FirstOrDefault();
                    if (userPrivilegeDTO != null)
                    {
                        userPrivilegeDTO.IsDeleted = isDeleted;
                        gt.Entry(userPrivilegeDTO).CurrentValues.SetValues(userPrivilegeDTO);
                        gt.SaveChanges();
                    }
                }
                return new DALResult<bool>(Status.Deleted, true, null, null);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message); return new DALResult<bool>(Status.Accepted, false, null, null); }
        }

        public DALResult<List<UserPrivilegeDTO>> GetAllUserPrivilege(bool isAdminCall = false)
        {
            try
            {
                var userPrivileges = new List<UserPrivilegeDTO>();
                using (var gt = new AddantEntities1())
                {
                    userPrivileges = gt.UserPrivileges.Select(s => new UserPrivilegeDTO()
                    {
                        CreatedDate = s.CreatedDate,
                        CreatePage = s.CreatePage,
                        DisablePage = s.DisablePage,
                        Edit = s.Edit,
                        IdPage = s.IdPage,
                        IdUserPrivilege = s.IdUserPrivilege,
                        IdUserRole = s.IdUserRole,
                        IsDeleted = s.IsDeleted,
                        ViewPage = s.ViewPage,
                        Deleted=s.Deleted,
                        DeletePage = s.DeletePage,

                    }).ToList();
                    userPrivileges = userPrivileges.Where(x => x.Deleted != true).ToList();

                }

                return new DALResult<List<UserPrivilegeDTO>>(Status.Found, userPrivileges, null, null);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message); return new DALResult<List<UserPrivilegeDTO>>(Status.Exception, null, null, null); }
        }

        public DALResult<UserPrivilegeDTO> GetUserPrivilegeById(int idUserPrivilege)
        {
            try
            {
                var userPrivilege = new UserPrivilegeDTO();
                using (var gt = new AddantEntities1())
                {
                    userPrivilege = gt.UserPrivileges.Where(x => x.IdUserPrivilege == idUserPrivilege).Select(s => new UserPrivilegeDTO()
                    {
                        CreatedDate = s.CreatedDate,
                        CreatePage = s.CreatePage,
                        DisablePage = s.DisablePage,
                        Edit = s.Edit,
                        IdPage = s.IdPage,
                        IdUserPrivilege = s.IdUserPrivilege,
                        IdUserRole = s.IdUserRole,
                        IsDeleted = s.IsDeleted,
                        ViewPage = s.ViewPage,
                        Deleted=s.Deleted,
                        DeletePage = s.DeletePage,
                    }).FirstOrDefault();
                }
                return new DALResult<UserPrivilegeDTO>(Status.Found, userPrivilege, null, null);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message); return new DALResult<UserPrivilegeDTO>(Status.Exception, null, null, null); }
        }
        public DALResult<List<PageDTO>> GetAllPage()
        {
            try
            {
                var pages = new List<PageDTO>();
                using (var gt = new AddantEntities1())
                {
                    pages = gt.Pages.Where(x => x.IsDeleted == false).Select(s => new PageDTO()
                    {
                        IdPage = s.IdPage,
                        IsDeleted = (bool)s.IsDeleted,
                        Name = s.Name,
                        Deleted=s.Deleted
                    }).ToList();
                    //pages = pages.Where(x => x.Deleted == false).ToList();
                }
                return new DALResult<List<PageDTO>>(Status.Found, pages, null, null);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message); return new DALResult<List<PageDTO>>(Status.Exception, null, null, null); }
        }

        public DALResult<bool> Delete(int idUserPrivilege)
        {
            try
            {
                using (var gt = new AddantEntities1())
                {
                    var userPrivilegeDTO = gt.UserPrivileges.Where(x => x.IdUserPrivilege == idUserPrivilege).FirstOrDefault();
                    if (userPrivilegeDTO != null)
                    {
                        //	userPrivilegeDTO.Deleted = true;
                        gt.Entry(userPrivilegeDTO).CurrentValues.SetValues(userPrivilegeDTO);
                        gt.SaveChanges();
                    }
                }
                return new DALResult<bool>(Status.Deleted, true, null, null);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message); return new DALResult<bool>(Status.Accepted, false, null, null); }
        }

        ////New User Priviledge by UserId
        ///

        public DALResult<List<UserPrivilegeDTO>> GetUserPrivilegeByRoleId(int RoleId)
        {
            try
            {
                var userPrivilege = new List<UserPrivilegeDTO>();
                using (var gt = new AddantEntities1())
                {
                    userPrivilege = gt.UserPrivileges.Where(x => x.IdUserRole == RoleId).Select(s => new UserPrivilegeDTO()
                    {
                        CreatedDate = s.CreatedDate,
                        CreatePage = s.CreatePage,
                        DisablePage = s.DisablePage,
                        Edit = s.Edit,
                        IdPage = s.IdPage,
                        IdUserPrivilege = s.IdUserPrivilege,
                        IdUserRole = s.IdUserRole,
                        IsDeleted = s.IsDeleted,
                        ViewPage = s.ViewPage,
                        Deleted = s.Deleted,
                        DeletePage = s.DeletePage,
                    }).ToList();
                    userPrivilege = userPrivilege.Where(x => x.Deleted != true).ToList();


                }
                return new DALResult<List<UserPrivilegeDTO>>(Status.Found, userPrivilege, null, null);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message); return new DALResult<List<UserPrivilegeDTO>>(Status.Exception, null, null, null); }
        }

        public DALResult<bool> UpdateDeletedStatus(int RoleId, bool isDeleted)
        {
            try
            {
                using (var gt = new AddantEntities1())
                {
                    UserPrivilege res = gt.UserPrivileges.Where(x => x.IdUserRole == RoleId).FirstOrDefault();
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
