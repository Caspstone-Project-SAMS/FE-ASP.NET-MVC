using System.Text.Json;
using WebMVCApplication.Common;
using WebMVCApplication.Entities;
using WebMVCApplication.Models;

namespace WebMVCApplication.Services;

public interface IScheduleService
{
    Task<ServiceResponseVM<List<Schedule>>> GetSchedules(string? lecturerId, int? semesterId, string? bearerToken, 
        DateTime? startDate, DateTime? endDate, int startPage, int endPage, int quantity);
}


public class ScheduleService : IScheduleService
{
    private readonly IApiHelper _apiHelper;
    public ScheduleService(IApiHelper apiHelper)
    {
        _apiHelper = apiHelper;
    }

    public async Task<ServiceResponseVM<List<Schedule>>> GetSchedules(string? lecturerId, int? semesterId, string? bearerToken,
        DateTime? startDate, DateTime? endDate, int startPage, int endPage, int quantity)
    {
        string apiEndpoint = $"/api/Schedule?startPage={startPage}&endPage={endPage}&quantity={quantity}";
        if (lecturerId != null) apiEndpoint += $"&lecturerId={lecturerId}";
        if (semesterId != null) apiEndpoint += $"&semesterId={semesterId}";
        if (startDate != null) apiEndpoint += $"&startDate={startDate.Value.ToString("yyyy-MM-dd")}";
        if (endDate != null) apiEndpoint += $"&endDate={endDate.Value.ToString("yyyy-MM-dd")}";

        var response = await _apiHelper.CallGetApi(apiEndpoint, bearerToken);
        var jsonResponse = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true // Enable case-insensitive matching
            };
            //var result = JsonSerializer.Deserialize<ServiceResponseVM<List<Schedule>>>(jsonResponse, options) ??
                
            return new ServiceResponseVM<List<Schedule>>
            {
                IsSuccess = false,
                Title = "Get schedules failed",
            };
        }

        using var document = JsonDocument.Parse(jsonResponse);
        var root = document.RootElement;

        var schedules = new List<Schedule>();
        if(root.ValueKind == JsonValueKind.Array)
        {
            for(int i = 0; i < root.GetArrayLength(); i++)
            {
                var item = root[i];
                schedules.Add(new Schedule
                {
                    Id = item.GetProperty("scheduleID").GetInt32(),
                    Date = DateOnly.Parse(item.GetProperty("date").GetString() ?? string.Empty),
                    StartTime = TimeOnly.ParseExact(item.GetProperty("startTime").GetString() ?? string.Empty, "HH:mm:ss"),
                    EndTime = TimeOnly.ParseExact(item.GetProperty("endTime").GetString() ?? string.Empty, "HH:mm:ss"),
                    SlotNumber = item.GetProperty("slotNumber").GetInt32(),
                    Status = item.GetProperty("scheduleStatus").GetInt32(),
                    AttendedStudent = item.GetProperty("attendStudent").GetString() ?? string.Empty,
                    Attended = item.GetProperty("attended").GetInt32(),
                    Room = new Room
                    {
                        Name = item.GetProperty("roomName").GetString() ?? ""
                    },
                    Subject = new Subject
                    {
                        Code = item.GetProperty("subjectCode").GetString() ?? ""
                    },
                    Class = new Class
                    {
                        ClassCode = item.GetProperty("classCode").GetString() ?? ""
                    }
                });
            }
        }

        return new ServiceResponseVM<List<Schedule>>
        {
            IsSuccess = true,
            Result = schedules,
        };
    }
}
