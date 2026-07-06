using System;
using System.Collections.Generic;
using System.Text;

namespace SESMAN.Domain.Entities
{
    public class RequestParameter : BaseEntity
    {
        public Guid RequestLogId { get; set; }
        public required string Key { get; set; } //required derleme zamanında çalışır hata varsa compile anında fark edilir.
        public required string Value { get; set; } 

    }
}
