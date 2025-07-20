using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

[ApiController]
[Route("api/[controller]")]
public class WebhookController : ControllerBase
{
    private readonly LineBotOptions _options;

    public WebhookController(IOptions<LineBotOptions> options)
    {
        _options = options.Value;
    }

    [HttpPost]
    public IActionResult Post()
    {
        // 回傳 OK 讓 LINE 不重送
        return Ok("Webhook received.");
    }
}