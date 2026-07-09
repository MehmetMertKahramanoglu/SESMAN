using System;
using System.Collections.Generic;
using System.Text;

namespace SESMAN.Application.DTOs
{
    public class UpdateResponseLogDto
    {
        public int StatusCode { get; set; }
        public string? Body { get; set; }
        public long ExecutionTimeMs { get; set; }

        public List<ResponseHeaderDto>? ResponseHeaders { get; set; } = new();
    }
}
