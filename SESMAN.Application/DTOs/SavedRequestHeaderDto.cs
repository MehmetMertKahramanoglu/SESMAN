using System;
using System.Collections.Generic;
using System.Text;

namespace SESMAN.Application.DTOs
{
    public class SavedRequestHeaderDto: BaseDto
    {
        public Guid SavedRequestId { get; set; }
        public required string Key { get; set; }
        public required string Value { get; set; }
    }
}
