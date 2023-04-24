
using AddantService.DAL;
using System.Net;
using System.Web.Http;

namespace AddantService.Controllers
{
    public class BaseController : ApiController
    {
        public IHttpActionResult WebResult<T>(DALResult<T> result)
        {
            if (result != null)
            {
                if (result.Status == Status.Accepted)
                    return Content(HttpStatusCode.Accepted, result.Object);
                if (result.Status == Status.Conflict)
                    return Content(HttpStatusCode.Conflict, result.ErrMessage);
                if (result.Status == Status.Created)
                    return Content(HttpStatusCode.Created, result.Object);
                if (result.Status == Status.Exception)
                    return Content(HttpStatusCode.ExpectationFailed, result.ErrMessage);
                if (result.Status == Status.Found)
                    return Ok(result.Object);
                if (result.Status == Status.NotFound)
                    return NotFound();
                if (result.Status == Status.Deleted)
                    return Ok(result.Object);
                if (result.Status == Status.NoContent)
                    return Ok(result.Object);

                if (result.Status == Status.Forbidden)
                    return Content(HttpStatusCode.Forbidden, result.ErrMessage);
            }

            return InternalServerError();
        }
    }
}
