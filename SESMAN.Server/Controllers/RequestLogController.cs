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
            return Ok(result);
        }

        [HttpGet("{id}")] // GET METODU Id ile çalışan
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost] //POST 
        public async Task<IActionResult> Create([FromBody] CreateLogDto dto)
        {
            await _service.CreateAsync(dto);
            return Ok("Kayıt başarıyla oluşturuldu.");
        }

        [HttpPut("{id}")] //PUT 
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateLogDto dto)
        {
            await _service.UpdateAsync(id, dto);
            return Ok("Kayıt başarıyla güncellendi.");
        }

        [HttpPatch("{id}")] //PATCH 
        public async Task<IActionResult> Patch(Guid id, [FromBody] UpdateLogDto dto)
        {
            await _service.PatchAsync(id, dto);
            return Ok("Kayıt kısmen güncellendi.");
        }

        [HttpDelete("{id}")] //DELETE
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.DeleteAsync(id);
            return Ok("Kayıt başarıyla silindi.");
        }
    }
}