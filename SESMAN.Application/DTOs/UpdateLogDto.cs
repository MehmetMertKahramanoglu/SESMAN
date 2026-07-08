using System;
using System.Collections.Generic;
using System.Text;

namespace SESMAN.Application.DTOs
{
    public class UpdateLogDto //PUT ve PATCH için
    {
        public string? Url { get; set; }
        public string? Method { get; set; }
        public string? RequestBody { get; set; }

        public List<HeaderDto>? Headers { get; set; } = new();
        public List<ParameterDto>? Parameters { get; set; } = new();
    }
}
