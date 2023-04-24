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
    [RoutePrefix("api/UserRole")]
    public class UserRoleController : BaseController
    {
        public IUserRoleRepository  _userRoleRepository;
        public UserRoleController(IUserRoleRepository  userRoleRepository)
        {
            _userRoleRepository = userRoleRepository;
        }
        // POST: Enquiry/Create
        [AllowAnonymous]
        [HttpPost]
        [Route("Create")]
        public IHttpActionResult CreateUserRole([FromBody] Models.UserRoleModel  userRoleModel)
        {
            try
            {
                var _data = new UserRoleDTO
                {
					Description = userRoleModel.Description,
					IdUserRole = userRoleModel.IdUserRole,
					IsActive = userRoleModel.IsActive,
					Name = userRoleModel.Name,
                    Deleted = false

                };
                var res = _userRoleRepository.CreateUserRole(_data);
                return WebResult(res);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
        }

       // [Authorize]
        [HttpGet]
        [Route("Detail")]
        public IHttpActionResult GetAllUserRole(string searchText = "")
        {
            try
            {
                var res = _userRoleRepository.GetAllUserRole(searchText);
                return WebResult(res);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
        }

        [Authorize]
        [HttpGet]
        [Route("Detail/{idUserRole}")]
        public IHttpActionResult GetAllEnquiry(int idUserRole)
        {
            try
            {
                var res = _userRoleRepository.GetAllUserRoleById(idUserRole);
                return WebResult(res);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
        }

        [Authorize]
        [HttpDelete]
        [Route("Detail/{idUserRole}/{isActive}")]
        public IHttpActionResult DeleteEnquiry(int idUserRole, bool isActive)
        {
            try
            {
                var res = _userRoleRepository.Delete(idUserRole, isActive);
                return WebResult(res);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
        }
        /// <summary>
        /// Delete 
        /// </summary>
        /// <param name="idUserRole"></param>
        /// <param name="isActive"></param>
        /// <returns></returns>

        [Authorize]
        [HttpDelete]
        [Route("Delete/{idUserRole}")]
        public IHttpActionResult DeleteUserRole(int idUserRole)
        {
            try
            {
                var res = _userRoleRepository.DeleteUserRole(idUserRole);
                return WebResult(res);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
        }

        [Authorize]
        [HttpGet]
        [Route("GetAllUsersByRole")]
        public IHttpActionResult GetAllUserRole(int RoleId)
        {
            try
            {
                var res = _userRoleRepository.GetAllUsersByRole(RoleId);
				if (res != null && res?.Object != null)
				{
                    foreach (var item in res.Object)
					if (item.ProfileImageUrl != string.Empty && item.ProfileImageUrl != null)
					{
						Match url = Regex.Match(item.ProfileImageUrl, @"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?");
						if (url.Length == 0)
							item.ProfileImageUrl = Path.Combine((Path.Combine(System.Configuration.ConfigurationManager.AppSettings["HostedDomainName"].ToString() + "/Uploads/Image", "")), item.ProfileImageUrl != null ? item.ProfileImageUrl : "");
					}
				}
				return WebResult(res);
			}
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
        }


           [AllowAnonymous]
        [Route("Deleted/{idUserRole}")]
        [HttpPost]
        public IHttpActionResult DeletedStatus(int idUserRole, bool isDeleted)
        {
            try
            {
                var res = _userRoleRepository.UpdateDeletedStatus(idUserRole, isDeleted);
                return WebResult(res);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
        }

    }
}