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
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Net.Http.Headers;

namespace AddantService.Controllers
{
    [RoutePrefix("api/EmailTemplate")]
    public class EmailTemplateController : BaseController
    {
        public IEmailTemplateRepository _emailTemplateRepository;
        public EmailTemplateController(IEmailTemplateRepository emailTemplateRepository)
        {
            _emailTemplateRepository = emailTemplateRepository;
        }
        // POST: Enquiry/Create
        //[Authorize]
        [HttpPost]
        [Route("Create")]
        public IHttpActionResult CreateEmailTemplate([FromBody] Models.EmailTemplateModel emailTemplateModel)
        {
            try
            {
                var _data = new EmailTemplateDTO
                {
                    Body = TextToHtml(emailTemplateModel.Body),
                    IdEmailTemplate = emailTemplateModel.IdEmailTemplate,
                    IdTemplateType = emailTemplateModel.IdTemplateType,
                    IsActive = emailTemplateModel.IsActive,
                    CreatedDate = emailTemplateModel.CreatedDate,
                    Deleted = false // default


                };
                var res = _emailTemplateRepository.CreateEmailTemplate(_data);
                return WebResult(res);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
        }

        //[Authorize]
        [HttpGet]
        [Route("Detail")]
        public IHttpActionResult GetAllEmailTemplate(DateTime? startDate = null, DateTime? endDate = null, string searchText = "", bool isAdminCall = false)
        {
            try
            {
                var res = _emailTemplateRepository.GetAllEmailTemplate(startDate, endDate, searchText,isAdminCall);
                if (res?.Object != null)
                {
                    foreach (var item in res.Object)
                    {
                        if (!string.IsNullOrEmpty(item.HeaderImageUrl))
                        {
                            Match url = Regex.Match(item.HeaderImageUrl, @"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?");
                            if (url.Length == 0)
                                item.HeaderImageUrl = Path.Combine((Path.Combine(ConfigurationManager.AppSettings["HostedDomainName"].ToString() + "/Uploads/EmailHeader", "")), item.HeaderImageUrl != null ? item.HeaderImageUrl : "");
                        }
                    }

                }
                return WebResult(res);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
        }

        [Authorize]
        [HttpGet]
        [Route("Detail/{idEmailTemplate}")]
        public IHttpActionResult GetAllEmailTemplate(int idEmailTemplate)
        {
            try
            {
                var res = _emailTemplateRepository.GetAllEmailTemplateById(idEmailTemplate);
                if (res?.Object != null)
                {
                    if (!string.IsNullOrEmpty(res.Object.HeaderImageUrl))
                    {
                        Match url = Regex.Match(res.Object.HeaderImageUrl, @"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?");
                        if (url.Length == 0)
                            res.Object.HeaderImageUrl = Path.Combine((Path.Combine(ConfigurationManager.AppSettings["HostedDomainName"].ToString() + "/Uploads/EmailHeader", "")), res.Object.HeaderImageUrl != null ? res.Object.HeaderImageUrl : "");
                    }
                    if (res?.Object != null)
                    {
                        res.Object.Body = HtmlToText(res?.Object.Body);
                    }

                }
                return WebResult(res);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
        }

        [Authorize]
        [HttpDelete]
        [Route("Detail/{idEmailTemplate}/{isActive}")]
        public IHttpActionResult DeleteEmailTemplate(int idEmailTemplate, bool isActive)
        {
            try
            {
                var res = _emailTemplateRepository.Delete(idEmailTemplate, isActive);
                return WebResult(res);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
        }
        public static string TextToHtml(string text)
        {
            var sb = new StringBuilder();
            var sr = new StringReader(text);
            var str = "<h>" + sr.ReadLine() + "</h>";

            while (str != null)
            {
                str = str.TrimEnd();
                str.Replace("  ", " &nbsp;");
                if (str.Length > 80 && !str.Contains("<h>"))
                {
                    sb.AppendLine($"{str}<br>");
                }
                else if (str.Length > 0 && !str.Contains("<h>"))
                {
                    sb.AppendLine($"{str}<br>");
                }
                else if (str.Contains("<h>"))
                {
                    sb.AppendLine($"{str}<br>");
                }
                else if (str.Length == 0)
                {
                    sb.AppendLine($"<br>");
                }

                str = sr.ReadLine();
            }
            string strRes = "<p>" + sb.ToString() + "</p>";
            return strRes;
        }

        public static string HtmlToText(string html)
        {
            string res = string.Empty;
            try
            {
                var sb = new StringBuilder();
                var sr = new StringReader(html);
                var str =  sr.ReadLine();
                while (html != null)
                {
                    html = html.TrimEnd();
                    html.Replace(" ", "");
                    if (str != null)
                    {
                        if (str.Length > 80 && !str.Contains("<h>"))
                        {
                            sb.AppendLine($"{str.Replace("</br>", "").Replace("<br>", "")}");
                        }
                        else if (str.Length > 0 && !str.Contains("<h>"))
                        {
                            sb.AppendLine($"{str.Replace("</br>", "").Replace("<br>", "")}");
                        }
                        else if (str.Contains("<h>"))
                        {
                            sb.AppendLine($"{str.Replace("<h>", "").Replace("</h>", "").Replace("</br>", "").Replace("<br>", "")}");
                        }
                        else if (str.Contains("<br>"))
                        {
                            sb.AppendLine($"{str.Replace("<br>","").Replace("<br>", "")}");
                        }
                    }
                    else
                    {
                        res = sb.ToString().Replace("<p>", "").Replace("</p>", "").Replace("</br>", "").Replace("<br>", "");
                        break;
                    }
                    str = sr.ReadLine();
                }
                res = sb.ToString().Replace("<p>","").Replace("</p>","");
                return res;
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message); return string.Empty; }
        }

        [Authorize]
        [Route("Upload/EmailHeaderBackground")]
        public async Task<bool> UploadEmailHeader()
        {
            try
            {
                string ImgName = string.Empty;
                var dicFileUpload = new Dictionary<string, string>();
                EmailTemplateDTO emailTemplateDTO = new EmailTemplateDTO();
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
                            folderPath = HttpContext.Current.Server.MapPath("~/Uploads/EmailHeader");
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
                var resOuter = _emailTemplateRepository.GetAllEmailTemplateById(Convert.ToInt32(string.IsNullOrEmpty(formData["IdEmailTemplate"]) ? "0" : formData["IdEmailTemplate"]));
                if (resOuter?.Object != null)
                    if (dicFileUpload != null)
                    {
                       if (!string.IsNullOrEmpty(formData["IdEmailTemplate"]))
                            emailTemplateDTO.IdEmailTemplate = Convert.ToInt32(string.IsNullOrEmpty(formData["IdEmailTemplate"]) ? "0" : formData["IdEmailTemplate"]);
                        if (dicFileUpload.ContainsKey("HeaderImageUrl"))
                            resOuter.Object.HeaderImageUrl = dicFileUpload["HeaderImageUrl"].Contains("http") ? string.Empty : dicFileUpload["HeaderImageUrl"];
                  

                        var rest = _emailTemplateRepository.CreateEmailTemplate(resOuter.Object);
                    }
                return true;
            }
            catch (Exception ex) { Logger.WriteLog("Inside UploadAddantFile():" + ex.Message.ToString()); return false; }
        }

        //[Authorize]
        [HttpGet]
        [Route("TemplateType")]
        public IHttpActionResult GetAllEmailTemplate()
        {
            try
            {
                var res = _emailTemplateRepository.GetTemplatetype();
                return WebResult(res);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("Detail/FormData")]
        public HttpResponseMessage GetAllEmbedCode(DateTime? startDate = null, DateTime? endDate = null, bool isAdminCall = false, int emailTemplateById = 0)
        {
            var content = new MultipartContent();
            var emailTemplates =new  List<EmailTemplateDTO>();
            if(emailTemplateById > 0)
                emailTemplates.Add( _emailTemplateRepository.GetAllEmailTemplateById(emailTemplateById)?.Object);
            else
                emailTemplates = _emailTemplateRepository.GetAllEmailTemplate(startDate, endDate, "", isAdminCall).Object;

            if (emailTemplates != null)
            {
                foreach (var item in emailTemplates)
                {
                    var file4Content = new StringContent(item.IdEmailTemplate.ToString(), System.Text.Encoding.UTF8, "text/plain");
                    file4Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("IdEmailTemplate");
                    file4Content.Headers.ContentDisposition.Name = "IdEmailTemplate";
                    content.Add(file4Content);
                    var file2Content = new StringContent(HtmlToText(item.Body?.ToString()), System.Text.Encoding.UTF8, "text/plain");
                    file2Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("Body");
                    file2Content.Headers.ContentDisposition.Name = "Body";
                    content.Add(file2Content);
                    var file1Content = new StringContent(item.CreatedDate?.ToString(), System.Text.Encoding.UTF8, "text/plain");
                    file1Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("CreatedDate");
                    file1Content.Headers.ContentDisposition.Name = "CreatedDate";
                    content.Add(file1Content);
                    var file3Content = new StringContent(item.HeaderImageUrl!= null? item.HeaderImageUrl?.ToString() : "", System.Text.Encoding.UTF8, "text/plain");
                    file3Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("HeaderImageUrl");
                    file3Content.Headers.ContentDisposition.Name = "HeaderImageUrl";
                    content.Add(file3Content);
                    var file5Content = new StringContent(item.IdTemplateType?.ToString(), System.Text.Encoding.UTF8, "text/plain");
                    file5Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("IdTemplateType");
                    file5Content.Headers.ContentDisposition.Name = "IdTemplateType";
                    var file6Content = new StringContent(item.IsActive?.ToString(), System.Text.Encoding.UTF8, "text/plain");
                    file6Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("IsActive");
                    file6Content.Headers.ContentDisposition.Name = "IsActive";
                    content.Add(file6Content);

                }
            }

            var response = new HttpResponseMessage();
            response.Content = content;
            return response;
        }
        [AllowAnonymous]
        [Route("Deleted/{IdEmailTemplate}")]
        [HttpPost]
        public IHttpActionResult DeletedStatus(int IdEmailTemplate, bool isDeleted)
        {
            try
            {
                var res = _emailTemplateRepository.UpdateDeletedStatus(IdEmailTemplate, isDeleted);
                return WebResult(res);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
        }
    }
}