using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Options;
using StudentLearningHistory.Context;
using StudentLearningHistory.Filters;
using StudentLearningHistory.Helpers;


var builder = WebApplication.CreateBuilder(args);
#region 服務註冊
//註冊MemoryCache ming
builder.Services.AddDistributedMemoryCache();

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//註冊cookie驗證 ming
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);//20分失效
        options.LoginPath = new PathString("/api/Auth/Login");//未登入導到登入
        options.AccessDeniedPath = new PathString("/api/Auth/NoAccess");//沒權限導到沒權限
    });

//註冊Context資訊給非controller可以呼叫到user資訊 ming
builder.Services.AddHttpContextAccessor();

//依照環境切換DB & 註冊跨源
builder.Services.AddCors(option => option.AddPolicy("any", builder =>
{
    builder.AllowAnyOrigin() //允許任何網域   
    .AllowAnyMethod()
    .AllowAnyHeader().WithExposedHeaders("Content-Disposition");
}));
if (builder.Environment.EnvironmentName == "Development")
{
    /*
     builder.Services.AddCors(options =>
     {    
         options.AddDefaultPolicy(builder =>
         {
             builder.AllowAnyMethod();
             builder.AllowAnyHeader();
             builder.AllowCredentials();
         });
     });
     */

    builder.Services.AddScoped<IDapperContext, DapperDevContext>();
}
else
{
    builder.Services.AddScoped<IDapperContext, DapperReleaseContext>();
}

//手動註冊helper
builder.Services.AddScoped<ZipHelper>();
builder.Services.AddScoped<FileHelper>();
builder.Services.AddScoped<MD5Helper>();

//註冊service ming
List<Type> profiles = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.Namespace == "StudentLearningHistory.Services" && t.IsVisible && t.IsPublic).ToList();
foreach (Type type in profiles)
{
    builder.Services.AddTransient(type);
}
profiles.Clear();

//註冊automapper ming
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddMvc(options =>
{
    options.Filters.Add(typeof(AuthorizationFilter));
    options.Filters.Add(typeof(ActionFilter));
});

//上傳檔案大小限制
builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = int.MaxValue;
});

builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = int.MaxValue; // if don't set default value is: 30 MB
});

builder.Services.Configure<FormOptions>(x =>
{
    x.ValueLengthLimit = int.MaxValue;
    x.MultipartBodyLengthLimit = int.MaxValue; 
    x.MultipartHeadersLengthLimit = int.MaxValue;
});
#endregion

//zeck
//builder.Services.ConfigureApplicationCookie(options =>
//{
//    options.Events.OnRedirectToAccessDenied =
//    options.Events.OnRedirectToLogin = context =>
//    {
//        if (context.Request.Method != "GET")
//        {
//            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
//            return Task.FromResult<object>(null);
//        }
//        context.Response.Redirect(context.RedirectUri);
//        return Task.FromResult<object>(null);
//    };
//});
//*****************************************************

var app = builder.Build();
#region server啟用
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting();

//跨源
/*if (app.Environment.IsDevelopment())
{
    app.UseCors("any");
}*/
app.UseCors("any");

//驗證授權順序不可以錯
//使用cookie
app.UseCookiePolicy();
//驗證
app.UseAuthentication();
//授權
app.UseAuthorization();

app.MapControllers();
#endregion

app.Run();