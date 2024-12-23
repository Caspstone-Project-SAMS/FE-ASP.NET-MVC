namespace WebMVCApplication.Models;

public class ResultViewModel
{
    public PopupNotification? PopupNotification { get; set; }
    public LoginUser? LoginUser { get; set; }
}

public class PopupNotification
{
    public bool IsSuccess { get; set; }
    public string Title { get; set; } = string.Empty;
    public IEnumerable<string> Description { get; set; } = new List<string>();
}

public class LoginUser
{
    public string Token { get; set; } = string.Empty;
    public string Id { get; set; } = string.Empty;
    public string UserRole { get; set; } = string.Empty;
    public string? UserName { get; set; } = "User";
    public string? Avatar { get; set; }
}
