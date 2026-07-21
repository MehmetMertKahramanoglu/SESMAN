using Microsoft.AspNetCore.Mvc;
using SESMAN.Application.DTOs;
using SESMAN.Application.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SESMAN.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SavedRequestController : ControllerBase
    {
        private readonly ISavedRequestService _service;

        public SavedRequestController(ISavedRequestService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();

            if (result == null || !result.Any())
            {
                return NoContent();
            }

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound("İlgili kayıtlı istek bulunamadı.");
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSavedRequestDto dto)
        {
            if (dto == null)
            {
                return BadRequest("Boş veri gönderilemez.");
            }

            try
            {
                await _service.CreateAsync(dto);
                return Ok("İstek şablonu başarıyla kaydedildi.");
            }
            catch (Exception)
            {
                return StatusCode(500, "İstek şablonu oluşturulurken bir hata meydana geldi.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CreateSavedRequestDto dto)
        {
            if (dto == null)
            {
                return BadRequest("Gönderilen veri boş olamaz.");
            }

            try
            {
                await _service.UpdateAsync(id, dto);
                return Ok("İstek şablonu başarıyla güncellendi.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Güncelleme sırasında bir hata meydana geldi.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return Ok("İstek şablonu başarıyla silindi.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Silme işlemi sırasında bir hata meydana geldi.");
            }
        }
    }
}