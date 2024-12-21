namespace WebMVCApplication.Entities;

public class Semester
{
    public int Id { get; set; }
    public string SemesterCode { get; set; } = string.Empty;
    public int Status { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
}
