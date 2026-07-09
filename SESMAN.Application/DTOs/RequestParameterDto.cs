using System;
using System.Collections.Generic;
using System.Text;

namespace SESMAN.Application.DTOs
{
    public class RequestParameterDto : BaseDto
    {
        public required string Key { get; set; } //required derleme zamanında çalışır hata varsa compile anında fark edilir.
        public required string Value { get; set; }
    }
}
