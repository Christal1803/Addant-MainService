using AddantSDAL.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using AddantSDAL.DTO;
using System.Net.Mail;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Net.Http;
using AddantService.Helper;
using System.Net.Http.Headers;
using System.Configuration;
using System.Text.RegularExpressions;

namespace AddantService.Controllers
{
    [RoutePrefix("api/Candidate")]
    public class CandidateController : BaseController
    {
        ICandidateRepository _candidateRepository;
        IEmailTemplateRepository _emailTemplateRepository;
        public CandidateController(ICandidateRepository candidateRepository, IEmailTemplateRepository emailTemplateRepository)
        {
            _candidateRepository = candidateRepository;
            _emailTemplateRepository = emailTemplateRepository;
        }

        [AllowAnonymous]
        [Route("compose")]
        public IHttpActionResult ComposeCandidate([FromBody] Models.CandidateModel candidateModel)
        {
            try
            {
                CandidateDTO candidateDTO = new CandidateDTO
                {
                    CreatedOn = candidateModel.CreatedOn,
                    Description = candidateModel.Description,
                    Email = candidateModel.Email,
                    FirstName = candidateModel.FirstName,
                    IdCandidate = candidateModel.IdCandidate,
                    IdPosition = candidateModel.IdPosition,
                    IsDeleted = candidateModel.IsDeleted,
                    LastName = candidateModel.LastName,
                    Mobile = candidateModel.Mobile,
                    ResumeUrl = candidateModel.ResumeUrl,
                    Status = candidateModel.Status,
                   
                    Deleted = false,

                };
                if (candidateModel.resumeContent != null)
                    candidateDTO.ResumeUrl = FileUpload.UploadResume(candidateModel.resumeContent, candidateModel.format);
                var res = _candidateRepository.Create(candidateDTO);
                if (res != null)
                    if (!string.IsNullOrEmpty(candidateDTO.Status))
                    {
                        var emailField = new Email.EmailField
                        {
                            UserName = candidateDTO.FirstName,
                            Position = candidateDTO.Position,
                            ToMail = candidateDTO.Email
                        };
                        switch (candidateDTO.Status)
                        {
                            case "ShortListed":
                                {
                                    emailField.TemplatePath = "/CandidateShortListed.html";
                                    emailField.Subject = "You are Short listed!";
                                    var templateCandidate = _emailTemplateRepository.GetAllEmailTemplateById(0, "CandidateShortListed")?.Object;
                                    Logger.WriteLog($"CandidateShortListed Mail template value -{templateCandidate}");
                                    emailField.Body = templateCandidate?.Body;
                                    if (!string.IsNullOrEmpty(templateCandidate?.HeaderImageUrl))
                                    {
                                        Match url = Regex.Match(templateCandidate?.HeaderImageUrl, @"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?");
                                        if (url.Length == 0)
                                            emailField.HeaderImageUrl = Path.Combine((Path.Combine(ConfigurationManager.AppSettings["HostedDomainName"].ToString() + "/Uploads/EmailHeader", "")), templateCandidate?.HeaderImageUrl != null ? templateCandidate?.HeaderImageUrl : "");
                                    }
                                    if (templateCandidate != null)
                                        Email.SendCandidateEmail(emailField);
                                }
                                break;
                            case "Rejected":
                                {
                                    emailField.TemplatePath = "/CandidateRejected.html";
                                    emailField.Subject = "Not this time!";
                                    var templateCandidate = _emailTemplateRepository.GetAllEmailTemplateById(0, "CandidateRejected")?.Object;
                                    Logger.WriteLog($"CandidateRejected Mail template value -{templateCandidate}");
                                    emailField.Body = templateCandidate?.Body;
                                    if (!string.IsNullOrEmpty(templateCandidate?.HeaderImageUrl))
                                    {
                                        Match url = Regex.Match(templateCandidate?.HeaderImageUrl, @"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?");
                                        if (url.Length == 0)
                                            emailField.HeaderImageUrl = Path.Combine((Path.Combine(ConfigurationManager.AppSettings["HostedDomainName"].ToString() + "/Uploads/EmailHeader", "")), templateCandidate?.HeaderImageUrl != null ? templateCandidate?.HeaderImageUrl : "");
                                    }
                                    if (templateCandidate != null)
                                        Email.SendCandidateEmail(emailField);
                                }
                                break;
                            case "Processing":
                                {

                                    emailField.TemplatePath = "/ThankCanidadate.html";
                                    emailField.Subject = "Thank you for applying!";
                                    emailField.ToMail = res != null ? res.Object.Email : "";
                                    var templateCandidate = _emailTemplateRepository.GetAllEmailTemplateById(0, "ThankCanidadate")?.Object;
                                    Logger.WriteLog($"ThankCanidadate Mail template value -{templateCandidate}");
                                    emailField.Body = templateCandidate?.Body;
                                    if (!string.IsNullOrEmpty(templateCandidate?.HeaderImageUrl))
                                    {
                                        Match url = Regex.Match(templateCandidate?.HeaderImageUrl, @"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?");
                                        if (url.Length == 0)
                                            emailField.HeaderImageUrl = Path.Combine((Path.Combine(ConfigurationManager.AppSettings["HostedDomainName"].ToString() + "/Uploads/EmailHeader", "")), templateCandidate?.HeaderImageUrl != null ? templateCandidate?.HeaderImageUrl : "");
                                    }
                                    if (templateCandidate != null)
                                        Email.SendCandidateEmail(emailField);
                                    emailField.TemplatePath = "/NotificationJobEnquiries.html";
                                    emailField.Subject = "New job inquiry";
                                    emailField.UserName = res?.Object?.FirstName + " " + res?.Object?.LastName;
                                    emailField.ToMail = ConfigurationManager.AppSettings["NotificationToAddantEmail"];
                                    emailField.ResumeUrl = res?.Object?.ResumeUrl;
                                    var templateCandidate2 = _emailTemplateRepository.GetAllEmailTemplateById(0, "NotificationJobEnquiries")?.Object;
                                    Logger.WriteLog($"NotificationJobEnquiries Mail template value -{templateCandidate}");
                                    emailField.Body = templateCandidate?.Body;
                                    if (!string.IsNullOrEmpty(templateCandidate2?.HeaderImageUrl))
                                    {
                                        Match url = Regex.Match(templateCandidate2?.HeaderImageUrl, @"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?");
                                        if (url.Length == 0)
                                            emailField.HeaderImageUrl = Path.Combine((Path.Combine(ConfigurationManager.AppSettings["HostedDomainName"].ToString() + "/Uploads/EmailHeader", "")), templateCandidate2?.HeaderImageUrl != null ? templateCandidate2?.HeaderImageUrl : "");
                                    }
                                    if (templateCandidate != null)
                                        Email.SendCandidateEmail(emailField);
                                }
                                break;
                        }
                    }
                return WebResult(res);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
        }

        //[Authorize]
        [Route("")]
        public IHttpActionResult GetAllCandidate(DateTime? startDate = null, DateTime? endDate = null, bool isAdminCall = false, string searchText = "", string status = "")
        {
            try
            {
                var res = _candidateRepository.GetAllCandidate(startDate, endDate, isAdminCall, searchText, status);
                if (res.Object.Count > 0)
                {
                    foreach (var item in res.Object)
                    {
                        if (!string.IsNullOrEmpty(item.ResumeUrl))
                        {
                            Match url = Regex.Match(item.ResumeUrl, @"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?");
                            if (url.Length == 0)
                                item.ResumeUrl = Path.Combine((Path.Combine(ConfigurationManager.AppSettings["HostedDomainName"].ToString() + "/Uploads/Resume", "")), item.ResumeUrl != null ? item.ResumeUrl : "");
                        }
                    }
                }
                return WebResult(res);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
        }

        [AllowAnonymous]
        [Route("DocumentUpload/MediaUpload")]
        public async Task<bool> UploadAddantFile()
        {
            try
            {
                string ImgName = string.Empty; string resumeUrl = "";
                var dicFileUpload = new Dictionary<string, string>();
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
                var reslt = _candidateRepository.GetCandidateById(Convert.ToInt32(formData["IdCandidate"]));
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
                                folderPath = HttpContext.Current.Server.MapPath("~/Uploads/Resume");
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
                                if (!string.IsNullOrEmpty(fileType))
                                    dicFileUpload.Add(item.Headers.ContentDisposition.Name.Trim('\"'), ImgName);
                            }
                        }
                    }
                    if (reslt.Object != null)
                        if (dicFileUpload != null)
                        {
                            CandidateDTO addantLifeDTO = reslt.Object;
                            addantLifeDTO.IdCandidate = Convert.ToInt32(formData["IdCandidate"]);
                            if (dicFileUpload.ContainsKey("ResumeUrl"))
                                addantLifeDTO.ResumeUrl = dicFileUpload["ResumeUrl"].Contains("http") ? "" : dicFileUpload["ResumeUrl"];
                            resumeUrl = addantLifeDTO.ResumeUrl;
                            reslt = _candidateRepository.Create(addantLifeDTO);


                        }
                }
                var emailField = new Email.EmailField
                {
                    UserName = reslt.Object?.FirstName + " " + reslt.Object?.LastName,
                    Position = reslt.Object?.Position,
                    Link = ConfigurationManager.AppSettings["AdminPortalUrl"].ToString()
                };
                return true;
            }
            catch (Exception ex) { return false; }
        }
        public HttpResponseMessage GetAllBlogImage(int BlogId = 0)
        {
            var content = new MultipartContent();
            var ids = new List<int>() { 1, 2, 3 };
            if (BlogId > 0)
            {
                var blogDTOs = _candidateRepository.GetCandidateById(BlogId);
                if (blogDTOs != null)
                {
                    ids = new List<int> { blogDTOs.Object.IdCandidate };

                    var objectContent = new ObjectContent<List<int>>(ids, new System.Net.Http.Formatting.JsonMediaTypeFormatter());
                    content.Add(objectContent);
                    //blog list here 
                    if (blogDTOs.Object != null)
                    {
                        var file1Content = new StreamContent(new FileStream(Path.Combine(HttpContext.Current.Server.MapPath("~/Uploads/Resume"), blogDTOs.Object.ResumeUrl), FileMode.Open));
                        file1Content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("image/jpeg");
                        file1Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = blogDTOs.Object.ResumeUrl };
                        file1Content.Headers.ContentDisposition.Name = "ResumeUrl";
                        content.Add(file1Content);
                    }
                }
            }
            else
            {
                var blogDTOs = _candidateRepository.GetAllCandidate(null, null, true);
                ids = blogDTOs?.Object.Select(x => x.IdCandidate).ToList();

                var objectContent = new ObjectContent<List<int>>(ids, new System.Net.Http.Formatting.JsonMediaTypeFormatter());
                content.Add(objectContent);
                //blog list here 
                if (blogDTOs.Object != null)
                {
                    foreach (var item in blogDTOs.Object)
                    {
                        var file1Content = new StreamContent(new FileStream(Path.Combine(HttpContext.Current.Server.MapPath("~/Uploads/Resume"), item.ResumeUrl), FileMode.Open));
                        file1Content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("image/jpeg");
                        file1Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = item.ResumeUrl };
                        file1Content.Headers.ContentDisposition.Name = "ResumeUrl";
                        content.Add(file1Content);
                    }
                }
            }


            var response = new HttpResponseMessage();
            response.Content = content;
            return response;
        }

        [Authorize]
        [Route("Detail/{IdCandidate}")]
        [HttpDelete]
        public IHttpActionResult DisableCandidate(int IdCandidate, bool isDeleted = false)
        {
            var res = _candidateRepository.DisableCandidate(IdCandidate, isDeleted);
            return WebResult(res);
        }

        [Authorize]
        [Route("Detail/{candidateId}")]        
        public IHttpActionResult GetCandidateById(int candidateId)
        {
            try
            {
                var res = _candidateRepository.GetCandidateById(candidateId);
                if (res.Object != null)
                {
                    if (!string.IsNullOrEmpty(res.Object.ResumeUrl))
                    {
                        Match url = Regex.Match(res.Object.ResumeUrl, @"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?");
                        if (url.Length == 0)
                            res.Object.ResumeUrl = Path.Combine((Path.Combine(ConfigurationManager.AppSettings["HostedDomainName"].ToString() + "/Uploads/Resume", "")), res.Object.ResumeUrl != null ? res.Object.ResumeUrl : "");
                    }

                }
                return WebResult(res);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
        }

        [Authorize]
        [Route("Delete/{IdCandidate}")]
        [HttpDelete]
        public IHttpActionResult Delete(int IdCandidate)
        {
            var res = _candidateRepository.Delete(IdCandidate);
            return WebResult(res);
        }

        [AllowAnonymous]
        [Route("Detail/CandidateByPositionId/{positionId}")]
        [HttpGet]
        public IHttpActionResult GetCandidateByPositionId(int positionId)
        {
            try
            {
                var res = _candidateRepository.GetCandidateByPositionId(positionId);
                if (res.Object != null)
                {
                    var candidateList = res.Object;
                    foreach (var candidate in candidateList)
                    {
                        if (!string.IsNullOrEmpty(candidate.ResumeUrl))
                        {
                            Match url = Regex.Match(candidate.ResumeUrl, @"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?");
                            if (url.Length == 0)
                                candidate.ResumeUrl = Path.Combine((Path.Combine(ConfigurationManager.AppSettings["HostedDomainName"].ToString() + "/Uploads/Resume", "")), candidate.ResumeUrl != null ? candidate.ResumeUrl : "");
                        }
                    }
                    res.Object = new List<CandidateDTO>(candidateList);

                }
                return WebResult(res);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
        }



        [AllowAnonymous]
        [Route("Deleted/{IdCandidate}")]
        [HttpPost]
        public IHttpActionResult DeletedStatus(int IdCandidate, bool isDeleted)
        {
            try
            {
                var res = _candidateRepository.UpdateDeletedStatus(IdCandidate, isDeleted);
                return WebResult(res);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
        }
    }
}