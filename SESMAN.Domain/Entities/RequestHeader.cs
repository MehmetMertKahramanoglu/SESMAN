using System;
using System.Collections.Generic;
using System.Text;

namespace SESMAN.Domain.Entities
{
    public class RequestHeader : BaseEntity
    {
        public Guid RequestLogId { get; set; }
        public required string Key {  get; set; } //Authorization, Content-lenght vb
        public required string Value { get; set; } //Bearer ey... , 1234 vb
    }
}
