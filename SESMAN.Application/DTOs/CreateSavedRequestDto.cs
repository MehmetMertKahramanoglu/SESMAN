using SESMAN.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SESMAN.Application.DTOs
{
    public class CreateSavedRequestDto
    {
        public required string Name { get; set; } 
        public Guid CollectionId { get; set; } // Hangi klasöre kaydoluyor?
        public required string Url { get; set; }
        public HttpMethodType Method { get; set; }
        public string? Body { get; set; }

        public string? BodyType { get; set; }

        public string? RawType { get; set; }

        // İstek kaydedilirken header ve parametre şablonları da beraberinde gelsin
        public ICollection<SavedRequestHeaderDto> SavedRequestHeaders { get; set; } = new List<SavedRequestHeaderDto>();
        public ICollection<SavedRequestParameterDto> SavedRequestParameters { get; set; } = new List<SavedRequestParameterDto>();


        //burada HAS-A yapısı ile vue tarafında veri göndereceğim zaman paket halinde düzenli şekilde göndermeyi yaptım.
        public AuthConfigDto? Auth { get; set; }
    }
}
