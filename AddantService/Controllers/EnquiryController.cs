using AddantSDAL.DAL;
using AddantSDAL.DTO;
using Newtonsoft.Json;
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
    [RoutePrefix("api/Enquiry")]
    public class EnquiryController : BaseController
    {
        public IEnquiryRepository _enquiryRepository;
        public IEmailTemplateRepository _emailTemplateRepository;
        public EnquiryController(IEnquiryRepository enquiryRepository , IEmailTemplateRepository emailTemplateRepository)
        {
            _enquiryRepository = enquiryRepository;
            _emailTemplateRepository = emailTemplateRepository;
        }
        // POST: Enquiry/Create
        [AllowAnonymous]
        [HttpPost]
        [Route("Create")]
        public IHttpActionResult CreateEnquiry([FromBody] Models.EnquiryModel enquiryModel)
        {
            try
            {
                var _data = new EnquiryDTO {
                    Email = enquiryModel.Email,
                    FirstName = enquiryModel.FirstName,
                    IdEnquiry = enquiryModel.IdEnquiry,
                    LastName = enquiryModel.LastName,
                    Message = enquiryModel.Message,
                    Mobile = enquiryModel.Mobile,
                    Status = enquiryModel.Status,
                    Subject = enquiryModel.Subject,
                    CreatedDate = enquiryModel.CreatedDate,
                    CategoryId = enquiryModel.CategoryId,
                    Deleted = false // default

                };
                var res = _enquiryRepository.CreateEnquiry(_data);
                if (res != null)
                {
                    Logger.WriteLog($"CreateEnquiry Case1  Creation response - : {JsonConvert.SerializeObject(res)}");
                    if (enquiryModel.IdEnquiry == 0)
                    {
                        Logger.WriteLog($"CreateEnquiry Case1.1");
                       var emailField = new Email.EmailField
                        {
                            UserName = _data.FirstName + " " + _data.LastName,
                            Mail = res.Object.Email
                        };

                        emailField.Subject = "Thanks for reaching us!";
                        emailField.TemplatePath = "/AcknowledgeEnquiry.html";
                        emailField.ToMail = _data.Email;
                        // get template
                        var emailTemplateDTO = _emailTemplateRepository.GetAllEmailTemplateById(0, "AcknowledgeEnquiry")?.Object;
                        Logger.WriteLog($"Before entering to Send Email. Getting all acknowledge email template by ID value -{emailTemplateDTO}");
                        if (emailTemplateDTO != null)
                        {
                            Logger.WriteLog($"Template not null. Getting all email template by ID value -{emailTemplateDTO}");
                            emailField.Body = emailTemplateDTO?.Body;
                            if (!string.IsNullOrEmpty(emailTemplateDTO?.HeaderImageUrl))
                            {
                                Match url = Regex.Match(emailTemplateDTO?.HeaderImageUrl, @"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?");
                                if (url.Length == 0)
                                    emailField.HeaderImageUrl = Path.Combine((Path.Combine(ConfigurationManager.AppSettings["HostedDomainName"].ToString() + "/Uploads/EmailHeader", "")), emailTemplateDTO?.HeaderImageUrl != null ? emailTemplateDTO?.HeaderImageUrl : "");
                            }
                            if (emailTemplateDTO != null)
                                Email.SendEnquiryEmail(emailField);
                        }


                        emailField.Subject = "Hey there is a new inquiry!";
                        emailField.TemplatePath = "/NotificationBusinessEnquiries.html";
                        var templateCandidate = _emailTemplateRepository.GetAllEmailTemplateById(0, "NotificationBusinessEnquiries")?.Object;
                        Logger.WriteLog($"Before entering to Send Email. Getting all email template by ID value -{templateCandidate}");
                        if (templateCandidate != null)
                        {
                            Logger.WriteLog($"Template not null. Getting all email template by ID value -{templateCandidate}");
                            emailField.Body = templateCandidate?.Body;
                            if (!string.IsNullOrEmpty(templateCandidate?.HeaderImageUrl))
                            {
                                Match url = Regex.Match(templateCandidate?.HeaderImageUrl, @"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?");
                                if (url.Length == 0)
                                    emailField.HeaderImageUrl = Path.Combine((Path.Combine(ConfigurationManager.AppSettings["HostedDomainName"].ToString() + "/Uploads/EmailHeader", "")), templateCandidate?.HeaderImageUrl != null ? templateCandidate?.HeaderImageUrl : "");
                            }
                            emailField.ToMail = ConfigurationManager.AppSettings["NotificationToAddantEmail"];
                            emailField.Mail = _data.Email;
                            emailField.Link = ConfigurationManager.AppSettings["AdminPortalUrl"];
                            if (emailTemplateDTO != null)
                                Email.SendEnquiryEmail(emailField);
                        }
                    }
                    else
                        Logger.WriteLog($"CreateEnquiry Case1.2");
                }
                else
                    Logger.WriteLog($"CreateEnquiry Case1  Creation response null");

                return WebResult(res); 
            }
            catch (Exception ex) 
            {
                Logger.WriteLog(ex.Message.ToString());
                Logger.WriteLog($"CreateEnquiry exception {JsonConvert.SerializeObject(ex)}");
                return null;
            }
        }

       //[Authorize]
        [HttpGet]
        [Route("Detail")]
        public IHttpActionResult GetAllEnquiry(DateTime? startDate=null, DateTime? endDate=null, string searchText = "", bool isAdminCall = false, string status= "")  
        {
            try
            {
                var res = _enquiryRepository.GetAllEnquiry(startDate, endDate, searchText, isAdminCall,status);
                return WebResult(res);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
        }

        [Authorize]
        [HttpGet]
        [Route("Detail/{idEnquiry}")]
        public IHttpActionResult GetAllEnquiry(int idEnquiry)
        {
            try
            {
                var res = _enquiryRepository.GetAllEnquiryById(idEnquiry);
                return WebResult(res);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
        }

        [Authorize]
        [HttpDelete]
        [Route("Detail/{idEnquiry}/{isDelete}")]
        public IHttpActionResult DeleteEnquiry(int idEnquiry, bool isDelete)
        {
            try
            {
                var res = _enquiryRepository.Delete(idEnquiry,isDelete);
                return WebResult(res);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
        }

        ///Getiing Enquiries by Category Id 
        ///

        [Authorize]
        [HttpGet]
        [Route("Categorywise/{CategoryId}")]
        public IHttpActionResult GetAllEnquiriesByCategory(int CategoryId)
        {
            try
            {
                var res = _enquiryRepository.GetAllEnquiriesByCategory(CategoryId);
                return WebResult(res);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
        }

        ///Getiing EnquiriesCategories 
        ///
        [AllowAnonymous]
        [HttpGet]
        [Route("Categories")]
        public IHttpActionResult GetAllEnquiryCategory()
        {
            try
            {
                var res = _enquiryRepository.GetAllEnquiryCategories();
                return WebResult(res);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
        }

        /////
        ///Delete Enquiry 
        ///setting up delete flag to false 
        /// <summary>
        /// Delete Enquiry 
        /// </summary>
        /// <param name="enquiryId"></param>
        /// <returns></returns>

        [Authorize]
        [Route("Delete/{blogId}")]
        [HttpDelete]
        public IHttpActionResult Delete(int enquiryId)
        {
            try
            {
                 var res = _enquiryRepository.Delete(enquiryId);
                return WebResult(res);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
        }


		[Authorize]
		[Route("GetAllEnquiry")]
		[HttpGet]
		public IHttpActionResult GetAllEnquiry()
		{
			try
			{
				var res = _enquiryRepository.GetAllEnquiries();
				return WebResult(res);
			}
			catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
		}


        [AllowAnonymous]
        [Route("Deleted/{enquiryId}")]
        [HttpPost]
        public IHttpActionResult DeletedStatus(int enquiryId, bool isDeleted)
        {
            try
            {
                var res = _enquiryRepository.UpdateDeletedStatus(enquiryId, isDeleted);
                return WebResult(res);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
        }

    }
}
