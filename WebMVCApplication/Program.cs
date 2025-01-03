using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using WebMVCApplication.Common;
using WebMVCApplication.Permission;
using WebMVCApplication.Services;

var builder = WebApplication.CreateBuilder(args);

IConfigurationRoot Configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson();

builder.Services.AddScoped<IApiHelper, ApiHelper>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ISemesterService, SemesterService>();
builder.Services.AddScoped<IClassService, ClassService>();
builder.Services.AddScoped<IScheduleService, ScheduleService>();


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
        options.LogoutPath = "/logout";
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy =>
    {
        policy.Requirements.Add(new HasScopeRequirement(UserRole.ADMIN, Configuration["Jwt:Issuer"]!));
    });

    options.AddPolicy("Lecturer", policy =>
    {
        policy.Requirements.Add(new HasScopeRequirement(UserRole.LECTURER, Configuration["Jwt:Issuer"]!));
    });

    options.AddPolicy("Student", policy =>
    {
        policy.Requirements.Add(new HasScopeRequirement(UserRole.STUDENT, Configuration["Jwt:Issuer"]!));
    });

    options.AddPolicy("Admin Lecturer", policy =>
    {
        policy.RequireAssertion(context =>
        {
            return context.User.HasClaim(claim =>

                (claim.Type == "scope") &&
                (
                    claim.Value.Equals(UserRole.ADMIN) ||
                    claim.Value.Equals(UserRole.LECTURER)
                )
            );
        });
    });
});

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

app.UseEndpoints(endpoints =>
{
    endpoints.MapPost("/logout", (HttpContext context) =>
    {
        context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        context.Response.Redirect("/");
    });
});

app.Run();
