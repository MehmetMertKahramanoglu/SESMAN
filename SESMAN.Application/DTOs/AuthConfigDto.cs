using System;
using System.Collections.Generic;
using System.Text;

namespace SESMAN.Application.DTOs
{
    public class AuthConfigDto
    {
        //başlangıçta none atadım. None gidince RestRequestService kısmında kontrol ile fonksiyone hiç girmemesini sağladım.
        public string Type { get; set; } = " none";

        //Basic doğrulama
        public string? Username { get; set; }
        public string? Password { get; set; }

        //Bearer ve OAuth 2.0 doğrulaması için
        public string? Token { get; set; }

        //Api Key doğrulaması için
        public string? ApiKeyName { get; set; }
        public string? ApiKeyValue { get; set; }
        public string? ApiKeyAddTo { get; set; }

    }
}
