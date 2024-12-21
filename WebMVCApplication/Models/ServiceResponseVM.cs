using System.Text.Json.Serialization;

namespace WebMVCApplication.Models;

public class ServiceResponseVM<T> where T : class
{
    [JsonIgnore]
    public bool IsSuccess { get; set; } = false;
    public string? Title { get; set; }
    public IEnumerable<string>? Errors { get; set; }
    [JsonIgnore]
    public T? Result { get; set; }
}

public class ServiceResponseVM
{
    [JsonIgnore]
    public bool IsSuccess { get; set; } = false;
    public string? Title { get; set; }
    public IEnumerable<string> Errors { get; set; } = new List<string>();
}
