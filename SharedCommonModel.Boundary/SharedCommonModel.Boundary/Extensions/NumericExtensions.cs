namespace SharedCommonModel.Boundary.Extensions;

public static class NumericExtensions
{
    /// <summary>
    /// Format a decimal to two decimal places if they exist - ie. 100.2 becomes 100.20, and 100.00 becomes 100.
    /// </summary>
    public static string FormatToTwoDecimalPlacesIfTheyExist(this decimal x)
    {
        var s = $"{x:0.00}";

        if (s.EndsWith("00"))
        {
            return ((int)x).ToString();
        }

        return s;
    }

    public static int PercentileOfValue(this List<decimal> values, decimal value, bool isHigherBetter)
    {
        // If there is only one value - then we assume that this is the best result - hence 100.
        if (values.Count == 1) { return 100; }

        var orderedValues = values.OrderBy(x => x).ToList();

        // Start from the best result
        if (isHigherBetter)
        {
            // Loop through to find the position
            for (var i = orderedValues.Count - 1; i >= 0; i--)
            {
                // The conversion to decimal is necessary to get an accurate result.
                if (orderedValues[i] == value)
                {
                    return Convert.ToInt32(100 / ((decimal)orderedValues.Count - 1) * i);
                }
                // we've gone too far and not found the value. Calculate a percentile based on where it would be
                else if (orderedValues[i] < value)
                {
                    // If this is the first value is the first value in the list - then the value is not in the list.
                    if (i == (orderedValues.Count - 1))
                    {
                        // this is not data from our dataset, but return 100th percentile anyway
                        return 100;
                    }
                    else
                    {
                        var higherP = 100 / ((decimal)orderedValues.Count - 1) * (i + 1);
                        var lowerP = 100 / ((decimal)orderedValues.Count - 1) * i;

                        return Convert.ToInt32(Math.Round(lowerP + (higherP - lowerP) * ((value - orderedValues[i]) / (orderedValues[i + 1] - orderedValues[i])), 0, MidpointRounding.AwayFromZero));
                    }
                }
            }
            return 0;
        }
        else
        {
            for (var i = 0; i < orderedValues.Count; i++)
            {
                if (orderedValues[i] == value)
                {
                    return Convert.ToInt32(100 / ((decimal)orderedValues.Count - 1) * ((decimal)orderedValues.Count - 1 - i));
                }
                else if (orderedValues[i] > value)
                {
                    if (i == 0)
                    {
                        // this is not data from our dataset, but return 100th percentile anyway
                        return 100;
                    }
                    else
                    {
                        var higherP = Convert.ToDecimal(100 - (100 / ((decimal)orderedValues.Count - 1))) * (i - 1);
                        var lowerP = Convert.ToDecimal(100 - (100 / ((decimal)orderedValues.Count - 1))) * i;

                        return Convert.ToInt32(Math.Round(lowerP + (higherP - lowerP) * ((orderedValues[i] - value) / (orderedValues[i] - orderedValues[i - 1])), 0, MidpointRounding.AwayFromZero));
                    }
                }
            }
            return 0;
        }
    }

    // Uses Alternative method used by excel to calculate a percentile value from a vector of numbers
    // see http://en.wikipedia.org/wiki/Percentile
    // Retrieve the value at the given percentile of the provided list of values. There is no need to sort the list before passing it in.
    public static decimal GetValueAtPercentile(this List<decimal> values, decimal percentile, bool isHigherBetter)
    {
        if (values.Count == 0) { throw new Exception("values cannot be an empty list"); }

        var orderedValues = values.OrderBy(x => x).ToList();

        // calculate rank n (has range 1 to thisValues.length)
        var n = (isHigherBetter ? percentile : (100 - percentile)) / 100 * (orderedValues.Count - 1) + 1;

        // n = k (integer component) + d (decimal componant )
        var k = Convert.ToInt32(Math.Floor(n)); // n is always positive
        var d = n - k;

        // n start range is 1, but orderedValues is 0 indexed
        if (n == 1)
        {
            return orderedValues[0];
        }

        if (n == orderedValues.Count)
        {
            return orderedValues[orderedValues.Count - 1];
        }

        return orderedValues[k - 1] + d * (orderedValues[k] - orderedValues[k - 1]);
    }

