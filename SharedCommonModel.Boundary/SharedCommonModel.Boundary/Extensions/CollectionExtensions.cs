namespace SharedCommonModel.Boundary.Extensions;

public static class CollectionExtensions
{
    public static string CommaSeparate<T>(this IEnumerable<T> source)
    {
        return string.Join(",", source.Select(s => s.ToString()).ToArray());
    }

    public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
        => !(source?.Any() ?? false);


    public static HashSet<T> ToHashset<T>(this IEnumerable<T> source)
    {
        return new HashSet<T>(source);
    }

    public  static void AddCommaSeparatedValues(this IList<string> collection, string commaSeparatedValues)
    {
        var valuesSplit = commaSeparatedValues.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var value in valuesSplit)
        {
            var trimmedValue = value.Trim();
            if (!string.IsNullOrEmpty(trimmedValue))
            {
                collection.Add(trimmedValue);
            }
        }
    }
    public static Span<T> ToSpanNet6<T>(this IEnumerable<T> source) => new (source.ToArray());
}

public static class EnumeratedValueExtensions
{

    /// <summary>
    /// Checks if the given enum object is defined (exists) within its expected enum type
    /// </summary>
    public static bool IsDefined(this Enum enumObj)
    {
        return Enum.IsDefined(enumObj.GetType(), enumObj);
    }

    /// <summary>
    /// Get comma seperated string which contains all the constant names, defined within the type of the enum object
    /// </summary>
    public static string GetEnumConstantNames(this Enum enumObj)
    {
        return string.Join(", ", Enum.GetNames(enumObj.GetType()));
    }

}