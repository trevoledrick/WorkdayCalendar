namespace WorkdayCalendar;

public class WorkdayCalendar : IWorkdayCalculator
{
    private readonly IWorkdaySettings _workdaySettings;
    private readonly IHolidayCalendar _holidayCalendar;

    public WorkdayCalendar(IWorkdaySettings workdaySettings, IHolidayCalendar holidayCalendar)
    {
        _workdaySettings = workdaySettings;
        _holidayCalendar = holidayCalendar;
    }

    public DateTime AddWorkdays(DateTime start, double workdays)
    {
        if (workdays == 0)
        {
            return NormalizeForward(start);
        }

        int direction = workdays > 0 ? 1 : -1;
        decimal absoluteWorkdays = Math.Abs((decimal)workdays);

        int wholeDays = (int)decimal.Truncate(absoluteWorkdays);
        decimal fractionalPart = absoluteWorkdays - wholeDays;

        DateTime current = direction > 0 ? NormalizeForward(start) : NormalizeBackward(start);

        for (int i = 0; i < wholeDays; i++)
        {
            if (direction > 0)
            {
                current = MoveToNextWorkday(current.AddDays(1));
            }
            else
            {
                current = MoveToPreviousWorkday(current.AddDays(-1));
            }
        }

        if (fractionalPart == 0)
        {
            return current;
        }

        var fractionalTicksDecimal = (decimal)_workdaySettings.WorkdayLength.Ticks * fractionalPart;
        var fractionalTicks = (long)Math.Round(fractionalTicksDecimal, MidpointRounding.AwayFromZero);
        var fractionalDuration = TimeSpan.FromTicks(fractionalTicks);

        if (direction > 0)
        {
            current = AddWorkingTimeForward(current, fractionalDuration);
        }
        else
        {
            current = AddWorkingTimeBackward(current, fractionalDuration);
        }

        return current;
    }

    private bool IsWeekend(DateTime date)
    {
        return date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
    }

    private bool IsWorkday(DateTime date)
    {
        return !IsWeekend(date) && !_holidayCalendar.IsHoliday(date);
    }
    
    private DateTime MoveToNextWorkday(DateTime date)
    {
        var result = date;
        while (!IsWorkday(result))
        {
            result = result.AddDays(1);
        }

        return result;
    }

    private DateTime MoveToPreviousWorkday(DateTime date)
    {
        var result = date;
        while (!IsWorkday(result))
        {
            result = result.AddDays(-1);
        }

        return result;
    }

    private DateTime NormalizeForward(DateTime dateTime)
    {
        var date = dateTime.Date;
        var time = dateTime.TimeOfDay;

        if (!IsWorkday(date))
        {
            var nextWorkDay = MoveToNextWorkday(date);
            return nextWorkDay.Date + _workdaySettings.WorkdayStart;
        }

        if (time < _workdaySettings.WorkdayStart)
        {
            return date + _workdaySettings.WorkdayStart;
        }

        if (time > _workdaySettings.WorkdayEnd)
        {
            var nextWorkDay = MoveToNextWorkday(date.AddDays(1));
            return nextWorkDay.Date + _workdaySettings.WorkdayStart;
        }

        return dateTime;
    }

    private DateTime NormalizeBackward(DateTime dateTime)
    {
        var date = dateTime.Date;
        var time = dateTime.TimeOfDay;

        if (!IsWorkday(date))
        {
            var previousWorkDay = MoveToPreviousWorkday(date);
            return previousWorkDay.Date + _workdaySettings.WorkdayEnd;
        }

        if (time > _workdaySettings.WorkdayEnd)
        {
            return date + _workdaySettings.WorkdayEnd;
        }

        if (time < _workdaySettings.WorkdayStart)
        {
            var previousWorkDay = MoveToPreviousWorkday(date.AddDays(-1));
            return previousWorkDay.Date + _workdaySettings.WorkdayEnd;
        }

        return dateTime;
    }

    private DateTime AddWorkingTimeForward(DateTime dateTime, TimeSpan duration)
    {
        var remaining = duration;
        var current = dateTime;

        while (remaining > TimeSpan.Zero)
        {
            var workdayEndTime = current.Date + _workdaySettings.WorkdayEnd;
            var availableToday = workdayEndTime - current;

            if (availableToday <= TimeSpan.Zero)
            {
                var nextWorkDate = MoveToNextWorkday(current.Date.AddDays(1)).Date;
                current = nextWorkDate + _workdaySettings.WorkdayStart;
                continue;
            }

            if (remaining <= availableToday)
            {
                current = current + remaining;
                break;
            }
            
            remaining -= availableToday;
            var nextDay = MoveToNextWorkday(current.Date.AddDays(1)).Date;
            current = nextDay + _workdaySettings.WorkdayStart;
        }

        return current;
    }

    private DateTime AddWorkingTimeBackward(DateTime dateTime, TimeSpan duration)
    {
        var remaining = duration;
        var current = dateTime;

        while (remaining > TimeSpan.Zero)
        {
            var workdayStartTime = current.Date + _workdaySettings.WorkdayStart;
            var availableToday = current - workdayStartTime;

            if (availableToday <= TimeSpan.Zero)
            {
                var previousWorkDate = MoveToPreviousWorkday(current.Date.AddDays(-1)).Date;
                current = previousWorkDate + _workdaySettings.WorkdayEnd;
                continue;
            }

            if (remaining <= availableToday)
            {
                current = current - remaining;
                break;
            }
            
            remaining -= availableToday;
            var previousDay = MoveToPreviousWorkday(current.Date.AddDays(-1)).Date;
            current = previousDay + _workdaySettings.WorkdayEnd;
        }

        return current;
    }
}