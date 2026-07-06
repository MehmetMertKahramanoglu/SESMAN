using System;
using System.Collections.Generic;
using System.Text;

namespace SESMAN.Domain.Entities
{
    public class ResponseLog : BaseEntity
    {
        public Guid RequestLogId { get; set; } //1-1 ilişki hangi isteğin yanıtı olduğunu tutacağım

        public int StatusCode { get; set; } //404, 200 vb
        public string? Body { get; set; } //gelen Json yanıt
        public long ExecutionTimeMs { get; set; } //yanıtın gelme süresi

        //1-n ilişki cevapla gelen header sayısı
        public virtual ICollection<ResponseHeader> Headers { get; set; } = new List<ResponseHeader>();

    }
}
