using Microsoft.AspNetCore.Mvc;

namespace LineBotAI.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return Content("Hello, this is LineBotAI MVC App running on Azure Web App!");
        }
    }
}
