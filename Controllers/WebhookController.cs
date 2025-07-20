using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using LineBotAI.Models;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Linq;

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
        public async Task<IActionResult> Post()
        {
            _logger.LogInformation("收到 LINE Webhook 請求");

            // 讀取請求內容
            using var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();

            // 取得簽章
            var xLineSignature = Request.Headers["X-Line-Signature"].FirstOrDefault();
            if (!IsValidSignature(body, xLineSignature))
            {
                _logger.LogWarning("簽章驗證失敗！");
                return Unauthorized(); // 401
            }

            _logger.LogInformation("簽章驗證成功！");
            _logger.LogInformation("Request Body: {Body}", body);

            // 📌 這裡可以加入後續處理訊息邏輯

            return Ok();
        }

        private bool IsValidSignature(string body, string? xLineSignature)
        {
            if (string.IsNullOrEmpty(xLineSignature))
                return false;

            var secret = Encoding.UTF8.GetBytes(_lineOptions.ChannelSecret);
            var bodyBytes = Encoding.UTF8.GetBytes(body);

            using var hmac = new HMACSHA256(secret);
            var hash = hmac.ComputeHash(bodyBytes);
            var signature = Convert.ToBase64String(hash);

            return signature == xLineSignature;
        }
    }
}
