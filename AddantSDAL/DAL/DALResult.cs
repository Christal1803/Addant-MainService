using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AddantService.DAL
{

    public enum Status { Found, NotFound, NoContent, Created, Accepted, Exception, Conflict, Deleted, Forbidden }
    public class DALResult<T>
    {
        public Status Status;
        public string ErrMessage;
        public T Object;
        public HttpResponseMessage httpResponseMessage;

        public DALResult(Status Status) : this(Status, default(T), null, null)
        {
        }
        public DALResult(Status Status, T Object, string comment, HttpResponseMessage httpResponseMessage)
        {
            this.Status = Status;
            this.Object = Object;
            this.ErrMessage = comment;
            this.httpResponseMessage = httpResponseMessage;
        }

    }
}
