using System.Text.Json;
using WebMVCApplication.Common;
using WebMVCApplication.Entities;
using WebMVCApplication.Models;

namespace WebMVCApplication.Services;

public interface ISemesterService
{
    Task<ServiceResponseVM<Semester>> GetActiveSemester(string? bearerToken);
    Task<ServiceResponseVM<List<Semester>>> GetAllSemester(string? bearerToken);
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
                    Title = "Get current semester failed",
                };
            return result;
        }

        using var document = JsonDocument.Parse(jsonResponse);
        var root = document.RootElement;
        var resultPropery = root.GetProperty("result");
        if(resultPropery.ValueKind == JsonValueKind.Null || resultPropery.GetArrayLength() <= 0)
        {
            return new ServiceResponseVM<Semester>
            {
                IsSuccess = true,
                Result = null
            };
        }
        var semesterResult = resultPropery[0];

        Semester semester = new Semester
        {
            Id = semesterResult.GetProperty("semesterID").GetInt32(),
            SemesterCode = semesterResult.GetProperty("semesterCode").GetString() ?? "",
            Status = semesterResult.GetProperty("semesterStatus").GetInt16(),
            StartDate = DateOnly.FromDateTime(semesterResult.GetProperty("startDate").GetDateTime()),
            EndDate = DateOnly.FromDateTime(semesterResult.GetProperty("endDate").GetDateTime())
        };
        return new ServiceResponseVM<Semester>
        {
            IsSuccess = true,
            Result = semester
        };
    }

    public async Task<ServiceResponseVM<List<Semester>>> GetAllSemester(string? bearerToken)
    {
        var response = await _apiHelper.CallGetApi("/api/Semester", bearerToken);
        var jsonResponse = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true // Enable case-insensitive matching
            };
            var result = JsonSerializer.Deserialize<ServiceResponseVM<List<Semester>>>(jsonResponse, options) ??
                new ServiceResponseVM<List<Semester>>
                {
                    IsSuccess = false,
                    Title = "Get all semester failed",
                };
            return result;
        }

        using var document = JsonDocument.Parse(jsonResponse);
        var resultPropery = document.RootElement;
        if (resultPropery.ValueKind == JsonValueKind.Null || resultPropery.GetArrayLength() <= 0)
        {
            return new ServiceResponseVM<List<Semester>>
            {
                IsSuccess = true,
                Result = null
            };
        }

        var semesters = new List<Semester>();
        for(int i = 0; i < resultPropery.GetArrayLength(); i++)
        {
            var semesterResult = resultPropery[i];

            Semester semester = new Semester
            {
                Id = semesterResult.GetProperty("semesterID").GetInt32(),
                SemesterCode = semesterResult.GetProperty("semesterCode").GetString() ?? "",
                Status = semesterResult.GetProperty("semesterStatus").GetInt16(),
                StartDate = DateOnly.FromDateTime(semesterResult.GetProperty("startDate").GetDateTime()),
                EndDate = DateOnly.FromDateTime(semesterResult.GetProperty("endDate").GetDateTime())
            };

            semesters.Add(semester);
        }
        
        return new ServiceResponseVM<List<Semester>>
        {
            IsSuccess = true,
            Result = semesters
        };
    }
}
