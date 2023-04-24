using AddantSDAL.DAL;
using AddantSDAL.DTO;
using AddantService.Helper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace AddantService.Controllers
{
    [RoutePrefix("api/AddantLife")]
    public class AddantLifeController : BaseController
    {
        IAddantLifeRepository _addantLifeRepository;
        public AddantLifeController(IAddantLifeRepository addantLifeRepository)
        {
            _addantLifeRepository = addantLifeRepository;
        }

        [Authorize]
        [Route("Compose")]
        public IHttpActionResult ComposeAddantLife([FromBody] Models.AddantLifeModel addantLifeModel)
        {
            try
            {
                if (addantLifeModel != null)
                {
                    AddantLifeDTO addantLifeDTO = new AddantLifeDTO
                    {
                        IdAddantLife = addantLifeModel.IdAddantLife,
                        IsDeleted = addantLifeModel.IsDeleted,
                        Title = addantLifeModel.Title,
                        BannerImgUrl = addantLifeModel.BannerImgUrl,
                        Description = addantLifeModel.Description,
                        IdEventCategory = addantLifeModel.IdEventCategory,
                        CreatedDate = addantLifeModel.CreatedDate,
                        addantLifeDetials = addantLifeModel.AddantLifeDetials?.Select(x => new AddantLifeDetialDTO {
                            IdAddantLife = x.IdAddantLife,
                            IdAddantLifeImage = x.IdAddantLifeImage,
                            InnerCaption = x.InnerCaption,
                            InnerImage = x.InnerImage,                                                  
                    Deleted = false

                        }).ToList()
                    };
                    var res = _addantLifeRepository.ComposeAddantLife(addantLifeDTO);
                    return WebResult(res);
                }
                else return null;
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
        }

        [AllowAnonymous]
        [Route("")]
        public IHttpActionResult GetAllAddantLife(int idAddantLife = 0, bool isAdminCall = false, string searchText ="",DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
               var res = _addantLifeRepository.GetAllAddantLife(idAddantLife,isAdminCall,searchText,startDate,endDate);
                if (res.Object.Count >0)
                {
                    foreach (var item in res.Object)
                    {
                        if (item.CoverImgUrl != string.Empty)
                            item.CoverImgUrl = Path.Combine((Path.Combine(System.Configuration.ConfigurationManager.AppSettings["HostedDomainName"].ToString() + "/Uploads/Image", "")), item.CoverImgUrl != null ? item.CoverImgUrl : "");
                        if (item.BannerImgUrl != string.Empty)
                            item.BannerImgUrl = Path.Combine((Path.Combine(System.Configuration.ConfigurationManager.AppSettings["HostedDomainName"].ToString() + "/Uploads/Image", "")), item.BannerImgUrl != null ? item.BannerImgUrl : "");
                    }
                }
                return WebResult(res);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
        }

        [AllowAnonymous]
        [Route("Detail/Image")]
        public IHttpActionResult GetAllImage(int idAddantLife = 0, bool isAdminCall = false)
        {
            try
            {
                var res = _addantLifeRepository.GetAllAddantLife(idAddantLife, isAdminCall);
                if (res.Object.Count >0)
                {
                    foreach (var item in res.Object)
                    {
                        if (item.CoverImgUrl != string.Empty)
                            item.CoverImgUrl = Path.Combine((Path.Combine(System.Configuration.ConfigurationManager.AppSettings["HostedDomainName"].ToString() + "/Uploads/Image", "")), item.CoverImgUrl != null ? item.CoverImgUrl : "");
                        if (item.BannerImgUrl != string.Empty)
                            item.BannerImgUrl = Path.Combine((Path.Combine(System.Configuration.ConfigurationManager.AppSettings["HostedDomainName"].ToString() + "/Uploads/Image", "")), item.BannerImgUrl != null ? item.BannerImgUrl : "");
                    }
                }
                return WebResult(res);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
        }

        [AllowAnonymous]
        [Route("Detail/Inner")]
        public IHttpActionResult GetAllInnerImage(int idAddantLife = 0, bool isAdminCall = false)
        {
            try
            {
                var res = _addantLifeRepository.GetAllInnerImage(idAddantLife);
                if (res.Object.Count>0)
                {
                    foreach (var item in res.Object)
                    {
                        if (item.InnerImageUrl != string.Empty)
                            item.InnerImageUrl = Path.Combine((Path.Combine(System.Configuration.ConfigurationManager.AppSettings["HostedDomainName"].ToString() + "/Uploads/Image", "")), item.InnerImageUrl != null ? item.InnerImageUrl : "");
                    }
                }
                return WebResult(res);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
        }

        [Authorize]
        [Route("DocumentUpload/MediaUpload")]
        public async Task<bool> UploadAddantFile()
        {
            try
            {
                string ImgName = string.Empty;
                var dicFileUpload = new Dictionary<string, string>();
                AddantLifeDTO addantLifeDTO = new AddantLifeDTO();
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
                    var res = _addantLifeRepository.GetAllAddantLife(Convert.ToInt32(formData["IdAddantLife"]), true);
                    if(res.Object.Count > 0)
                    if (dicFileUpload != null)
                    {
                            addantLifeDTO = res.Object.FirstOrDefault();
                            addantLifeDTO.IdAddantLife = Convert.ToInt32(formData["IdAddantLife"]);
                            if(dicFileUpload.ContainsKey("coverImg"))
                            addantLifeDTO.CoverImgUrl = dicFileUpload["coverImg"].Contains("http") ? string.Empty : dicFileUpload["coverImg"];
                            if (dicFileUpload.ContainsKey("BannerImgUrl"))
                            addantLifeDTO.BannerImgUrl = dicFileUpload["BannerImgUrl"].Contains("http") ? string.Empty : dicFileUpload["BannerImgUrl"];

                            var rest = _addantLifeRepository.ComposeAddantLife(addantLifeDTO);
                    }
                }
                return true;
            }
            catch (Exception ex) {  Logger.WriteLog("Inside UploadAddantFile():" + ex.Message.ToString()); return false; }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("File")]
        public HttpResponseMessage GetAllAddantImage(int IdAddantLife = 0)
        {
            var content = new MultipartContent();
            var ids = new List<int>() { 1, 2, 3 };
            if (IdAddantLife > 0)
            {
                var blogDTOs = _addantLifeRepository.GetAllAddantLife(IdAddantLife);
                if (blogDTOs != null)
                {
                    ids =  blogDTOs.Object.Select(x=>x.IdAddantLife).ToList();

                    var objectContent = new ObjectContent<List<int>>(ids, new System.Net.Http.Formatting.JsonMediaTypeFormatter());
                    content.Add(objectContent);
                    //blog list here 
                    if (blogDTOs.Object != null)
                    {
                        if (blogDTOs.Object.FirstOrDefault().CoverImgUrl != null)
                        {
                            var file1Content = new StreamContent(new FileStream(Path.Combine(HttpContext.Current.Server.MapPath("~/Uploads/Image"), blogDTOs.Object.FirstOrDefault().CoverImgUrl), FileMode.Open));
                            file1Content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("image/jpeg");
                            file1Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = blogDTOs.Object.FirstOrDefault().CoverImgUrl };
                            file1Content.Headers.ContentDisposition.Name = "CoverImgUrl";
                            content.Add(file1Content);
                        }
                       

                        //if (blogDTOs.Object.FirstOrDefault().ThumbNailInnerUrl != null)
                        //{
                        //    var file3Content = new StreamContent(new FileStream(Path.Combine(HttpContext.Current.Server.MapPath("~/Uploads/Image"), blogDTOs.Object.FirstOrDefault().ThumbNailInnerUrl), FileMode.Open));
                        //    file3Content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("image/jpeg");
                        //    file3Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = blogDTOs.Object.FirstOrDefault().ThumbNailInnerUrl };
                        //    file3Content.Headers.ContentDisposition.Name = "ThumbNailInnerUrl";
                        //    content.Add(file3Content);
                        //}
                    }
                }
            }
            else
            {
                var blogDTOs = _addantLifeRepository.GetAllAddantLife();
                ids = blogDTOs?.Object.Select(x => x.IdAddantLife).ToList();

                var objectContent = new ObjectContent<List<int>>(ids, new System.Net.Http.Formatting.JsonMediaTypeFormatter());
                content.Add(objectContent);
                //blog list here 
                if (blogDTOs.Object != null)
                {
                    foreach (var item in blogDTOs.Object)
                    {
                        if (item.CoverImgUrl != null)
                        {
                            var file1Content = new StreamContent(new FileStream(Path.Combine(HttpContext.Current.Server.MapPath("~/Uploads/Image"), item.CoverImgUrl), FileMode.Open));
                            file1Content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("image/jpeg");
                            file1Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = item.CoverImgUrl };
                            file1Content.Headers.ContentDisposition.Name = "CoverImgUrl";
                            content.Add(file1Content);
                        }

                        //if (item.ThumbNailInnerUrl != null)
                        //{  
                        //    var file3Content = new StreamContent(new FileStream(Path.Combine(HttpContext.Current.Server.MapPath("~/Uploads/Image"), item.ThumbNailInnerUrl), FileMode.Open));
                        //    file3Content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("image/jpeg");
                        //    file3Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = item.ThumbNailInnerUrl };
                        //    file3Content.Headers.ContentDisposition.Name = "ThumbNailInnerUrl";
                        //    content.Add(file3Content);
                        //}
                    }
                }
            }


            var response = new HttpResponseMessage();
            response.Content = content;
            return response;
        }

        [Authorize]
        [Route("DocumentUpload/InnerMediaUpload")]
        public async Task<bool> UploadAddantInnerImage()
        {
            try
            {
                string ImgName = string.Empty;
                var dicFileUpload = new Dictionary<string, string>();
                var _addantLifeDetails = new List<AddantLifeDetialDTO>();
                AddantLifeDTO addantLifeDTO = new AddantLifeDTO();
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
                var _formDataContent = new List<FormDataContent>();
               // HttpContent file1 = null;
                foreach (var res in provider.Contents)
                {
                    var _data = new FormDataContent();
                    if (formData["UploadedDocs"] == "UploadedDocs")
                    {
                        if (res.Headers.ContentType == null)
                        {
                           
                            if (res.Headers.ContentDisposition.Name.Trim('\"') == "InnerCaption")
                            { _data.Key = "InnerCaption"; _data.Value = formData["InnerCaption"]; }
                            if (res.Headers.ContentDisposition.Name.Trim('\"') == "IdAddantLifeImage")
                            { _data.Key = "IdAddantLifeImage"; _data.Value = formData["IdAddantLifeImage"]; }
                            if (res.Headers.ContentDisposition.Name.Trim('\"') == "InnerImageUrl")
                            { _data.Key = "InnerImageUrl"; _data.Value = formData["InnerImageUrl"]; }
                            if (res.Headers.ContentDisposition.Name.Trim('\"') == "IdAddantLife")
                            { _data.Key = "IdAddantLife"; _data.Value = formData["IdAddantLife"]; }
                        }
                        else
                        {
                            HttpContent file2 = files.FirstOrDefault();
                            var thisFileName = res?.Headers.ContentDisposition.FileName.Trim('\"');
                            var fileType = Path.GetExtension(thisFileName?.ToString());
                            if (!string.IsNullOrEmpty(fileType))
                            {
                                Stream input = await file2.ReadAsStreamAsync();
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
                                }
                                if (!Directory.Exists(folderPath))
                                {
                                    Directory.CreateDirectory(folderPath);
                                }
                                using (Stream file = File.OpenWrite(imagePath))
                                {
                                    input.CopyTo(file);
                                    file.Close();
                                    files.Remove(file2);
                                }
                                _data.Key = "InnerImageUrl"; _data.Value = ImgName;
                            }
                        }
                        if(_data!=null)
                        _formDataContent.Add(_data);
                    }
                }
                if (_formDataContent?.Count > 0)
                {
                    List<string> innerCaptions = _formDataContent.Where(x => x.Key == "InnerCaption").FirstOrDefault()?.Value.Split(',').ToList();
                    List<string> idAddantLifeImages = _formDataContent.Where(x => x.Key == "IdAddantLifeImage").FirstOrDefault()?.Value.Split(',').ToList();
                    List<string> innerImageUrls = _formDataContent.Where(x => x.Key == "InnerImageUrl").Select(y=>y.Value).ToList();
                    var _addantLifeDetail = new List<AddantLifeDetialDTO>();
                    int pos = innerCaptions.Count;
                    int countPic =0;
                    foreach (var item in innerCaptions)
                    {
                        
                        var addantLifeDetail = new AddantLifeDetialDTO();
                        addantLifeDetail.IdAddantLife = Convert.ToInt32(_formDataContent.Where(x => x.Key == "IdAddantLife").FirstOrDefault().Value.ToString());
                        addantLifeDetail.InnerCaption = item;
                        addantLifeDetail.IdAddantLifeImage = Convert.ToInt32(idAddantLifeImages.ElementAt(pos-1));
                        addantLifeDetail.InnerImageUrl = innerImageUrls.ElementAt(countPic).Contains("https") ?"": innerImageUrls?.ElementAt(countPic);
                        _addantLifeDetail.Add(addantLifeDetail);
                        --pos;countPic++;
                    }
                    var resAddant = _addantLifeRepository.GetAllAddantLife(Convert.ToInt32(formData["IdAddantLife"]), true)?.Object?.FirstOrDefault();
                    AddantLifeDTO res = resAddant != null ? resAddant : null;
                    res.addantLifeDetials = _addantLifeDetail;
                    var r = _addantLifeRepository.ComposeAddantLife(res);

                }
                return true;
            }
            catch (Exception ex) { return false; }
        }

        [Authorize]
        [Route("{idAddantLife}")]
        [HttpDelete]
        public IHttpActionResult DisableAddantLife(int idAddantLife, bool isDeleted =false)
        {
            var res = _addantLifeRepository.DisableAddantLife(idAddantLife, isDeleted);
             return WebResult(res);
        }

        [Authorize]
        [Route("Detail/{idAddantLifeInner}")]
        [HttpDelete]
        public IHttpActionResult DisableAddantLifeInner(int idAddantLifeInner, bool isDeletedDetail = false)
        {
            var res = _addantLifeRepository.DisableAddantLifeDetail(idAddantLifeInner, isDeletedDetail);
            return WebResult(res);
        }

        [Authorize]
        [Route("DocumentUpload/SingleInnerImage")]
        public async Task<bool> UploadAddantSingleInnerImage()
        {
            try
            {
                string ImgName = string.Empty;
                var dicFileUpload = new Dictionary<string, string>();
                AddantLifeDTO addantLifeDTO = new AddantLifeDTO();
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
                                dicFileUpload.Add(file1.Headers.ContentDisposition.Name.Trim('\"'), ImgName);
                            }
                        }
                    }
                    var resOuter = _addantLifeRepository.GetAllAddantLife(Convert.ToInt32(string.IsNullOrEmpty(formData["IdAddantLife"]) ? "0" : formData["IdAddantLife"]));
                    if (resOuter?.Object.Count > 0)
                        if (dicFileUpload != null)
                        {
                        var addantLifeDetialDTO = new AddantLifeDetialDTO();
                        addantLifeDetialDTO = _addantLifeRepository.GetAllInnerImage(Convert.ToInt32(formData["IdAddantLife"]))?.Object?.Where(x=>x.IdAddantLifeImage == Convert.ToInt32(formData["IdAddantLifeImage"])).FirstOrDefault();
                            addantLifeDTO.IdAddantLife = Convert.ToInt32(string.IsNullOrEmpty(formData["IdAddantLife"])?"0": formData["IdAddantLife"]);
                        if (!string.IsNullOrEmpty(formData["InnerCaption"]))
                            addantLifeDetialDTO.InnerCaption = formData["InnerCaption"];
                            if (dicFileUpload.ContainsKey("InnerImageUrl"))
                            addantLifeDetialDTO.InnerImageUrl = dicFileUpload["InnerImageUrl"].Contains("http") ? string.Empty : dicFileUpload["InnerImageUrl"];
                        addantLifeDTO.addantLifeDetials = new List<AddantLifeDetialDTO>() { addantLifeDetialDTO };

                            var rest = _addantLifeRepository.ComposeAddantLife(addantLifeDTO);
                        }
                return true;
            }
            catch (Exception ex) { Logger.WriteLog("Inside UploadAddantFile():" + ex.Message.ToString()); return false; }
        }

        public class FormDataContent
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }
        [AllowAnonymous]
        [Route("Detail/Event")]
        public IHttpActionResult GetAllAddantByCategor(bool isAdminCall = false, string category = "", string groupBy = "Year")
        {
            try
            {
                var res = _addantLifeRepository.GetAllAddantLifeByCategory( isAdminCall, category, groupBy);
                
                return WebResult(res);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
        }



        [AllowAnonymous]
        [Route("Deleted/{AddantLifeId}")]
        [HttpPost]
        public IHttpActionResult DeletedStatus(int AddantLifeId, bool isDeleted)
        {
            try
            {
                var res = _addantLifeRepository.UpdateDeletedStatus(AddantLifeId, isDeleted);
                return WebResult(res);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
        }

        [AllowAnonymous]
        [Route("AddantDetailDeleted/{DetailId}")]
        [HttpPost]
        public IHttpActionResult DetailDeletedStatus(int DetailId, bool isDeleted)
        {
            try
            {
                var res = _addantLifeRepository.DetailUpdateDeletedStatus(DetailId, isDeleted);
                return WebResult(res);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
        }

    }
}