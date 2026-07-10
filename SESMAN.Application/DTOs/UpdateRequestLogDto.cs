using System;
using System.Collections.Generic;
using System.Text;

namespace SESMAN.Application.DTOs
{
    public class UpdateRequestLogDto : BaseDto //PUT ve PATCH için
    {
        public string? Url { get; set; }
        public string? Method { get; set; }
        public string? Body { get; set; }

        public List<RequestHeaderDto> RequestHeaders { get; set; } = new();
        public List<RequestParameterDto> RequestParameters { get; set; } = new();
    }
}
