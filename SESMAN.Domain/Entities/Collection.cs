using System;
using System.Collections.Generic;
using System.Text;

namespace SESMAN.Domain.Entities
{
    public class Collection : BaseEntity
    {
        public string Name { get; set; }

        public virtual ICollection<SavedRequest> SavedRequests { get; set; } = new List<SavedRequest>();
    }
}
