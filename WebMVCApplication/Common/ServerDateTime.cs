namespace WebMVCApplication.Common;

public static class ServerDateTime
{
    public static DateTime GetVnDateTime()
    {
        DateTime utcDateTime = DateTime.UtcNow;
        string vnTimeZoneKey = "SE Asia Standard Time";
        TimeZoneInfo vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById(vnTimeZoneKey);
        DateTime vnDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, vnTimeZone);
        return vnDateTime;
    }
}
