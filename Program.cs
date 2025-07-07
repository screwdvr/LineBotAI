var builder = WebApplication.CreateBuilder(args);

// 加入 MVC 控制器服務
builder.Services.AddControllersWithViews();

// Swagger 支援
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 啟用 Swagger（僅開發環境）
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // 若日後有 View 可使用靜態檔案

app.UseRouting();

app.UseAuthorization();

// 設定 MVC 路由
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
