using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

[ApiController]
[Route("api/[controller]")]
public class LineBotController : ControllerBase
{
    private readonly LineBotOptions _options;

    public LineBotController(IOptions<LineBotOptions> options)
    {
        _options = options.Value;
    }

    [HttpPost]
    public IActionResult Post()
    {
        // TODO: 實作訊息處理邏輯
        return Ok("Webhook received");
    }
}