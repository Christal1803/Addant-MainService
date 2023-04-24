using AddantSDAL.DAL;
using AddantSDAL.DTO;
using AddantService.Helper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;

namespace AddantService.Controllers
{

    [RoutePrefix("api/Blog")]
    public class BlogController : BaseController
    {
        IBlogRepository _blogRepository;
        IEmailTemplateRepository _emailTemplateRepository;
        public BlogController(IBlogRepository blogRepository, IEmailTemplateRepository emailTemplateRepository)
        {
            _blogRepository = blogRepository;
            _emailTemplateRepository = emailTemplateRepository;
        }
        [Authorize]
        [Route("compose")]
        public IHttpActionResult ComposeBlog([FromBody] Models.BlogModel blogModel)
        {
            try
            {
                Logger.WriteLog("Inside ComposeBlog()");
                Logger.WriteLog("Inside ComposeBlog1111 testing ()");

                BlogDTO _data = new BlogDTO
                {
                    BannerImgUrl = blogModel.BannerImgUrl,
                    ContentUrl = blogModel.ContentUrl,
                    ConverImgUrl = blogModel.ConverImgUrl,
                    CreatedOn = blogModel.CreatedOn,
                    IdBlog = blogModel.IdBlog,
                    IsDeleted = blogModel.IsDeleted,
                    MainHeader = blogModel.MainHeader,
                    CreatedBy = blogModel.CreatedBy,
                    BlogContent = blogModel.BlogContent,
                    Deleted = false

                };

                if (!String.IsNullOrEmpty(_data.BlogContent))
                    _data.MinReadTime = GetMinReadTime(_data.BlogContent);
                //if (_data.IsDeleted == false)
                //{
                //    var emailField = new Email.EmailField
                //    {
                //        UserName = _data.CreatedBy,
                //        Title = _data.MainHeader,
                //        Subject = "Check out the newly created blog",
                //        TemplatePath = "/NotifyBlogToaddant.html"
                //    };
                //    var templateCandidate = _emailTemplateRepository.GetAllEmailTemplateById(0, "NotifyBlogToaddant")?.Object;
                //    emailField.Body = templateCandidate?.Body;
                //    if (!string.IsNullOrEmpty(templateCandidate?.HeaderImageUrl))
                //    {
                //        Match url = Regex.Match(templateCandidate?.HeaderImageUrl, @"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?");
                //        if (url.Length == 0)
                //            emailField.HeaderImageUrl = Path.Combine((Path.Combine(ConfigurationManager.AppSettings["HostedDomainName"].ToString() + "/Uploads/EmailHeader", "")), templateCandidate?.HeaderImageUrl != null ? templateCandidate?.HeaderImageUrl : "");
                //    }
                // Email.SendBlogEmail(emailField);
                // }
                var res = _blogRepository.ComposeBlog(_data);
                return WebResult(res);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
        }

        [AllowAnonymous]
        [Route("Detail")]
        public IHttpActionResult GetAllBlog(bool isAdminCall = false, DateTime? startDate = null, DateTime? endDate = null, string SearchText = "")
        {
            try
            {
                var res = _blogRepository.GetAllBlog(isAdminCall, startDate, endDate, SearchText, "");
                if (res.Object != null)
                {
                    foreach (var item in res.Object)
                    {
                        if (item.BannerImgUrl != string.Empty)
                            item.BannerImgUrl = Path.Combine((Path.Combine(ConfigurationManager.AppSettings["HostedDomainName"].ToString() + "/Uploads/Image", "")), item.BannerImgUrl != null ? item.BannerImgUrl : "");

                        if (item.ConverImgUrl != string.Empty)
                            item.ConverImgUrl = Path.Combine((Path.Combine(ConfigurationManager.AppSettings["HostedDomainName"].ToString() + "/Uploads/Image", "")), item.ConverImgUrl != null ? item.ConverImgUrl : "");

                        if (item.ProfilePicUrl != string.Empty)
                            item.ProfilePicUrl = Path.Combine((Path.Combine(ConfigurationManager.AppSettings["HostedDomainName"].ToString() + "/Uploads/Image", "")), item.ProfilePicUrl != null ? item.ProfilePicUrl : "");
                    }
                }
                return WebResult(res);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
        }

        [AllowAnonymous]
        [Route("Detail/{blogId}")]
        public IHttpActionResult GetBlogById(int blogId)
        {
            try
            {
                var res = _blogRepository.GetBlogById(blogId);
                if (res.Object != null)
                {
                    if (res.Object.BannerImgUrl != string.Empty)
                        res.Object.BannerImgUrl = Path.Combine((Path.Combine(System.Configuration.ConfigurationManager.AppSettings["HostedDomainName"].ToString() + "/Uploads/Image", "")), res.Object.BannerImgUrl != null ? res.Object.BannerImgUrl : "");
                    if (res.Object.ConverImgUrl != string.Empty)
                        res.Object.ConverImgUrl = Path.Combine((Path.Combine(System.Configuration.ConfigurationManager.AppSettings["HostedDomainName"].ToString() + "/Uploads/Image", "")), res.Object.ConverImgUrl != null ? res.Object.ConverImgUrl : "");
                    if (res.Object.ProfilePicUrl != string.Empty)
                        res.Object.ProfilePicUrl = Path.Combine((Path.Combine(System.Configuration.ConfigurationManager.AppSettings["HostedDomainName"].ToString() + "/Uploads/Image", "")), res.Object.ProfilePicUrl != null ? res.Object.ProfilePicUrl : "");

                }
                return WebResult(res);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
        }

        [AllowAnonymous]
        [Route("{blogId}")]
        [HttpDelete]
        public IHttpActionResult Delete(int blogId, bool isDelete)
        {
            try
            {
                var res = _blogRepository.Delete(blogId, isDelete);
                if (!isDelete && res != null)
                {
                    var BlogData = _blogRepository.GetBlogById(blogId);

                    var emailField = new Email.EmailField
                    {
                        UserName = BlogData?.Object?.CreatedBy,
                        Title = BlogData?.Object?.MainHeader,
                        Subject = "Check out the newly created blog",
                        TemplatePath = "/NotifyBlogToaddant.html",
                        Link = $"https://addant.com/#/blog/{BlogData?.Object?.MainHeader?.Replace(" ", "-")}",
                    };
                    var templateCandidate = _emailTemplateRepository.GetAllEmailTemplateById(0, "NotifyBlogToaddant")?.Object;
                    if (templateCandidate != null)
                    {
                        emailField.Body = templateCandidate?.Body;
                        if (!string.IsNullOrEmpty(templateCandidate?.HeaderImageUrl))
                        {
                            Match url = Regex.Match(templateCandidate?.HeaderImageUrl, @"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?");
                            if (url.Length == 0)
                                emailField.HeaderImageUrl = Path.Combine((Path.Combine(ConfigurationManager.AppSettings["HostedDomainName"].ToString() + "/Uploads/EmailHeader", "")), templateCandidate?.HeaderImageUrl != null ? templateCandidate?.HeaderImageUrl : "");
                        }
                        Email.SendBlogEmail(emailField);
                    }
                }
                return WebResult(res);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
        }

        [Authorize]
        [Route("DocumentUpload/MediaUpload")]
        public async Task<bool> UploadBlogFile()
        {
            try
            {
                string ImgName = string.Empty;
                BlogDTO blogDTO = new BlogDTO();
                var dicFileUpload = new Dictionary<string, string>();
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

                            if (!string.IsNullOrEmpty(fileType))
                            {
                                Stream input = await file1.ReadAsStreamAsync();
                                string imagePath = string.Empty;
                                string folderPath = string.Empty;

                                if (formData["UploadedDocs"] == "UploadedDocs" && !string.IsNullOrEmpty(fileType))
                                {
                                    folderPath = HttpContext.Current.Server.MapPath(Path.Combine("~/Uploads/Image"));
                                    ImgName = "Img" + Guid.NewGuid() + fileType.ToString();
                                    imagePath = folderPath + "/" + ImgName;

                                    //Deletion exists file  
                                    if (File.Exists(imagePath))
                                    {
                                        File.Delete(imagePath);
                                    }
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
                    if (_blogRepository.GetBlogById(Convert.ToInt32(formData["IdBlog"])).Object != null)
                    {
                        blogDTO = _blogRepository.GetBlogById(Convert.ToInt32(formData["IdBlog"])).Object;
                        if (dicFileUpload != null)
                        {
                            blogDTO.BannerImgUrl = dicFileUpload.ContainsKey("BannerImg") ? dicFileUpload["BannerImg"].Contains("http") ? "" : dicFileUpload["BannerImg"] : "";
                            blogDTO.ConverImgUrl = dicFileUpload.ContainsKey("CoverImg") ? dicFileUpload["CoverImg"].Contains("http") ? "" : dicFileUpload["CoverImg"] : "";
                            blogDTO.ProfilePicUrl = dicFileUpload.ContainsKey("ProfilePic") ? dicFileUpload["ProfilePic"].Contains("http") ? "" : dicFileUpload["ProfilePic"] : "";

                            var res = _blogRepository.ComposeBlog(blogDTO);
                            Logger.WriteLog("Inside dicFileUpload -" + res.ToString());
                            return res != null ? true : false;
                        }
                    }
                }
                else
                    return true;
                return false;
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return false; }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("File")]
        public HttpResponseMessage GetAllBlogImage(int BlogId = 0)
        {
            var content = new MultipartContent();
            var ids = new List<int>() { 1, 2, 3 };
            if (BlogId > 0)
            {
                var blogDTOs = _blogRepository.GetBlogById(BlogId);
                if (blogDTOs != null)
                {
                    ids = new List<int> { blogDTOs.Object.IdBlog };

                    var objectContent = new ObjectContent<List<int>>(ids, new System.Net.Http.Formatting.JsonMediaTypeFormatter());
                    content.Add(objectContent);
                    //blog list here 
                    if (blogDTOs.Object != null)
                    {
                        var file1Content = new StreamContent(new FileStream(Path.Combine(HttpContext.Current.Server.MapPath("~/Uploads/Image"), blogDTOs.Object.BannerImgUrl), FileMode.Open));
                        file1Content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("image/jpeg");
                        file1Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = blogDTOs.Object.BannerImgUrl };
                        file1Content.Headers.ContentDisposition.Name = "BannerImg";
                        content.Add(file1Content);

                        var file3Content = new StreamContent(new FileStream(Path.Combine(HttpContext.Current.Server.MapPath("~/Uploads/Image"), blogDTOs.Object.ConverImgUrl), FileMode.Open));
                        file3Content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("image/jpeg");
                        file3Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = blogDTOs.Object.ConverImgUrl };
                        file3Content.Headers.ContentDisposition.Name = "ConverImg";
                        content.Add(file3Content);

                        var file4Content = new StreamContent(new FileStream(Path.Combine(HttpContext.Current.Server.MapPath("~/Uploads/Image"), blogDTOs.Object.ProfilePicUrl), FileMode.Open));
                        file4Content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("image/jpeg");
                        file4Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = blogDTOs.Object.ProfilePicUrl };
                        file4Content.Headers.ContentDisposition.Name = "ProfilePic";
                        content.Add(file4Content);
                    }
                }
            }
            else
            {
                var blogDTOs = _blogRepository.GetAllBlog();
                ids = blogDTOs?.Object.Select(x => x.IdBlog).ToList();

                var objectContent = new ObjectContent<List<int>>(ids, new System.Net.Http.Formatting.JsonMediaTypeFormatter());
                content.Add(objectContent);
                //blog list here 
                if (blogDTOs.Object != null)
                {
                    foreach (var item in blogDTOs.Object)
                    {
                        var file1Content = new StreamContent(new FileStream(Path.Combine(HttpContext.Current.Server.MapPath("~/Uploads/Image"), item.BannerImgUrl), FileMode.Open));
                        file1Content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("image/jpeg");
                        file1Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = item.BannerImgUrl };
                        file1Content.Headers.ContentDisposition.Name = "BannerImg";
                        content.Add(file1Content);

                        var file3Content = new StreamContent(new FileStream(Path.Combine(HttpContext.Current.Server.MapPath("~/Uploads/Image"), item.ConverImgUrl), FileMode.Open));
                        file3Content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("image/jpeg");
                        file3Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = item.ConverImgUrl };
                        file3Content.Headers.ContentDisposition.Name = "ConverImg";
                        content.Add(file3Content);

                        var file4Content = new StreamContent(new FileStream(Path.Combine(HttpContext.Current.Server.MapPath("~/Uploads/Image"), item.ProfilePicUrl), FileMode.Open));
                        file4Content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("image/jpeg");
                        file4Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = item.ProfilePicUrl };
                        file4Content.Headers.ContentDisposition.Name = "ProfilePic";
                        content.Add(file4Content);
                    }
                }
            }


