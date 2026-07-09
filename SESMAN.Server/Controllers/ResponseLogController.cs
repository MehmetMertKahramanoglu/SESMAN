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
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateResponseLogDto dto)
        {
            await _service.CreateAsync(dto);
            return Ok("Cevap kaydı başarıyla oluşturuldu.");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateResponseLogDto dto)
        {
            await _service.UpdateAsync(id, dto);
            return Ok("Cevap kaydı başarıyla güncellendi.");
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(Guid id, [FromBody] UpdateResponseLogDto dto)
        {
            await _service.PatchAsync(id, dto);
            return Ok("Yanıt kaydı (Response) başarıyla kısmen güncellendi.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.DeleteAsync(id);
            return Ok("Cevap kaydı başarıyla silindi.");
        }
    }
}
