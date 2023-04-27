using AddantSDAL.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using AddantSDAL.DTO;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Net.Http;
using AddantService.Helper;
using System.Net.Http.Headers;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Security.Policy;

namespace AddantService.Controllers
{
    [RoutePrefix("api/User")]
    public class UserController : BaseController
    {
        IUserRepository _userRepository;
        IEmailTemplateRepository _emailTemplateRepository;
        public UserController(IUserRepository userRepository, IEmailTemplateRepository emailTemplateRepository)
        {
            _userRepository = userRepository;
            _emailTemplateRepository = emailTemplateRepository;
        }

        //[Authorize]
        [HttpPost]
        [Route("compose")]
        public IHttpActionResult Create([FromBody] Models.UserModel userModel)
        {
            try
            {
                UserDTO userDTO = new UserDTO
                {
                    CreatedOn = userModel.CreatedOn != null ? userModel.CreatedOn : DateTime.Now,
                    BloodGroup = userModel.BloodGroup,
                    Designation = userModel.Designation,
                    Dob = userModel.Dob,
                    Email = userModel.Email,
                    EmployeeID = userModel.EmployeeID,
                    FirstName = userModel.FirstName,
                    IsActive = userModel.IsActive,
                    LastName = userModel.LastName,
                    Mobile = userModel.Mobile,
                    Password = userModel.Password,
                    ProfileImageUrl = userModel.ProfileImageUrl,
                    Role = userModel.Role,
                    UserId = string.IsNullOrEmpty(userModel.UserId.ToString()) ? System.Guid.NewGuid() : userModel.UserId,
                    Username = userModel.Username,
                    IdUserRole = userModel.IdUserRole,
                    Deleted=false
                    
                };
                var res = _userRepository.Create(userDTO);

                // For sending Email to New User
                if (res?.Object != null)
                {
                    if (userModel?.UserId == null)//not send email for update
                    {
                        var emailField = new Email.EmailField
                        {
                            UserName = res?.Object?.Email,
                            Title = res?.Object?.Username,
                            Password = res?.Object.Password,
                            Subject = "New User created successfully",
                            TemplatePath = "/UserCreation.html",
                            Link = "https://portal.addant.com/#/"
                        };
                        var templateCandidate = _emailTemplateRepository.GetAllEmailTemplateById(0, "UserCreation")?.Object;
                        if (templateCandidate != null)
                        {
                            emailField.Body = templateCandidate?.Body;
                            if (!string.IsNullOrEmpty(templateCandidate?.HeaderImageUrl))
                            {
                                Match url = Regex.Match(templateCandidate?.HeaderImageUrl, @"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?");
                                if (url.Length == 0)
                                    emailField.HeaderImageUrl = Path.Combine((Path.Combine(ConfigurationManager.AppSettings["HostedDomainName"].ToString() + "/Uploads/EmailHeader", "")), templateCandidate?.HeaderImageUrl != null ? templateCandidate?.HeaderImageUrl : "");
                            }
                            Email.SendUserEmail(emailField);
                        }
                    }
                }
                return WebResult(res);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
        }

        //[Authorize]
        [HttpGet]
        [Route("Detail")]
        public IHttpActionResult GetAllUser(DateTime? startDate = null, DateTime? endDate = null, bool isActive = true, string searchText = "", string status = "")
        {
            try
            {
                var res = _userRepository.GetAllUser(startDate, endDate, searchText, status, isActive);
                if (res?.Object?.Count > 0)
                {
                    foreach (var item in res.Object)
                    {
                        if (item.ProfileImageUrl != string.Empty && item.ProfileImageUrl != null)
                        {
                            Match url = Regex.Match(item.ProfileImageUrl, @"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?");
                            if (url.Length == 0)
                                item.ProfileImageUrl = Path.Combine((Path.Combine(System.Configuration.ConfigurationManager.AppSettings["HostedDomainName"].ToString() + "/Uploads/Image", "")), item.ProfileImageUrl != null ? item.ProfileImageUrl : "");
                        }
                    }
                }
                return WebResult(res);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
        }

        [Authorize]
        [Route("Detail")]
        [HttpDelete]
        public IHttpActionResult DisableUser(string IdUser, bool isActive = false)
        {
            var res = _userRepository.DisableUser(IdUser, isActive);
            return WebResult(res);
        }

        //[Authorize]
        [HttpGet]
        [Route("Detail/{userId}")]
        public IHttpActionResult GetUserById(string userId)
        {
            try
            {
                var res = _userRepository.GetUserById(userId);
                if (res != null)
                {
                    if (res.Object?.ProfileImageUrl != string.Empty && res.Object?.ProfileImageUrl != null)
                    {
                        Match url = Regex.Match(res.Object?.ProfileImageUrl, @"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?");
                        if (url.Length == 0)
                            res.Object.ProfileImageUrl = Path.Combine((Path.Combine(System.Configuration.ConfigurationManager.AppSettings["HostedDomainName"].ToString() + "/Uploads/Image", "")), res.Object?.ProfileImageUrl != null ? res.Object?.ProfileImageUrl : "");
                    }
                }
                return WebResult(res);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
        }

        [Authorize]
        [Route("DocumentUpload/MediaUpload")]
        public async Task<bool> UploadUserImage()
        {
            try
            {
                string ImgName = string.Empty;
                var dicFileUpload = new Dictionary<string, string>();
                UserDTO addantLifeDTO = new UserDTO();
                // Check if the request contains multipart/form-data.  
                if (!Request.Content.IsMimeMultipartContent())
                {
                    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
                }

                var provider = await Request.Content.ReadAsMultipartAsync<InMemoryMultipartFormDataStreamProvider>(new InMemoryMultipartFormDataStreamProvider());
                //access form data  
                NameValueCollection formData = provider.FormData;
                //access files  
                IList<HttpContent> files = provider.Files;
                HttpContent file1 = null;
                if (files.Count > 0)
                {
                    foreach (var item in files)
                    {
                        // if (item.Headers.ContentDisposition.Name.Trim('\"') == formDataName)
                        file1 = item;

                        if (file1 != null)
                        {
                            var thisFileName = file1?.Headers.ContentDisposition.FileName.Trim('\"');
                            var fileType = Path.GetExtension(thisFileName?.ToString());

                            Stream input = await file1.ReadAsStreamAsync();
                            string imagePath = string.Empty;
                            string folderPath = string.Empty;

                            if (formData["UploadedDocs"] == "UploadedDocs" && !string.IsNullOrEmpty(fileType))
                            {
                                folderPath = HttpContext.Current.Server.MapPath("~/Uploads/Image");
                                ImgName = "Img" + Guid.NewGuid() + fileType.ToString();
                                imagePath = folderPath + "/" + ImgName;

                                //Deletion exists file  
                                if (File.Exists(imagePath))
                                {
                                    File.Delete(imagePath);
                                }

                                if (!Directory.Exists(folderPath))
                                {
                                    Directory.CreateDirectory(folderPath);
                                }
                                using (Stream file = File.OpenWrite(imagePath))
                                {
                                    input.CopyTo(file);
                                    file.Close();
                                }
                                dicFileUpload.Add(item.Headers.ContentDisposition.Name.Trim('\"'), ImgName);
                            }
                        }
                    }
                    var res = _userRepository.GetUserById(formData["UserId"]);
                    if (res.Object != null)
                        if (dicFileUpload != null)
                        {
                            addantLifeDTO = res.Object;
                            if (dicFileUpload.ContainsKey("ProfileImageUrl"))
                                addantLifeDTO.ProfileImageUrl = dicFileUpload["ProfileImageUrl"].Contains("http") ? string.Empty : dicFileUpload["ProfileImageUrl"];

                            var rest = _userRepository.UploadImage(addantLifeDTO);
                        }
                }
                return true;
            }
            catch (Exception ex) { Logger.WriteLog("Inside UploadAddantFile():" + ex.Message.ToString()); return false; }
        }

        [Authorize]
        [Route("ChangePassword")]
        [HttpPost]
        public IHttpActionResult ChangePassword([FromBody] Models.UserResetModel userModel)
        {
            //time being user password iis assigned to lastname
            UserDTO userDTO = new UserDTO
            {
                Username = userModel.Username,
                Password = userModel.Password,
                LastName = userModel.NewPassword
            };

            var res = _userRepository.ChangePassword(userDTO);
            return WebResult(res);
        }

        [Authorize]
        [Route("Delete")]
        [HttpDelete]
        public IHttpActionResult Delete(string IdUser)
        {
            var res = _userRepository.Delete(IdUser);
            return WebResult(res);
        }

        [AllowAnonymous]
        [Route("Deleted/{idUser}")]
        [HttpPost]
        public IHttpActionResult DeletedStatus(Guid idUser, bool isDeleted)
        {
            try
            {
                var res = _userRepository.UpdateDeletedStatus(idUser, isDeleted);
                return WebResult(res);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
        }
    }
}