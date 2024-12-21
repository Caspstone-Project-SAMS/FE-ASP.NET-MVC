using WebMVCApplication.Entities;

namespace WebMVCApplication.Services;

public interface IClassService
{
    List<Class> GetClasses(string? lecturerId, int? semesterId, string? bearerToken);
}
public class ClassService : IClassService
{
    public List<Class> GetClasses(string? lecturerId, int? semesterId, string? bearerToken)
    {
        return new List<Class>();
    }
}
