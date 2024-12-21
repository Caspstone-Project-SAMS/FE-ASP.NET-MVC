namespace WebMVCApplication.Entities;

public class Class
{
    public int Id { get; set; }
    public string ClassCode { get; set; } = string.Empty;
    public int Status { get; set; }
    public string SlotDuration { get; set; } = string.Empty;
    public SlotType? SlotType { get; set; }
    public Semester? Semester { get; set; }
    public Room? Room { get; set; }
    public Subject? Subject { get; set; }
    public Lecturer? Lecturer { get; set; }
}
