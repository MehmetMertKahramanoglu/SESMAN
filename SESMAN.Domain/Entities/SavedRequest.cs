using SESMAN.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SESMAN.Domain.Entities
{
    public class SavedRequest: BaseEntity
    {
        public string? Name { get; set; }
        public required string Url { get; set; }
        public HttpMethodType Method { get; set; }
        public Guid CollectionId { get; set; }
        public string? Body { get; set; }

        public string? BodyType { get; set; }

        public string? RawType { get; set; }

        //1-n olunca ICollection yapısı kullanılır.
        public virtual ICollection<SavedRequestParameter> SavedRequestParameters { get; set; } = new List<SavedRequestParameter>();
        public virtual ICollection<SavedRequestHeader> SavedRequestHeaders { get; set; } = new List<SavedRequestHeader>();
    }
}
