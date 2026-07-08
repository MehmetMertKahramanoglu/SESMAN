using System;
using System.Collections.Generic;
using System.Text;

namespace SESMAN.Application.DTOs
{
    public class RequestLogDto //GET için
    {
        public Guid Id { get; set; }
        public string? Url { get; set; }
        public string? Method { get; set; }

        public string? RequestBody { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<HeaderDto> Headers { get; set; } = new();
        public List<ParameterDto> Parameters { get; set; } = new();
    }
}
