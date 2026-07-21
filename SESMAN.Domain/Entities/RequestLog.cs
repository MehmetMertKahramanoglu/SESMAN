using SESMAN.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SESMAN.Domain.Entities
{
    public class RequestLog: BaseEntity
    {
        public required string Url { get; set; }
        public HttpMethodType Method {  get; set; }

        public string? Body { get; set; }

        public string? BodyType { get; set; }

        //body type
        public string? RawType { get; set; }

        //1-n olunca ICollection yapısı kullanılır.
        public virtual ICollection<RequestParameter> RequestParameters { get; set; } = new List<RequestParameter>();
        public virtual ICollection<RequestHeader> RequestHeaders { get; set; } = new List<RequestHeader>();

        //1-1 olunca bu yapı kullanılır.
        public virtual ResponseLog? Response {  get; set; }
    }
}
