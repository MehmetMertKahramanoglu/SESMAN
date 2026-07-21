using System;
using System.Collections.Generic;
using System.Text;

namespace SESMAN.Application.DTOs
{
    public class CreateRequestLogDto //POST için
    {
        public string? Url { get; set; }
        public string Method { get; set; }

        public string? Body { get; set; }

        public string? BodyType { get; set; }

        public string? RawType { get; set; }
        public List<RequestHeaderDto> RequestHeaders { get; set; } = new();
        public List<RequestParameterDto> RequestParameters { get; set; } = new();
    }
}
