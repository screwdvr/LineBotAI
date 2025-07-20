using LineBotAI.Models;

var builder = WebApplication.CreateBuilder(args);

// 讀取 LineBot 設定
builder.Services.Configure<LineBotOptions>(
    builder.Configuration.GetSection("LineBot"));

// 加入 MVC 控制器服務
builder.Services.AddControllersWithViews();

// Swagger 支援
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// 設定 MVC 預設路由
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// 加入 API 控制器路由（如 WebhookController）
app.MapControllers();

app.Run();
