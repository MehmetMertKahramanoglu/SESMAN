using System;
using System.Collections.Generic;
using System.Text;

namespace SESMAN.Domain.Entities
{
    public class ResponseHeader : BaseEntity
    {
        public Guid ResponseLogId { get; set; }

        public required string Key { get; set; }
        public required string Value { get; set; }
    }
}
