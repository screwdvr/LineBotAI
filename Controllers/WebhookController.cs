using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using LineBotAI.Models;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Net.Http.Headers;

namespace LineBotAI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebhookController : ControllerBase
    {
        private readonly ILogger<WebhookController> _logger;
        private readonly LineBotOptions _lineBotSettings;

        public WebhookController(
            ILogger<WebhookController> logger,
            IOptions<LineBotOptions> lineBotSettings)
        {
            _logger = logger;
            _lineBotSettings = lineBotSettings.Value;
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            _logger.LogInformation("📩 收到 LINE Webhook 訊息");

            string body;
            using (var reader = new StreamReader(Request.Body))
            {
                body = await reader.ReadToEndAsync();
            }

            // 驗證簽章
            var xLineSignature = Request.Headers["X-Line-Signature"];
            var hash = ComputeHmacSha256(body, _lineBotSettings.ChannelSecret);
            if (xLineSignature != hash)
            {
                _logger.LogWarning("❌ 簽章驗證失敗");
                return Unauthorized();
            }

            // 解析 JSON 並處理事件
            var jsonDoc = JsonDocument.Parse(body);
            var events = jsonDoc.RootElement.GetProperty("events");

            foreach (var lineEvent in events.EnumerateArray())
            {
                var type = lineEvent.GetProperty("type").GetString();
                var replyToken = lineEvent.GetProperty("replyToken").GetString();

                if (type == "message" &&
                    lineEvent.GetProperty("message").GetProperty("type").GetString() == "text")
                {
                    var userMessage = lineEvent.GetProperty("message").GetProperty("text").GetString();
                    _logger.LogInformation("👤 使用者訊息：{Message}", userMessage);

                    string replyMessage;

                    if (userMessage.Contains("你好"))
                    {
                        replyMessage = "你好！我是 Nora 🤖";
                    }
                    else if (userMessage.Contains("提醒我"))
                    {
                        replyMessage = "請問你要提醒什麼事情呢？";
                    }
                    else if (userMessage.Contains("天氣"))
                    {
                        replyMessage = "想查哪裡的天氣呢？";
                    }
                    else
                    {
                        replyMessage = $"你說的是：「{userMessage}」，但我還不太懂唷～";
                    }

                    await ReplyText(replyToken, replyMessage);
                }
            }

            return Ok();
        }

        private static string ComputeHmacSha256(string data, string key)
        {
            var encoding = new UTF8Encoding();
            byte[] keyBytes = encoding.GetBytes(key);
            byte[] dataBytes = encoding.GetBytes(data);

            using (var hmac = new HMACSHA256(keyBytes))
            {
                byte[] hashBytes = hmac.ComputeHash(dataBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

        private async Task ReplyText(string replyToken, string message)
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _lineBotSettings.ChannelAccessToken);

            var body = new
            {
                replyToken = replyToken,
                messages = new[]
                {
                    new
                    {
                        type = "text",
                        text = message
                    }
                }
            };

            var json = JsonSerializer.Serialize(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("https://api.line.me/v2/bot/message/reply", content);
            _logger.LogInformation("LINE 回覆結果：{StatusCode}", response.StatusCode);
        }
    }
}
