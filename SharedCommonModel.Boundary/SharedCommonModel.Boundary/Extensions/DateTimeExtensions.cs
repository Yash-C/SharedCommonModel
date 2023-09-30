namespace SharedCommonModel.Boundary.Extensions;

public static class DateTimeExtensions
{
    public static IEnumerable<DateTime> EachMonth(this DateTime start, DateTime to)
    {
        var from = start;
        while (from <= to)
        {
            yield return from;
            from = from.AddMonths(1);
        }
    }

    public static bool IsLastDayOfMonth(this DateTime dt)
    {
        return dt.AddDays(1).Day == 1;
    }

    /// <summary>
    /// Pluralize the string if required.
    /// </summary>
    private static string ToQuantity(this string input, int quantity) => quantity == 1 ? $"{quantity} {input}" : $"{quantity} {input}s";


    /// <summary>
    /// Compare a date and time with the current date and time and return a nice user friendly string like '5 days' or '20 minutes'.
    /// </summary>
    public static string GetDelta(this DateTime dt, bool future, bool stopAtDays = false)
    {
        // Get the UTC time - rounded up to the nearest minute. The rounding is because we are comparing it to 
        // a value that has been pulled from a "smalldatetime" field in the database - which doesn't keep track of seconds.
        var now = DateTime.UtcNow.Ceil(new TimeSpan(0, 1, 0));
        var span = future ? dt - now : now - dt;

        if (!future && dt > now) { return "In the future"; }
        if (future && dt < now) { return "In the past"; }

        if (span.TotalHours < 1) { return "minute".ToQuantity(span.Minutes); }

        if (span.TotalHours < 24)
        {
            // Use half an hour as the cutoff point. Ie. if it is 2:35:00 then report 3 hours else if its 2:28:00 then report 2 hours.
            if (span.Minutes <= 30)
            {
                return "hour".ToQuantity(span.Hours);
            }

            return (span.Hours + 1).ToString() + " hours";

        }

        if (span.TotalDays <= 31 || stopAtDays)
        {
            return span.Hours <= 12 ? "day".ToQuantity(span.Days) : span.Days + " days";
        }

        if (span.TotalDays <= 365)
        {
            var months = (int)Math.Round((decimal)span.Days / 30, 0);
            return "month".ToQuantity(months);
        }

        var years = (int)Math.Round(((decimal)span.Days / 365), 0);
        return "year".ToQuantity(years);
    }

    /// <summary>
    /// Compare a date and time with the current date and time and return a nice user friendly string like '5 days' or '20 minutes'.
    /// </summary>
    public static string GetDeltaDays(this DateTime dt, bool future)
    {
        // Get the UTC time - rounded up to the nearest minute. The rounding is because we are comparing it to 
        // a value that has been pulled from a "smalldatetime" field in the database - which doesn't keep track of seconds.
        DateTime now = DateTime.UtcNow.Ceil(new TimeSpan(0, 1, 0));
        var span = future ? dt - now : now - dt;

        return span.Days == 1 ? "1 day" : (span.Days + " days");
    }
    
    public static bool IsDeltaTwentyFourHoursOrMore(this DateTime oldDate, DateTime newDate, bool future)
    {
        // Get the UTC time - rounded up to the nearest minute. The rounding is because we are comparing it to 
        // a value that has been pulled from a "smalldatetime" field in the database - which doesn't keep track of seconds.
        DateTime daily = newDate.Ceil(new TimeSpan(24, 0, 0));
        var span = future ? oldDate - daily : daily - oldDate;

        return span.Days >= 1;
        
    }

    public static DateTime ConvertFromUtc(this DateTime dt, TimeZoneInfo tzi)
    {
        return TimeZoneInfo.ConvertTimeFromUtc(dt, tzi);
    }

    /// <summary>
    /// This version of addMonths is designed to always give you the last day of the month as the result. ie. If you start with June 30th and 
    /// add 1 month it gives you the 31st of July rather than the 30th of July.
    /// </summary>
    /// <param name="date">Must be the last day of a month</param>
    public static DateTime AddMonthsCustom(this DateTime date, int months)
    {
        if (date.Day != DateTime.DaysInMonth(date.Year, date.Month)) { throw new Exception("Must be the last day of the month."); }

        return date.AddDays(1).AddMonths(months).AddDays(-1);
    }