            var response = new HttpResponseMessage();
            response.Content = content;
            return response;
        }



        [AllowAnonymous]
        [Route("Deleted/{Blogid}")]
        [HttpPost]
        public IHttpActionResult DeletedStatus(int Blogid, bool isDeleted)
        {
            try
            {
                var res = _blogRepository.UpdateDeletedStatus(Blogid, isDeleted);
                return WebResult(res);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
        }


        #region Methods
        /// <summary>
        /// To calculate min read time of blog.
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public int GetMinReadTime(string strContent)
        {
            int readTime = 0;
            //string strContent = "";
            int wordCount = 0; int avmForContent = 200; // AWM - Average words per minute for a person.
            try
            {
                //strContent = System.Text.Encoding.UTF8.GetString(content);
                if (!string.IsNullOrEmpty(strContent))
                {
                    wordCount = strContent.Split(' ').Length;
                    decimal t1 = (wordCount / avmForContent);
                    readTime = Convert.ToInt32(Math.Round(t1, 0));
                }
                return readTime;
            }
            catch (Exception ex) { return readTime; }
        }


        ////
        ///Delete Blog by setting up a Deleted fiels to True
        ///
        [AllowAnonymous]
        [Route("{blogId}")]
        [HttpDelete]
        public IHttpActionResult Delete(int blogId)
        {
            try
            {
                var res = _blogRepository.Delete(blogId);
                var BlogData = _blogRepository.Delete(blogId);
                return WebResult(res);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
        }




        #endregion
    }
}