namespace SharedCommonModel.Boundary.Extensions;

public static class ObjectExtensions
{
    public static bool IsCollectionExcludingStringAndByte(this Type typeToTest)
    {
        var isCollection = typeof(IEnumerable).IsAssignableFrom(typeToTest);
        var isString = typeToTest.Name == "String";
        var isByteArray = typeToTest.Name == "Byte[]";
        
        return isCollection && !isString && !isByteArray;
    }

    public static bool IsCustomClass(this Type type)
    {
        var isCollection = typeof(IEnumerable).IsAssignableFrom(type);
        var isClassNotDefinitive = type.IsClass;
        var isString = type.Name == "String";
        var isByteArray = type.Name == "Byte[]";
        var isObject = type.Name == "Object";

        return
            isClassNotDefinitive &&
            !isCollection &&
            !isString &&
            !isByteArray &&
            !isObject;
    }
        
    /// <summary>
    /// If this object is already property infos, then return these, unchanged. Otherwise, get the property infos of this object's type. 
    /// </summary>
    public static PropertyInfo[] GetPropertyInfos(this object target)
    {
        return 
            target is PropertyInfo[] propertyInfos
                ? propertyInfos
                : target.GetType().GetProperties();
    }

    public static object GenerateListOfGenericType(this Type type)
    {
        var listInstance = (IList)typeof(List<>)
            .MakeGenericType(type)
            .GetConstructor(Type.EmptyTypes)
            .Invoke(null);
        return listInstance;
    }


    /// <summary>
    /// General method for all value types, to check if a nullable object has value == null or its default value
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static bool IsNullOrDefault<T>(this T? obj) where T : struct
    {
        return !obj.HasValue || obj.Value.Equals(default(T));
    }

    /// <summary>
    /// General method for all value types, to check if a non nullable object has value == default value
    /// <para>Using method overload, to handle both nullable and non nullable object extensions</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static bool IsNullOrDefault<T>(this T obj) where T : struct
    {
        return obj.Equals(default(T));
    }

    /// <summary>
    /// General method for all string array
    /// </summary>
    /// <param name="arr"></param>
    /// <returns></returns>
    public static string StringArrayToJson(this string[] arr)
    {
        return System.Text.Json.JsonSerializer.Serialize(arr);
    }
}