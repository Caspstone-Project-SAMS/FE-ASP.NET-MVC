namespace WebMVCApplication.Entities;

public class Schedule
{
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public int SlotNumber { get; set; }
    public int Status { get; set; }
    public string AttendedStudent { get; set; } = "0/0";
    public int Attended { get; set; }

    public Room? Room { get; set; }
    public Subject? Subject { get; set; }
    public Class? Class { get; set; }
}
