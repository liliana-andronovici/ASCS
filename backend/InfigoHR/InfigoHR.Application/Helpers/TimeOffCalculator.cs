namespace InfigoHR.Application.Helpers;

public static class TimeOffCalculator
{
    public static int WorkingDays(DateTime start, DateTime end)
    {
        var days = 0;
        for (var date = start.Date; date <= end.Date; date = date.AddDays(1))
        {
            if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
            {
                days++;
            }
        }
        return days;
    }
}