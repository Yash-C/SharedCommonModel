using System.Text;
using System.Text.RegularExpressions;

namespace SharedCommonModel.Boundary.Extensions;

public static class StringExtensions
{
    public static string ToCamelCaseString(this string @string)
    {
        if (string.IsNullOrEmpty(@string) || !char.IsUpper(@string[0]))
        {
            return @string;
        }

        string lowerCasedFirstChar = char.ToLower(@string[0], CultureInfo.InvariantCulture).ToString();

        if (@string.Length > 1)
        {
            lowerCasedFirstChar = lowerCasedFirstChar + @string.Substring(1);
        }

        return lowerCasedFirstChar;
    }
        
    public static string ToDelimitedString(this string @string, char delimiter)
    {
        var camelCaseString = @string.ToCamelCaseString();
        return new string(InsertDelimiterBeforeCaps(camelCaseString.ToCharArray(), delimiter).ToArray());
    }

    public static int ToNonNullInt(this string value) => int.TryParse(value, out var result) ? result : default;

    public static int? ToNullableInt(this string value) => int.TryParse(value, out var result) ? result : default;

    public static long ToLong(this string value) => long.TryParse(value, out var result) ? result : default;
    public static long? ToNullableLong(this string value) => long.TryParse(value, out var result) ? result : default;

    public static short ToShort(this string value) => short.TryParse(value, out var result) ? result : default;
    public static short? ToNullableShort(this string value) => short.TryParse(value, out var result) ? result : default;

    public static byte ToByte(this string value) => byte.TryParse(value, out var result) ? result : default;
    public static byte? ToNullableByte(this string value) => byte.TryParse(value, out var result) ? result : default;

    public static bool? ToNullableBool(this string value)
    {
        if (value is null)
            return null;
        return value.ToLower() == "true" || value.ToLower() == "yes";
    }
        
    public static string ToNonNullableYesNo(this bool? value, ToCase toCase = ToCase.TitleCase) 
        => value == null
            ? false.BoolToCased("Yes", "No", toCase)
            : ((bool)value).BoolToCased("Yes", "No", toCase);

    public static string ToNullableYesNo(this bool? value, ToCase toCase = ToCase.TitleCase)
        => value == null
            ? ""
            : ((bool)value).BoolToCased("Yes", "No", toCase);

    public static string ToNonNullableTrueFalse(this bool? value, ToCase toCase = ToCase.LowerCase)
        => value == null
            ? false.BoolToCased("True", "False", toCase)
            : ((bool)value).BoolToCased("True", "False", toCase);

    public static string ToNullableTrueFalse(this bool? value, ToCase toCase = ToCase.LowerCase)
        => value == null
            ? ""
            : ((bool)value).BoolToCased("True", "False", toCase);

    public static string ToYesNo(this bool value, ToCase toCase = ToCase.TitleCase)
        => value.BoolToCased("Yes", "No", toCase);

    public static string ToTrueFalse(this bool value, ToCase toCase = ToCase.LowerCase)
        => value.BoolToCased("True", "False", toCase);

    public static Guid ToNonNullableGuid(this Guid? value)
    {
        if (value == null)
            return new Guid();
        return (Guid) value;
    }


    /// <summary>
    /// Convert to a Guid
    /// </summary>
    /// <param name="value"></param>
    /// <returns>Empty Guid if null, overflowing string or any conversion exception.</returns>
    public static Guid ToGuidOrEmptyGuid(this string value)
        => Guid.TryParse(value, out var result) ? result : default;

    public static Guid? ToNullableGuid(this string value)
        => Guid.TryParse(value, out var result) ? result : default;


    public static float? ToNullableFloat(this string value)
    {
        if (float.TryParse(value, out var result))
            return result;
        return null;
    }

    public static decimal? ToNullableDecimal(this string value)
    {
        if (decimal.TryParse(value, out var result))
            return result;
        return null;
    }
        
    public static double? ToNullableDouble(this string value)
    {
        if (double.TryParse(value, out var result))
            return result;
        return null;
    }

    public static TimeZoneInfo TimeZoneTextToInfo(this string timeZone)
        => TimeZoneInfo.FindSystemTimeZoneById(timeZone);

    public static string TimeZoneTextNullableToOffsetHoursMinutes(this string timeZoneText)
        => String.IsNullOrWhiteSpace(timeZoneText)
            ? ""
            : timeZoneText.TimeZoneTextToInfo().TimeZoneInfoToOffsetHoursMinutes();

    public static string TimeZoneTextToOffsetHoursMinutes(this string timeZoneText)
        => timeZoneText.TimeZoneTextToInfo().TimeZoneInfoToOffsetHoursMinutes();