    public static DateTime AddYearsCustom(this DateTime date, int years) { return date.AddMonthsCustom(years * 12); }

    public static DateTime Ceil(this DateTime date, TimeSpan span)
    {
        var ticks = (date.Ticks + span.Ticks - 1) / span.Ticks;
        return new DateTime(ticks * span.Ticks);
    }

    /// <summary>
    /// Given a datetime - return a string in the format "3rd March 1979" which includes the ordinal ('rd','th','nd', etc...) value for the day.
    /// </summary>
    public static string ToOrdinalString(this DateTime date)
    {
        return date.Day.AddOrdinal() + " " + date.ToString("MMMM yyyy");
    }

    /// <summary>
    /// Given a datetime - return a string in the format "3rd March 1979 3:45pm" which includes the ordinal ('rd','th','nd', etc...) value for the day.
    /// </summary>
    public static string ToOrdinalStringWithTime(this DateTime date)
    {
        return date.Day.AddOrdinal() + " " + date.ToString("MMMM yyyy h:mm") + date.ToString("tt").ToLower();
    }

    public static int DaysUntil(this DateTime date, string timeZoneId, DateTime? forceNow = null)
    {
        var tzi = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);

        var todayOrganisationTime = forceNow == null ? TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tzi) : (DateTime)forceNow;
        var today12amOrganisationTime = new DateTime(todayOrganisationTime.Year, todayOrganisationTime.Month, todayOrganisationTime.Day, 0, 0, 0);

        var thisDateOrganisationTime = TimeZoneInfo.ConvertTimeFromUtc(date, tzi);
        var thisDate12amOrganisationTime = new DateTime(thisDateOrganisationTime.Year, thisDateOrganisationTime.Month, thisDateOrganisationTime.Day, 0, 0, 0);

        var span = thisDate12amOrganisationTime - today12amOrganisationTime;
        return span.Days;
    }

    public static DateTime Earliest(this DateTime dt, DateTime dt2) { return dt < dt2 ? dt : dt2; }

    public static DateTime Latest(this DateTime dt, DateTime dt2) { return dt > dt2 ? dt : dt2; }

    /// <summary>
    /// Convert a UTC DateTime to a unix timestamp (seconds from 1/1/1970)
    /// </summary>
    public static long ToUnixTimestamp(this DateTime dt)
    {
        return (long)(dt - DateTime.UnixEpoch).TotalSeconds;
    }

    /// <summary>
    /// Convert a nullable UTC DateTime to a unix timestamp (seconds from 1/1/1970), if DateTime is not null; else returns null
    /// </summary>
    public static long? ToUnixTimestamp(this DateTime? dt)
    {
        return dt != null ? (long)((DateTime)dt - DateTime.UnixEpoch).TotalSeconds : (long?)null;
    }

    /// <summary>
    /// Convert a UTC DateTime to a unix timestamp (seconds from 1/1/1970)
    /// </summary>
    public static long ToUnixTimestampMilliseconds(this DateTime dt)
    {
        return (long)(dt - DateTime.UnixEpoch).TotalMilliseconds;
    }

    /// <summary>
    /// Convert a nullable UTC DateTime to a unix timestamp (seconds from 1/1/1970), if DateTime is not null; else returns null
    /// </summary>
    public static long? ToUnixTimestampMilliseconds(this DateTime? dt)
    {
        return dt != null ? (long)((DateTime)dt - DateTime.UnixEpoch).TotalMilliseconds : (long?)null;
    }

    /// <summary>
    /// Convert Unix timestamp to a DateTime object
    /// </summary>
    /// <param name="unixTimestamp">The Unix timestamp you want to convert to DateTime</param>
    /// <returns>Returns a DateTime object that represents value of the Unix timestamp</returns>
    public static DateTime ToDateTime(this long unixTimestamp)
    {
        return DateTime.UnixEpoch.AddSeconds(unixTimestamp);
    }

    /// <summary>
    /// Convert nullable Unix timestamp to a DateTime object if Unix timestamp is not null; else returns null
    /// </summary>
    /// <param name="unixTimestamp">The Unix timestamp you want to convert to DateTime or null</param>
    /// <returns>Returns a DateTime object that represents value of the Unix timestamp or null</returns>
    public static DateTime? ToDateTime(this long? unixTimestamp)
    {
        return unixTimestamp != null ? DateTime.UnixEpoch.AddSeconds((long)unixTimestamp) : (DateTime?)null;
    }

    /// <summary>
    /// Convert Unix timestamp to a DateTime object
    /// </summary>
    /// <param name="unixTimestamp">The Unix timestamp you want to convert to DateTime</param>
    /// <returns>Returns a DateTime object that represents value of the Unix timestamp</returns>
    public static DateTime ToDateTimeFromMilliseconds(this long unixTimestamp)
    {
        return DateTime.UnixEpoch.AddMilliseconds(unixTimestamp);
    }

    /// <summary>
    /// Convert nullable Unix timestamp to a DateTime object if Unix timestamp is not null; else returns null
    /// </summary>
    /// <param name="unixTimestamp">The Unix timestamp you want to convert to DateTime or null</param>
    /// <returns>Returns a DateTime object that represents value of the Unix timestamp or null</returns>
    public static DateTime? ToDateTimeFromMilliseconds(this long? unixTimestamp)
    {
        return unixTimestamp != null ? DateTime.UnixEpoch.AddMilliseconds((long)unixTimestamp) : (DateTime?)null;
    }

    public static DateTime ToDateTime(this string dateString)
        => DateTime.Parse(dateString);

    public static DateTime? ToNullableDateTime(this string dateString)
        => String.IsNullOrWhiteSpace(dateString)
            ? null
            : DateTime.TryParse(dateString, out DateTime result)
                ? result
                : null;

    public static string GetFinancialQuarterName(this DateTime date, int firstMonthOfFinancialYear)
    {
        var quarter = Math.Ceiling((((date.Month - (decimal)firstMonthOfFinancialYear + 12) % 12) + 1) / 3);
        return "Q" + quarter + " " + date.GetFinancialYearName(firstMonthOfFinancialYear);
    }

    public static string GetFinancialYearName(this DateTime date, int firstMonthOfFinancialYear)
    {
        return firstMonthOfFinancialYear == 1
                   ? date.Year.ToString()
                   : (date.Month < firstMonthOfFinancialYear
                          ? date.AddYears(-1).Year + "/" + date.Year
                          : date.Year + "/" + date.AddYears(1).Year);
    }
    public static DateTime DateFromBasePlusMilliseconds(this string millisecondsText)
    {
        if (!long.TryParse(millisecondsText, out var milliseconds))
            throw new ArgumentException($"{nameof(millisecondsText)} could not be converted to a long number from {millisecondsText}");

        return DateFromBasePlusMilliseconds(milliseconds);
    }
    public static DateTime? DateFromBasePlusMillisecondsNullable(this string millisecondsText)
    {
        if (!long.TryParse(millisecondsText, out var milliseconds))
            return null;

        return DateFromBasePlusMilliseconds(milliseconds);
    }
    public static DateTime DateFromBasePlusMilliseconds(this long milliseconds)
    {
        return DateTime.UnixEpoch.AddMilliseconds(milliseconds);
    }
    public static long ToMillisecondsAfterBase(this DateTime date)
    {
        return (long)((date - DateTime.UnixEpoch).TotalMilliseconds);
    }

    public static long ToMillisecondsAfterBaseDateOnly(this DateTime date)
    {
        DateTime dt = DateTime.ParseExact(date.ToString("yyyy-MM-dd"), "yyyy-MM-dd", CultureInfo.InvariantCulture);
        return (long)((dt - DateTime.UnixEpoch).TotalMilliseconds);
    }

    public static string ToFormattedDate(this DateTime date, string formatString)
        => date.ToString(formatString);

    public static string ToFormattedDate(this DateTime? date, string formatString)
        => date == null ? "" : date.Value.ToFormattedDate(formatString);
    //private static DateTime BaseDate => new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);

}
