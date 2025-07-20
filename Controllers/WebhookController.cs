using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Text.Json;
using LineBotAI.Models;
using LineBotAI.Services;

namespace LineBotAI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebhookController : ControllerBase
    {
        private readonly ILogger<WebhookController> _logger;
        private readonly LineBotService _lineBotService;
        private readonly LineBotOptions _lineBotOptions;

        // 用來記錄已處理過的 webhook event id，防止重複處理
        private static readonly ConcurrentDictionary<string, bool> _processedEvents = new();

        public WebhookController(
            ILogger<WebhookController> logger,
            LineBotService lineBotService,
            IOptions<LineBotOptions> options)
        {
            _logger = logger;
            _lineBotService = lineBotService;
            _lineBotOptions = options.Value;
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync()
        {
            try
            {
                // 取得 X-Line-Signature header
                var signature = Request.Headers["X-Line-Signature"];
                if (string.IsNullOrEmpty(signature))
                {
                    _logger.LogWarning("Missing X-Line-Signature header.");
                    return BadRequest();
                }

                // 讀取 Request Body
                using var reader = new StreamReader(Request.Body);
                var body = await reader.ReadToEndAsync();

                // 驗證簽章
                if (!_lineBotService.VerifySignature(body, signature))
                {
                    _logger.LogWarning("Signature validation failed.");
                    return Unauthorized();
                }

                // 解析 JSON
                var payload = JsonSerializer.Deserialize<LineWebhookPayload>(body);
                if (payload?.events == null || !payload.events.Any())
                {
                    _logger.LogInformation("No events in payload.");
                    return Ok();
                }

                foreach (var evt in payload.events)
                {
                    // 防呆：避免重複處理相同的 event
                    if (!_processedEvents.TryAdd(evt.@eventId ?? Guid.NewGuid().ToString(), true))
                    {
                        _logger.LogWarning($"Duplicate event skipped: {evt.@eventId}");
                        continue;
                    }

                    // 呼叫服務處理邏輯
                    await _lineBotService.HandleWebhookEventAsync(evt);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Webhook processing error.");
                return StatusCode(500);
            }
        }
    }
}
