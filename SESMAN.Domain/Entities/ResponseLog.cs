using System;
using System.Collections.Generic;
using System.Text;

namespace SESMAN.Domain.Entities
{
    public class ResponseLog : BaseEntity
    {
        // 1-1 İlişki: Hangi isteğin yanıtı olduğu
        public Guid RequestLogId { get; set; }
        public virtual RequestLog? RequestLog { get; set; } // İsteğe geri dönebilmek için (Navigation Property)

        // Yanıt Detayları
        public int StatusCode { get; set; } // 404, 200 vb.
        public string? Body { get; set; } // Gelen Json yanıt
        public long ExecutionTimeMs { get; set; } // Yanıtın gelme süresi

        // 1-N İlişki: Cevapla gelen header'lar
        public virtual ICollection<ResponseHeader> Headers { get; set; } = new List<ResponseHeader>();
    }
}
