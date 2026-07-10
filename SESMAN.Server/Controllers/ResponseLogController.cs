using Microsoft.AspNetCore.Mvc;
using SESMAN.Application.DTOs;
using SESMAN.Application.Interfaces;

namespace SESMAN.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResponseLogController : ControllerBase
    {
        private readonly IResponseLogService _service;

        public ResponseLogController(IResponseLogService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            if (result == null || !result.Any()) return NoContent(); //eğer veri tabanında hiç kayıt yoksa hata kontrolü (204)
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound("İlgili kayıt bulunamadı.");
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateResponseLogDto dto)
        {
            if (dto == null) return BadRequest("Gönderilen veri boş olamaz.");

            try
            {
                await _service.CreateAsync(dto);
                return Ok("Cevap kaydı başarıyla oluşturuldu.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Kayıt oluşturulurken bir hata meydana geldi.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateResponseLogDto dto)
        {
            if (dto == null) return BadRequest("Gönderilen veri boş olamaz.");

            if (id != dto.Id)
            {
                return BadRequest("URL'deki ID ile gönderilen verideki ID uyuşmuyor");
            }

            try
            {
                await _service.UpdateAsync(id, dto);
                return Ok("Cevap kaydı başarıyla güncellendi.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Güncelleme sırasında bir hata meydana geldi.");
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(Guid id, [FromBody] UpdateResponseLogDto dto)
        {
            if (dto == null) return BadRequest("Gönderilen veri boş olamaz.");

            try
            {
                await _service.PatchAsync(id, dto);
                return Ok("Yanıt kaydı (Response) başarıyla kısmen güncellendi.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Kısmi güncelleme sırasında bir hata meydana geldi.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return Ok("Cevap kaydı başarıyla silindi.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Silme işlemi sırasında bir hata meydana geldi.");
            }
        }
    }
}
