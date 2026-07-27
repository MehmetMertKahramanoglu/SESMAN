using Microsoft.AspNetCore.Mvc;
using SESMAN.Application.DTOs;
using SESMAN.Server;

[ApiController]
[Route("api/[controller]")]
public class IntegrationController : ControllerBase
{
    // HTTP isteklerinin servisini kullanmak için
    private readonly IRestRequestService _restRequestService;

    public IntegrationController(IRestRequestService restRequestService)
    {
        _restRequestService = restRequestService;
    }

    [HttpPost]
    public async Task<IActionResult> ExecuteRequest([FromBody] CreateRequestLogDto dto) //requestStore'dan gelen payload dto'nun içine konur. 
    {
        //url boş dönerse veya payload'ın içi boş dönerse diye kontrol
        if (dto == null || string.IsNullOrWhiteSpace(dto.Url))
        {
            return BadRequest("URL boş olamaz");
        }

        //gelen isteği servise gönderip hem servisi çalıştırıyorum hemde veritabanına kaydediyorum.
        var resultDto = await _restRequestService.ExecuteAndSaveRequestAsync(dto);

        //işlem sonunda UI tarafına status code ve ExecutionTimeMs için dönüş yapıyorum. RequestStore'a dönüş yapılır.
        return Ok(resultDto);
    }
}