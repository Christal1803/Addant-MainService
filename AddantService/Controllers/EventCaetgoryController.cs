using AddantSDAL.DAL;
using AddantSDAL.DTO;
using AddantService.Helper;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace AddantService.Controllers
{
    [RoutePrefix("api/EventCategory")]
    public class EventCaetgoryController : BaseController
    {

        public IEventCategoryRepository _eventCategoryRepository;
        public EventCaetgoryController(IEventCategoryRepository  eventCategoryRepository)
        {
            _eventCategoryRepository = eventCategoryRepository;
        }
        // POST: Enquiry/Create
        [Authorize]
        [HttpPost]
        [Route("Create")]
        public IHttpActionResult CreateEventCategory([FromBody] Models.EventCategoryModel enquiryModel)
        {
            try
            {
                var _data = new EventCategoryDTO
                {
					Description = enquiryModel.Description,
					IdEventCategory = enquiryModel.IdEventCategory,
					IsActive = enquiryModel.IsActive,
					Name = enquiryModel.Name,
					BannerImgUrl = enquiryModel.BannerImgUrl,
					IdMainCategory = enquiryModel.IdMainCategory,
                    Deleted=false
                    
				};
                var res = _eventCategoryRepository.CreateEventCategory(_data);
              

                return WebResult(res);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("Detail")]
        public IHttpActionResult GetAllEventCategory(bool isAdminCall = false, bool isMaincategory = false)
        {
            try
            {
                var res = _eventCategoryRepository.GetAllEventCategory(isAdminCall,isMaincategory);
                if (res?.Object != null)
                {
                    foreach (var item in res.Object)
                    {
                        if (!string.IsNullOrEmpty(item.BannerImgUrl))
                        {
                            Match url = Regex.Match(item.BannerImgUrl, @"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?");
                            if (url.Length == 0)
                                item.BannerImgUrl = Path.Combine((Path.Combine(ConfigurationManager.AppSettings["HostedDomainName"].ToString() + "/Uploads/Category", "")), item.BannerImgUrl != null ? item.BannerImgUrl : "");
                        }
                    }

                }
                return WebResult(res);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("Detail/{idEventCategory}")]
        public IHttpActionResult GetAllEventCategoryById(int idEventCategory = 0 )
        {
            try
            {
                var res = _eventCategoryRepository.GetAllEventCategoryById(idEventCategory);
                if (res?.Object != null)
                {
                    if (!string.IsNullOrEmpty(res.Object.BannerImgUrl))
                    {
                        Match url = Regex.Match(res.Object.BannerImgUrl, @"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?");
                        if (url.Length == 0)
                            res.Object.BannerImgUrl = Path.Combine((Path.Combine(ConfigurationManager.AppSettings["HostedDomainName"].ToString() + "/Uploads/Category", "")), res.Object.BannerImgUrl != null ? res.Object.BannerImgUrl : "");
                    }
                }
                return WebResult(res);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("SubCategory/{IdMainCategory}")]
        public IHttpActionResult GetAllEventByIdMainCategory(int IdMainCategory = 0)
        {
            try
            {
                var res = _eventCategoryRepository.GetAllEventByIdMainCategory(IdMainCategory);
                if (res?.Object != null)
                {
                    if (!string.IsNullOrEmpty(res.Object.BannerImgUrl))
                    {
                        Match url = Regex.Match(res.Object.BannerImgUrl, @"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?");
                        if (url.Length == 0)
                            res.Object.BannerImgUrl = Path.Combine((Path.Combine(ConfigurationManager.AppSettings["HostedDomainName"].ToString() + "/Uploads/Category", "")), res.Object.BannerImgUrl != null ? res.Object.BannerImgUrl : "");
                    }
                }
                else
                {
                    GetAllEventCategoryById(IdMainCategory);
                }
                return WebResult(res);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
        }

        [Authorize]
        [HttpDelete]
        [Route("Detail/{idEventCategory}/{isDelete}")]
        public IHttpActionResult DeleteEnquiry(int idEventCategory, bool isDelete)
        {
            try
            {
                var res = _eventCategoryRepository.Delete(idEventCategory, isDelete);
                return WebResult(res);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
        }


        [Authorize]
        [Route("Upload/BannerImage")]
        public async Task<bool> UploadEmailHeader()
        {
            try
            {
                string ImgName = string.Empty;
                var dicFileUpload = new Dictionary<string, string>();
                EventCategoryDTO emailTemplateDTO = new EventCategoryDTO();
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
                if (files?.Count > 0)
                {
                    HttpContent file1 = provider.Files?.ElementAt(0);
                    // if (item.Headers.ContentDisposition.Name.Trim('\"') == formDataName)
                    if (file1 != null)
                    {
                        var thisFileName = file1?.Headers.ContentDisposition.FileName.Trim('\"');
                        var fileType = Path.GetExtension(thisFileName?.ToString());

                        Stream input = await file1.ReadAsStreamAsync();
                        string imagePath = string.Empty;
                        string folderPath = string.Empty;

                        if (formData["UploadedDocs"] == "UploadedDocs" && !string.IsNullOrEmpty(fileType))
                        {
                            folderPath = HttpContext.Current.Server.MapPath("~/Uploads/Category");
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
                            dicFileUpload.Add(file1.Headers.ContentDisposition.Name.Trim('\"'), ImgName);
                        }
                    }
                }
                var resOuter = _eventCategoryRepository.GetAllEventCategoryById(Convert.ToInt32(string.IsNullOrEmpty(formData["IdEventCategory"]) ? "0" : formData["IdEventCategory"]));
                if (resOuter?.Object != null)
                    if (dicFileUpload != null)
                    {
                        if (!string.IsNullOrEmpty(formData["IdEventCategory"]))
                            emailTemplateDTO.IdEventCategory = Convert.ToInt32(string.IsNullOrEmpty(formData["IdEventCategory"]) ? "0" : formData["IdEventCategory"]);
                        if (dicFileUpload.ContainsKey("BannerImgUrl"))
                            resOuter.Object.BannerImgUrl = dicFileUpload["BannerImgUrl"].Contains("http") ? string.Empty : dicFileUpload["BannerImgUrl"];


                        var rest = _eventCategoryRepository.CreateEventCategory(resOuter.Object);
                    }
                return true;
            }
            catch (Exception ex) { Logger.WriteLog("Inside UploadAddantFile():" + ex.Message.ToString()); return false; }
        }


		[AllowAnonymous]
		[HttpGet]
		[Route("EventCategoryByCategoryId/{idEventCategory}")]
		public IHttpActionResult EventCategoryByCategoryId(int idEventCategory = 0)
		{
			try
			{
				var res = _eventCategoryRepository.EventCategoryByCategory(idEventCategory);
                foreach (var item in res.Object)
                {
                    if (!string.IsNullOrEmpty(item.BannerImgUrl))
                    {
                        Match url = Regex.Match(item.BannerImgUrl, @"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?");
                        if (url.Length == 0)
                            item.BannerImgUrl = Path.Combine((Path.Combine(ConfigurationManager.AppSettings["HostedDomainName"].ToString() + "/Uploads/Category", "")), item.BannerImgUrl != null ? item.BannerImgUrl : "");
                    }
                }
                return WebResult(res);
			}
			catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
		}
	}
}