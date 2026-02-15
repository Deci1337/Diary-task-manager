namespace DiaryTaskManagerApp.Infrastructure;

public static class TimeZones
{
    public static TimeZoneInfo GetMoscow()
    {
        // Windows: "Russian Standard Time", Android/Linux: "Europe/Moscow"
        try { return TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time"); } catch { }
        try { return TimeZoneInfo.FindSystemTimeZoneById("Europe/Moscow"); } catch { }
        return TimeZoneInfo.Local;
    }
}




