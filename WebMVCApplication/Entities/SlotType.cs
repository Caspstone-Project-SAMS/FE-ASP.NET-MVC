namespace WebMVCApplication.Entities;

public class SlotType
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Status { get; set; }
    public int SessionCount { get; set; }
}
