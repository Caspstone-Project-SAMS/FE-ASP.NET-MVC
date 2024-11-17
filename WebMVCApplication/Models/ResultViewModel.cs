namespace WebMVCApplication.Models;

public class ResultViewModel
{
    public bool IsSuccess { get; set; }
    public string Title { get; set; } = string.Empty;
    public IEnumerable<string> Description { get; set; } = new List<string>();
}
