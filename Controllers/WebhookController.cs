using Microsoft.AspNetCore.Mvc;
<<<<<<< HEAD
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
=======
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using LineBotAI.Models;

namespace LineBotAI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebhookController : ControllerBase
    {
        private readonly ILogger<WebhookController> _logger;
        private readonly LineBotOptions _lineOptions;

        public WebhookController(
            ILogger<WebhookController> logger,
            IOptions<LineBotOptions> lineOptions)
        {
            _logger = logger;
            _lineOptions = lineOptions.Value;
        }

        [HttpPost]
        public IActionResult Post()
        {
            _logger.LogInformation("Received webhook from LINE.");
            _logger.LogInformation("Channel Secret: {Secret}", _lineOptions.ChannelSecret);
            _logger.LogInformation("Access Token: {Token}", _lineOptions.ChannelAccessToken.Substring(0, 10) + "...");
            
            // 後續：加入 LINE webhook 處理邏輯
            return Ok();
        }
    }
}
>>>>>>> 97313cdee81834d2903d4d0dc7c1e2b0cdef0a08
