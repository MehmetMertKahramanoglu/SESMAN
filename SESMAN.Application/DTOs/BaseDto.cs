using System;
using System.Collections.Generic;
using System.Text;

namespace SESMAN.Application.DTOs
{
    public abstract class BaseDto
    {
        public Guid Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; } //güncellenme tarihi (null olabilir (hiç güncellenmemiş))
    }
}
