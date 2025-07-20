using Microsoft.AspNetCore.Mvc;
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
