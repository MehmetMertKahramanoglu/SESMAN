using System;
using System.Collections.Generic;
using System.Text;

namespace SESMAN.Application.DTOs
{
    public class ResponseLogDto : BaseDto
    {
        public Guid RequestLogId { get; set; } // Hangi isteğin cevabı olduğunu bilmek için
        public int StatusCode { get; set; }
        public string? Body { get; set; }
        public long ExecutionTimeMs { get; set; } // Cevap süresi
        public DateTime CreatedAt { get; set; }

        public List<ResponseHeaderDto> ResponseHeaders { get; set; } = new();
    }
}
