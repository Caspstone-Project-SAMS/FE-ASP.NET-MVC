using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Text.Json;
using WebMVCApplication.Common;
using WebMVCApplication.Models;

namespace WebMVCApplication.Services;


public interface IAuthService
{
    Task<ServiceResponseVM> Login(string username, string password);
}

public class AuthService : IAuthService
{
    private readonly IApiHelper _apiHelper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthService(IApiHelper apiHelper, IHttpContextAccessor httpContextAccessor)
    {
        _apiHelper = apiHelper;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ServiceResponseVM> Login(string username, string password)
    {
        var response = await _apiHelper.CallPostApi("/api/Auth/login", new
        {
            username, password
        }, null);
        var jsonResponse = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true // Enable case-insensitive matching
            };
            var result = JsonSerializer.Deserialize<ServiceResponseVM>(jsonResponse, options) ?? 
                new ServiceResponseVM
                {
                    IsSuccess = false,
                    Title = "Login failed",
                    Errors = new string[1] { "Invalid credentials" }
                };
            return result;
        }
        if(_httpContextAccessor.HttpContext is null)
        {
            return new ServiceResponseVM
            {
                IsSuccess = false,
                Title = "Login failed",
                Errors = new string[1] { "Can not login" }
            };
        }

        using var document = JsonDocument.Parse(jsonResponse);
        var root = document.RootElement;
        var resultPropery = root.GetProperty("result");

        var claims = new List<Claim>
        {
            new Claim("token", root.GetProperty("token").GetString() ?? ""),
            new Claim(ClaimTypes.NameIdentifier, resultPropery.GetProperty("id").GetString() ?? ""),
            new Claim("StudentId", resultPropery.GetProperty("studentID").GetString() ?? ""),
            new Claim("EmployeeId", resultPropery.GetProperty("employeeID").GetString() ?? ""),
            new Claim("email", resultPropery.GetProperty("email").GetString() ?? ""),
            new Claim("PhoneNumber", resultPropery.GetProperty("phoneNumber").GetString() ?? ""),
            new Claim("PhoneNumberConfirmed", resultPropery.GetProperty("phoneNumberConfirmed").GetRawText()),
            new Claim("Avatar", resultPropery.GetProperty("avatar").GetString() ?? ""),
            new Claim("DisplayName", resultPropery.GetProperty("displayName").GetString() ?? ""),
            new Claim("Address", resultPropery.GetProperty("address").GetString() ?? ""),
            new Claim("Dob", resultPropery.GetProperty("dob").GetString() ?? ""),
            new Claim("Gender", resultPropery.GetProperty("gender").GetRawText()),
            new Claim("FirstName", resultPropery.GetProperty("firstName").GetString() ?? ""),
            new Claim("LastName", resultPropery.GetProperty("lastName").GetString() ?? ""),
            new Claim("RoleId", resultPropery.GetProperty("role").GetProperty("roleId").GetRawText()),
            new Claim("Role", resultPropery.GetProperty("role").GetProperty("name").GetString() ?? "")
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        return new ServiceResponseVM
        {
            IsSuccess = true,
            Title = "Login successfully",
        };
    }
}
