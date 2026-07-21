using System;
using System.Collections.Generic;
using System.Text;

namespace SESMAN.Application.DTOs
{
    public class CollectionDto: BaseDto
    {
        public required string Name { get; set; }
        public virtual ICollection<SavedRequestDto> SavedRequests { get; set; } = new List<SavedRequestDto>();
    }
}
