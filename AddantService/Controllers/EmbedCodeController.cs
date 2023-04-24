using AddantSDAL.DAL;
using AddantSDAL.DTO;
using AddantService.DAL;
using AddantService.Helper;
using Microsoft.AspNetCore.Mvc.Formatters;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace AddantService.Controllers
{
    [RoutePrefix("api/EmbedCode")]
    public class EmbedCodeController : BaseController
    {
        public IEmbedCodeRepository _embedCodeRepository;
        public EmbedCodeController(IEmbedCodeRepository embedCodeRepository)
        {
            _embedCodeRepository = embedCodeRepository;
        }

        [Authorize]
        [Route("Create")]
        public async Task<EmbededCodeDTO> CreateEnbedCodeData()
        {
            try
            {
                var embededCodeDTO = new EmbededCodeDTO();
                if (!Request.Content.IsMimeMultipartContent())
                {
                    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
                }

                var provider = await Request.Content.ReadAsMultipartAsync<InMemoryMultipartFormDataStreamProvider>(new InMemoryMultipartFormDataStreamProvider());
                //access form data  
                NameValueCollection formData = provider.FormData;

                if (provider.FormData != null)
                {
                    embededCodeDTO.IdEmbedCode = Convert.ToInt32(formData["IdEmbedCode"] != null ? formData["IdEmbedCode"] : "0");
                    embededCodeDTO.Code = formData["Code"];
                    embededCodeDTO.IdCodeType = Convert.ToInt32(formData["IdCodeType"] != null ? formData["IdCodeType"] : "0");
                    embededCodeDTO.IsDeleted = Convert.ToBoolean(formData["IsDeleted"]);
                    embededCodeDTO.CreatedDate = Convert.ToDateTime(formData["CreatedDate"]);
                    var rest = _embedCodeRepository.CreateEmbededCode(embededCodeDTO);
                    embededCodeDTO = rest?.Object;
                }
                    
                return embededCodeDTO;
            }
            catch (Exception ex) { Logger.WriteLog("Inside UploadAddantFile():" + ex.Message.ToString()); return null; }
        }
        [Authorize]
        [HttpPost]
        [Route("CreateData")]
        public IHttpActionResult CreateEmbedCode([FromBody] Models.EmbedCodeModel embedCodeModel)
        {
            try
            {
                var _data = new EmbededCodeDTO
                {
                    IdEmbedCode = embedCodeModel.IdEmbedCode,
                    Code = embedCodeModel.Code,
                    CreatedDate = embedCodeModel.CreatedDate,
                    IsDeleted = embedCodeModel.IsDeleted,
                    Deleted = false

                };
                var res = _embedCodeRepository.CreateEmbededCode(_data);
                return WebResult(res);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("Detail")]
        public IHttpActionResult GetAllEmbedCode(DateTime? startDate = null, DateTime? endDate = null,  bool isAdminCall = false)
        {
            try
            {
                var res = _embedCodeRepository.GetAllEmbededCode(startDate, endDate, isAdminCall);
                return WebResult(res);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("Detail/FormData")]
        public HttpResponseMessage GetAllEmbedCode(DateTime? startDate = null, DateTime? endDate = null, bool isAdminCall = false,int IdEmbedCode = 0)
        {
            var content = new MultipartContent();

            //TextToHtml("Hi am a htm content");
            var blogDTOs = IdEmbedCode == 0 ? _embedCodeRepository.GetAllEmbededCode(startDate,endDate,isAdminCall) : _embedCodeRepository.GetEmbededCodeById(IdEmbedCode);
            //ids = blogDTOs?.Object.Select(x => x.IdEmbedCode).ToList();

            if (blogDTOs?.Object != null)
            {
                foreach (var item in blogDTOs.Object)
                {
                    var file2Content = new StringContent(item.IdEmbedCode.ToString(), System.Text.Encoding.UTF8, "text/plain");
                    file2Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("IdEmbedCode");
                    file2Content.Headers.ContentDisposition.Name = "IdEmbedCode";
                    content.Add(file2Content);
                    var file1Content = new StringContent(item.Code, System.Text.Encoding.UTF8, "text/plain");
                    file1Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("Code") ;
                    file1Content.Headers.ContentDisposition.Name = "Code";
                    content.Add(file1Content);
                    var file3Content = new StringContent(item.IdCodeType.ToString(), System.Text.Encoding.UTF8, "text/plain");
                    file3Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("IdEmbedCodeType");
                    file3Content.Headers.ContentDisposition.Name = "IdEmbedCodeType";
                    content.Add(file3Content);
                    var file4Content = new StringContent(item.CreatedDate.ToString(), System.Text.Encoding.UTF8, "text/plain");
                    file4Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("CreatedDate");
                    file4Content.Headers.ContentDisposition.Name = "CreatedDate";
                    content.Add(file4Content);
                    var file5Content = new StringContent(item.IsDeleted.ToString(), System.Text.Encoding.UTF8, "text/plain");
                    file5Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("IsDeleted");
                    file5Content.Headers.ContentDisposition.Name = "IsDeleted";
                    content.Add(file5Content);

                }
            }

            var response = new HttpResponseMessage();
            response.Content = content;
            return response;
        }

        [Authorize]
        [HttpGet]
        [Route("Detail/{idEmbedCode}")]
        public IHttpActionResult GetAllEmbed(int idEmbedCode)
        {
            try
            {
                var res = _embedCodeRepository.GetEmbededCodeById(idEmbedCode);
                return WebResult(res);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
        }

        [Authorize]
        [HttpDelete]
        [Route("Detail/{idEmbedCode}/{isDelete}")]
        public IHttpActionResult DeleteEnquiry(int idEmbedCode, bool isDelete)
        {
            try
            {
                var res = _embedCodeRepository.Delete(idEmbedCode, isDelete);
                return WebResult(res);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
        }
        //[AllowAnonymous]
        //[HttpPost]
        //[Route("testHtmlConversion")]
        //public IHttpActionResult TestHtmlConversion(string content)
        //{
        //    content = TextToHtml(content);
        //    return WebResult(new DALResult<string>(Status.Created, null, content, null) );
        //}
        [AllowAnonymous]
        [Route("Deleted/{IdEmbedCode}")]
        [HttpPost]
        public IHttpActionResult DeletedStatus(int IdEmbedCode, bool isDeleted)
        {
            try
            {
                var res = _embedCodeRepository.UpdateDeletedStatus(IdEmbedCode, isDeleted);
                return WebResult(res);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
        }

    }
}