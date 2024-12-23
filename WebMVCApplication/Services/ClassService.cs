using System.Text.Json;
using WebMVCApplication.Common;
using WebMVCApplication.Entities;
using WebMVCApplication.Models;

namespace WebMVCApplication.Services;

public interface IClassService
{
    Task<ServiceResponseVM<List<Class>>> GetClasses(string? lecturerId, int? semesterId, string? bearerToken);
}
public class ClassService : IClassService
{
    private readonly IApiHelper _apiHelper;

    public ClassService(IApiHelper apiHelper)
    {
        _apiHelper = apiHelper;
    }

    public async Task<ServiceResponseVM<List<Class>>> GetClasses(string? lecturerId, int? semesterId, string? bearerToken)
    {
        string apiEndpoint = "/api/Class?quantity=50";
        if (lecturerId is not null) apiEndpoint += $"&lecturerId={lecturerId}";
        if (semesterId is not null) apiEndpoint += $"&semesterId={semesterId}";

        var response = await _apiHelper.CallGetApi(apiEndpoint, bearerToken);
        var jsonResponse = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true // Enable case-insensitive matching
            };
            var result = JsonSerializer.Deserialize<ServiceResponseVM<List<Class>>>(jsonResponse, options) ??
                new ServiceResponseVM<List<Class>>
                {
                    IsSuccess = false,
                    Title = "Get classes failed",
                };
            return result;
        }

        using var document = JsonDocument.Parse(jsonResponse);
        var root = document.RootElement;
        var resultPropery = root.GetProperty("result");

        var classes = new List<Class>();
        if (resultPropery.ValueKind == JsonValueKind.Array)
        {
            for(int i = 0; i < resultPropery.GetArrayLength(); i++)
            {
                var item = resultPropery[i];
                classes.Add(new Class
                {
                    Id = item.GetProperty("classID").GetInt32(),
                    ClassCode = item.GetProperty("classCode").GetString() ?? "",
                    Status = item.GetProperty("classStatus").GetInt16(),
                    SlotDuration = $"{item.GetProperty("slotType").GetProperty("sessionCount").GetInt16() * 45} minutes",
                    Semester = new Semester
                    {
                        Id = item.GetProperty("semester").GetProperty("semesterID").GetInt32(),
                        SemesterCode = item.GetProperty("semester").GetProperty("semesterCode").GetString() ?? ""
                    },
                    Room = new Room
                    {
                        Id = item.GetProperty("room").GetProperty("roomID").GetInt32(),
                        Name = item.GetProperty("room").GetProperty("roomName").GetString() ?? ""
                    },
                    Subject = new Subject
                    {
                        Id = item.GetProperty("subject").GetProperty("subjectID").GetInt32(),
                        Code = item.GetProperty("subject").GetProperty("subjectCode").GetString() ?? "",
                        Name = item.GetProperty("subject").GetProperty("subjectName").GetString() ?? ""
                    }
                });
            }
        }

        return new ServiceResponseVM<List<Class>>
        {
            IsSuccess = true,
            Result = classes,
        };
    }
}
