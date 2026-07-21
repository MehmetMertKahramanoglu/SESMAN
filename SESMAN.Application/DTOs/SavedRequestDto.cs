using SESMAN.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SESMAN.Application.DTOs
{
    public class SavedRequestDto : BaseDto
    {
        public required string Name { get; set; }
        public Guid CollectionId { get; set; }
        public required string Url { get; set; }
        public HttpMethodType Method { get; set; }
        public string? Body { get; set; }

        public string? BodyType { get; set; }

        public string? RawType { get; set; }

        public virtual ICollection<SavedRequestHeaderDto> SavedRequestHeaders { get; set; } = new List<SavedRequestHeaderDto>();
        public virtual ICollection<SavedRequestParameterDto> SavedRequestParameters { get; set; } = new List<SavedRequestParameterDto>();
    }
}
