using System;
using System.Collections.Generic;
using System.Text;

namespace SESMAN.Domain.Entities
{
    public class AuthConfig 
    {
        public string Type { get; set; } = " none";
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Token { get; set; }
        public string? ApiKeyName { get; set; }
        public string? ApiKeyValue { get; set; }
        public string? ApiKeyAddTo { get; set; }

    }
}
