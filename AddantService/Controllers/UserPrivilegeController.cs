using AddantSDAL.DAL;
using AddantSDAL.DTO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;

namespace AddantService.Controllers
{
    [RoutePrefix("api/UserPrivilege")]
    public class UserPrivilegeController : BaseController
    {
        public IUserPrivilegeRepository _userPrivilegeRepository;
        public UserPrivilegeController(IUserPrivilegeRepository userPrivilegeRepository)
        {
            _userPrivilegeRepository = userPrivilegeRepository;
        }
        // POST: Enquiry/Create
        [Authorize]
        [HttpPost]
        [Route("Create")]
        public IHttpActionResult Create([FromBody] Models.UserPrivilegeModel[] userPrivilegeModel)
        {
            try
            {
                List<UserPrivilegeDTO> lstUserPriviledge = new List<UserPrivilegeDTO>();
                foreach (var item in userPrivilegeModel)
                {
                    var _data = new UserPrivilegeDTO
                    {
                        CreatedDate = item.CreatedDate != null ? item.CreatedDate : DateTime.Now,
                        CreatePage = item.CreatePage,
                        DisablePage = item.DisablePage,
                        Edit = item.Edit,
                        IdPage = item.IdPage,
                        IdUserPrivilege = item.IdUserPrivilege,
                        IdUserRole = item.IdUserRole,
                        IsDeleted = item.IsDeleted,
                        ViewPage = item.ViewPage,
                        Deleted = false,
                        DeletePage = item.DeletePage,
                    };
                    lstUserPriviledge.Add(_data);
                }
                var res = _userPrivilegeRepository.Create(lstUserPriviledge);
                return WebResult(res);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
        }

        //[Authorize]
        [HttpGet]
        [Route("Detail")]
        public IHttpActionResult GetAllUserPrivilege(bool isAdminCall = false)
        {
            try
            {
                var res = _userPrivilegeRepository.GetAllUserPrivilege(isAdminCall);
                return WebResult(res);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
        }

        [Authorize]
        [HttpGet]
        [Route("Detail/{IdUserPrivilege}")]
        public IHttpActionResult GetAllEnquiry(int IdUserPrivilege)
        {
            try
            {
                var res = _userPrivilegeRepository.GetUserPrivilegeById(IdUserPrivilege);
                return WebResult(res);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
        }

        [Authorize]
        [HttpGet]
        [Route("CheckPrivilege/{idUser}")]
        public IHttpActionResult CheckUserPrivilege(Guid idUser)
        {
            try
            {
                var res = _userPrivilegeRepository.CheckUserPrivilegeExist(idUser);
                return WebResult(res);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
        }


        [Authorize]
        [HttpDelete]
        [Route("Detail/{IdUserPrivilege}/{isDelete}")]
        public IHttpActionResult Delete(int IdUserPrivilege, bool isDelete)
        {
            try
            {
                var res = _userPrivilegeRepository.Delete(IdUserPrivilege, isDelete);
                return WebResult(res);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("Page")]
        public IHttpActionResult GetAllPage()
        {
            try
            {
                var res = _userPrivilegeRepository.GetAllPage();
                return WebResult(res);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
        }


        [Authorize]
        [HttpDelete]
        [Route("Delete/{IdUserPrivilege}")]
        public IHttpActionResult Delete(int IdUserPrivilege)
        {
            try
            {
                var res = _userPrivilegeRepository.Delete(IdUserPrivilege);
                return WebResult(res);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
        }

		//[Authorize]
		[HttpGet]
		[Route("PrivilegeByRoleId/{RoleId}")]
		public IHttpActionResult GetAllUserPrivilegeByRoleId(int RoleId)
		{
			try
			{
				var res = _userPrivilegeRepository.GetUserPrivilegeByRoleId(RoleId);
				return WebResult(res);
			}
			catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
		}


        [AllowAnonymous]
        [Route("Deleted/{RoleId}")]
        [HttpPost]
        public IHttpActionResult DeletedStatus(int RoleId, bool isDeleted)
        {
            try
            {
                var res = _userPrivilegeRepository.UpdateDeletedStatus(RoleId, isDeleted);
                return WebResult(res);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
        }

    }
}