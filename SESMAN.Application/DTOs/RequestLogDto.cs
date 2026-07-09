using SESMAN.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SESMAN.Application.DTOs
{
    public class RequestLogDto :BaseDto //GET için
    {
        public string? Url { get; set; }
        public HttpMethodType Method { get; set; }

    
        public string? Body { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<RequestHeaderDto> RequestHeaders { get; set; } = new();
        public List<RequestParameterDto> RequestParameters { get; set; } = new();
    }
}
