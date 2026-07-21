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
    public class CollectionController : ControllerBase
    {
        private readonly ICollectionService _service;

        public CollectionController(ICollectionService service)
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
            if (result == null) return NotFound("The relevant collection was not found.");
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCollectionDto dto)
        {
            if (dto == null)
            {
                return BadRequest("Empty data cannot be sent.");
            }

            try
            {
                await _service.CreateAsync(dto);
                return Ok("The collection was successfully created.");
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while creating the collection.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CreateCollectionDto dto)
        {
            if (dto == null)
            {
                return BadRequest("Gönderilen veri boş olamaz.");
            }

            try
            {
                //başlangıçta bu Id'ye sahip bir kayıt var mı diye kontrol ediyoruz.
                bool isUpdated = await _service.UpdateAsync(id, dto);
                if (!isUpdated)
                {
                    return NotFound("The request template to be deleted was not found.");
                }
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
                //başlangıçta bu Id'ye sahip bir kayıt var mı diye kontrol ediyoruz.
                bool isDeleted = await _service.DeleteAsync(id);
                if (!isDeleted)
                {
                    return NotFound("The request template to be deleted was not found.");
                }
                return Ok("The request template was successfully deleted.");
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred during the deletion process.");
            }
        }
    }
}