    public static int GetRank(this List<decimal> values, decimal value, bool isHigherBetter)
    {
        if (!values.Contains(value))
        {
            throw new Exception("The value was not found in the collection of values. Unable to calcualte rank.");
        }

        return isHigherBetter
            ? values.OrderByDescending(x => x).ToList().IndexOf(value) + 1
            : values.OrderBy(x => x).ToList().IndexOf(value) + 1;
    }

    public static string AddOrdinal(this int num)
    {
        switch (num % 100)
        {
            case 11:
            case 12:
            case 13:
                return num.ToString() + "th";
        }

        switch (num % 10)
        {
            case 1:
                return num.ToString() + "st";
            case 2:
                return num.ToString() + "nd";
            case 3:
                return num.ToString() + "rd";
            default:
                return num.ToString() + "th";
        }

    }

    /// <summary>
    /// https://stackoverflow.com/questions/3874627/floating-point-comparison-functions-for-c-sharp
    /// </summary>
    public static bool NearlyEqual(this float a, float b, float unimportantDifference = 0.0000001f)
    {
        // if not equal
        const float floatNormal = (1 << 23) * float.Epsilon;
        var absA = Math.Abs(a);
        var absB = Math.Abs(b);
        var diff = Math.Abs(a - b);

        if (a == b)
        {
            // Shortcut, handles infinities
            return true;
        }

        if (a == 0.0f || b == 0.0f || diff < floatNormal)
        {
            // a or b is zero, or both are extremely close to it.
            // relative error is less meaningful here
            return diff < (unimportantDifference * floatNormal);
        }

        // use relative error
        return diff / Math.Min((absA + absB), float.MaxValue) < unimportantDifference;
    }

    /// <summary>
    /// https://stackoverflow.com/questions/3874627/floating-point-comparison-functions-for-c-sharp
    /// </summary>
    public static bool NearlyEqual(this double a, double b, double unimportantDifference = 0.0000001)
    {
        // if not equal
        const double doubleNormal = (1 << 23) * double.Epsilon;
        var absA = Math.Abs(a);
        var absB = Math.Abs(b);
        var diff = Math.Abs(a - b);

        if (a == b)
        {
            // Shortcut, handles infinities
            return true;
        }

        if (a == 0.0f || b == 0.0f || diff < doubleNormal)
        {
            // a or b is zero, or both are extremely close to it.
            // relative error is less meaningful here
            return diff < (unimportantDifference * doubleNormal);
        }

        // use relative error
        return diff / Math.Min((absA + absB), double.MaxValue) < unimportantDifference;
    }

    /// <summary>
    /// https://stackoverflow.com/questions/3874627/floating-point-comparison-functions-for-c-sharp
    /// </summary>
    public static bool NearlyEqual(this decimal a, decimal b, decimal unimportantDifference = 0.0000001m)
    {
        // if not equal
        const decimal doubleNormal = (1 << 23) * (decimal)double.Epsilon;
        var absA = Math.Abs(a);
        var absB = Math.Abs(b);
        var diff = Math.Abs(a - b);

        if (a == b)
        {
            // Shortcut, handles infinities
            return true;
        }

        if (a == 0.0m || b == 0.0m || diff < doubleNormal)
        {
            // a or b is zero, or both are extremely close to it.
            // relative error is less meaningful here
            return diff < (unimportantDifference * doubleNormal);
        }

        // use relative error
        return diff / Math.Min((absA + absB), decimal.MaxValue) < unimportantDifference;
    }
    public static long ParseLong(this string value, long defaultIntValue = 0)
    {
        var result = defaultIntValue;
        if (int.TryParse(value, out var parsedInt))
        {
            result = parsedInt;
        }
        return result;
    }

}