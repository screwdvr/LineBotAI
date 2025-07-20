var builder = WebApplication.CreateBuilder(args);

// 讀取 LineBot 設定
builder.Services.Configure<LineBotOptions>(builder.Configuration.GetSection("LineBot"));

// 加入 MVC 控制器服務
builder.Services.AddControllers();

// 應用程式建構
var app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();