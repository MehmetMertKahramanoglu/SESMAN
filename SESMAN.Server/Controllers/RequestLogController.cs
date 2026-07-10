using Microsoft.AspNetCore.Mvc;
using SESMAN.Application.DTOs;
using SESMAN.Application.Interfaces;

namespace SESMAN.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestLogController : ControllerBase
    {
        private readonly IRequestLogService _service;

        public RequestLogController(IRequestLogService service)
        {
            _service = service;
        }

        [HttpGet] //GET METODU
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();

            if (result == null || !result.Any()) //veritabanında kayıt olmaması durumunda hata kontrolü
            {
                return NoContent();
            }

            return Ok(result);
        }

        [HttpGet("{id}")] // GET METODU Id ile çalışan
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound("İlgili kayıt bulunamadı.");
            return Ok(result);
        }

        [HttpPost] //POST 
        public async Task<IActionResult> Create([FromBody] CreateRequestLogDto dto)
        {
            if (dto == null)
            {
                return BadRequest("Boş veri gönderilemez."); // 400
            }

            try
            {
                await _service.CreateAsync(dto);
                return Ok("Kayıt başarıyla oluşturuldu.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Kayıt oluşturulurken bir hata meydana geldi.");
            }
        }

        [HttpPut("{id}")] //PUT 
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRequestLogDto dto)
        {
            if (dto == null)
            {
                return BadRequest("Gönderilen veri boş olamaz.");
            }

            if (id != dto.Id)
            {
                return BadRequest("URL'deki ID ile gönderilen verideki ID uyuşmuyor");
            }

            try
            {
                await _service.UpdateAsync(id, dto);
                return Ok("Kayıt başarıyla güncellendi.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Güncelleme sırasında bir hata meydana geldi.");
            }
        }

        [HttpPatch("{id}")] //PATCH 
        public async Task<IActionResult> Patch(Guid id, [FromBody] UpdateRequestLogDto dto)
        {
            if (dto == null)
            {
                return BadRequest("Gönderilen veri boş olamaz.");
            }

            try
            {
                await _service.PatchAsync(id, dto);
                return Ok("Kayıt kısmen güncellendi.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Kısmi güncelleme sırasında bir hata meydana geldi.");
            }
        }

        [HttpDelete("{id}")] //DELETE
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return Ok("Kayıt başarıyla silindi.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Silme işlemi sırasında bir hata meydana geldi.");
            }
        }
    }
}