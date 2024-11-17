using Microsoft.AspNetCore.Authentication.Cookies;
using WebMVCApplication.Common;
using WebMVCApplication.Services;

var builder = WebApplication.CreateBuilder(args);

IConfigurationRoot Configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IApiHelper, ApiHelper>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddHttpClient("MyApi", client =>
{
    var config = Configuration["MyConfig:BackendApi"];
    client.BaseAddress = new Uri(config);
    client.DefaultRequestHeaders.Add("User-Agent", "MVC app");
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
    });
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
