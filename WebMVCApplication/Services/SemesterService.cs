using System.Text.Json;
using WebMVCApplication.Common;
using WebMVCApplication.Entities;
using WebMVCApplication.Models;

namespace WebMVCApplication.Services;

public interface ISemesterService
{
    Task<ServiceResponseVM<Semester>> GetActiveSemester(string? bearerToken);
}

public class SemesterService : ISemesterService
{
    private readonly IApiHelper _apiHelper;
    public SemesterService(IApiHelper apiHelper)
    {
        _apiHelper = apiHelper;
    }

    public async Task<ServiceResponseVM<Semester>> GetActiveSemester(string? bearerToken)
    {
        var response = await _apiHelper.CallGetApi("/api/Semester/all?semesterStatus=2", bearerToken);
        var jsonResponse = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true // Enable case-insensitive matching
            };
            var result = JsonSerializer.Deserialize<ServiceResponseVM<Semester>>(jsonResponse, options) ??
                new ServiceResponseVM<Semester>
                {
                    IsSuccess = false,
                    Title = "Login failed",
                    Errors = new string[1] { "Invalid credentials" }
                };
            return result;
        }

        

        return null;
    }
}