    public static string TimeZoneInfoToOffsetHoursMinutes(this TimeZoneInfo timeZoneInfo)
        => $"{timeZoneInfo.BaseUtcOffset.Hours:00}{timeZoneInfo.BaseUtcOffset.Minutes:00}";

    /// <summary>
    /// Match where - can match _ and CapitalCase can match underscore_case
    /// </summary>
    public static bool FuzzyMatch(this string apples, string pears)
    {
        return
            apples.Replace("-", "_").CamelCaseToLowerUnderscoreCase() ==
            pears.Replace("-", "_").CamelCaseToLowerUnderscoreCase() ||
            apples.Replace("-", "_").ToLower() == pears.Replace("-", "_").ToLower() ||
            apples.Replace("-", "").Replace("_", "").ToLower() == 
            pears.Replace("-", "_").Replace("_", "").ToLower();
    }
    public static string CamelCaseToLowerUnderscoreCase(this string value)
    {
        return string.Concat(value.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x : x.ToString())).ToLower();
    }

    public static string StripHtml(this string htmlString)
    {
        if (string.IsNullOrEmpty(htmlString))
            return "";

        try
        {
            // This regex approach to removing HTML tags isn't exhaustive but given that this text will just be used
            // to give an idea of what the email looks like, it is quicker and simpler than the correct DOM approach.
            var regex = new Regex(" ?<(?:\"[^\"]*\"['\"]*|'[^']*'['\"]*|[^'\">])+(?<!/\\s*)>");
            var withoutTags = regex.Replace(htmlString, string.Empty);
            var result = withoutTags.Replace("&nbsp;", "");
            return result;
        }
        catch
        {
            return htmlString;
        }
    }
        
    private static IEnumerable<char> InsertDelimiterBeforeCaps(IEnumerable<char> input, char delimiter)
    {
        bool lastCharWasUpper = false;

        foreach (char c in input)
        {
            if (char.IsUpper(c))
            {
                if (!lastCharWasUpper)
                {
                    yield return delimiter;
                    lastCharWasUpper = true;
                }
                yield return char.ToLower(c);
                continue;
            }

            yield return c;
            lastCharWasUpper = false;
        }
    }

    public static string DeUnicode(this string source)
        => Encoding.ASCII.GetString(
            Encoding.Convert(
                Encoding.UTF8,
                Encoding.ASCII,
                Encoding.UTF8.GetBytes(source)
            )
        );

    public enum ToCase
    {
        TitleCase,
        LowerCase,
        UpperCase
    };

    private static string BoolToCased(this bool value, string trueText, string falseText, ToCase toCase)
    {
        string text = value ? trueText : falseText;

        switch (toCase)
        {
            case ToCase.LowerCase:
                return text.ToLower();
            case ToCase.UpperCase:
                return text.ToUpper();
            case ToCase.TitleCase:
            default:
                return text;
        }
    }


    public static T ToTypedValue<T>(this string value)
    {
        if (typeof(T) == typeof(double?))
            return (T)(object)value.ToNullableDouble();

        if (typeof(T) == typeof(double))
            return (T)(object)value.ToNullableDouble();

        if (typeof(T) == typeof(decimal?))
            return (T)(object)value.ToNullableDecimal();

        if (typeof(T) == typeof(decimal))
            return (T)(object)value.ToNullableDecimal();

        if (typeof(T) == typeof(float?))
            return (T)(object)value.ToNullableFloat();

        if (typeof(T) == typeof(float))
            return (T)(object)value.ToNullableFloat();

        if (typeof(T) == typeof(long?))
            return (T)(object)value.ToNullableLong();

        if (typeof(T) == typeof(long))
            return (T)(object)value.ToLong();

        if (typeof(T) == typeof(int?))
            return (T)(object)value.ToNullableInt();

        if (typeof(T) == typeof(int))
            return (T)(object)value.ToNonNullInt();

        if (typeof(T) == typeof(short?))
            return (T)(object)value.ToNullableShort();

        if (typeof(T) == typeof(short))
            return (T)(object)value.ToShort();

        if (typeof(T) == typeof(byte?))
            return (T)(object)value.ToNullableByte();

        if (typeof(T) == typeof(byte))
            return (T)(object)value.ToByte();

        if (typeof(T) == typeof(DateTime?))
            return (T)(object)value.ToNullableDateTime();

        if (typeof(T) == typeof(DateTime))
            return (T)(object)value.ToDateTime();

        if (typeof(T) == typeof(Guid?))
            return (T)(object)value.ToNullableGuid();

        if (typeof(T) == typeof(Guid))
            return (T)(object)value.ToGuidOrEmptyGuid();

        return (T)(object)value;
    }
}