namespace SharedCommonModel.Boundary.Extensions;

public static class DictionaryExtensions
{
    // http://stackoverflow.com/questions/254178/c-sharp-dictionaries-valueornull-valueordefault
    public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default(TValue))
    {
        if (dictionary == null) { throw new ArgumentNullException(nameof(dictionary)); }
        if (key == null) { throw new ArgumentNullException(nameof(key)); }

        return dictionary.TryGetValue(key, out var value) ? value : defaultValue;
    }


    public static string GetValueAsString<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
    {
        if (dictionary is null) throw new ArgumentNullException(nameof(dictionary));
        if (key is null) throw new ArgumentNullException(nameof(key));

        var genericValue = GetValueOrDefault(dictionary, key);
        return genericValue == null ? default : genericValue.ToString();
    }
        
        
    public static void AddIfNotExisting<TKey, TValue>(
        this IDictionary<TKey, TValue> map, TKey key, TValue value)
    {
        if (map is null) throw new ArgumentNullException(nameof(map));
        if (key is null) throw new ArgumentNullException(nameof(key));

        if (!map.ContainsKey(key)) map.Add(key, value);
    }

    public static void SafeAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
    {
        if (dictionary is null) throw new ArgumentNullException(nameof(dictionary));
        if (key is null) throw new ArgumentNullException(nameof(key));

        if (dictionary.ContainsKey(key))
            dictionary[key] = value;
        else
            dictionary.Add(key, value);
    }

    public static IDictionary<TKey, TValue> MergeAdd<TKey, TValue>(this IDictionary<TKey, TValue> target,
        IDictionary<TKey, TValue> source, bool overwrite = true)
    {
        if (target == null) throw new ArgumentNullException(nameof(target));
        if (source == null) throw new ArgumentNullException(nameof(source));

        if (source.IsNullOrEmpty()) return target;

        foreach (var sourceKey in source.Keys)
        {
            if (target.ContainsKey(sourceKey))
            {
                if (overwrite)
                    target[sourceKey] = source[sourceKey];
            }
            else
                target.Add(sourceKey, source[sourceKey]);
        }

        return target;
    }
    public static IDictionary<TKey, TValue> MergeAdd<TKey, TValue>(this IDictionary<TKey, TValue> target,
        IEnumerable<KeyValuePair<TKey, TValue>> source, bool overwrite = true)
    {
        if (target == null) throw new ArgumentNullException(nameof(target));
        if (source == null) throw new ArgumentNullException(nameof(source));

        if (source.IsNullOrEmpty()) return target;

        foreach (var (key, value) in source)
        {
            if (target.ContainsKey(key))
            {
                if (overwrite)
                    target[key] = value;
            }
            else
                target.Add(key, value);
        }

        return target;
    }

    public static Dictionary<string, string> ToStandardisedDictionary(this IDictionary<string, string> source,
        IEqualityComparer<string> stringComparer)
        => source.ToStandardisedDictionary<string>(stringComparer);

    public static Dictionary<string, object> ToStandardisedDictionary(this IDictionary<string, object> source,
        IEqualityComparer<string> stringComparer)
        => source.ToStandardisedDictionary<object>(stringComparer);

    public static Dictionary<string, T> ToStandardisedDictionary<T>(this IDictionary<string, T> source,
        IEqualityComparer<string> stringComparer)
        => source.ToDictionary(s => s.Key, s => s.Value, stringComparer);

    public static IDictionary<string, object> ToPropertyDictionary(this object source, IEqualityComparer<string> stringComparer = null)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));
        stringComparer ??= StringComparer.InvariantCultureIgnoreCase;

        return source.GetType()
            .GetProperties()
            .Where(p => p.CanRead)
            .ToDictionary(p => p.Name, p => p.GetValue(source), stringComparer);
    }

    public static IDictionary<string, object> JsonToPropertyDictionary<T>(this string json, IEqualityComparer<string> stringComparer = null)
    {
        var model = System.Text.Json.JsonSerializer.Deserialize<T>(json);
        return model.ToPropertyDictionary(stringComparer);
    }

    public static T PopulatePropertiesFromDictionary<T>(this IReadOnlyDictionary<string, object> propertyDictionary,
        T targetObject)
        where T : class
    {
        if (propertyDictionary is null) throw new ArgumentNullException(nameof(propertyDictionary));
        if (targetObject is null) throw new ArgumentNullException(nameof(targetObject));

        var settableProperties = targetObject.GetType().GetProperties().Where(p => p.CanWrite).ToArray();

        var matchedProperties = settableProperties.Where(p => propertyDictionary.Keys.Contains(p.Name)).ToArray();

        foreach (var matchedProperty in matchedProperties)
        {
            matchedProperty.SetValue(targetObject, propertyDictionary[matchedProperty.Name]);
        }

        return targetObject;
    }

    public static T PopulateNewPropertiesFromDictionary<T>(this IReadOnlyDictionary<string, object> propertyDictionary)
        where T : class, new()
        => propertyDictionary.PopulatePropertiesFromDictionary(new T());

    public static bool IsMatchingDictionaries(IDictionary<string, object> source,
        IDictionary<string, object> target)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));
        if (target is null) throw new ArgumentNullException(nameof(target));

        if(ReferenceEquals(source, target)) return true;

        if (source.Count != target.Count) return false;

        if (source.Keys.Except(target.Keys).Any()) return false;
        
        foreach (var (key, value) in source)
        {
            if (!target.ContainsKey(key)) return false;

            if (target[key] != value) return false;
        }
        return true;
    }

    public static bool SafeRemove<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
    {
        if (dictionary is null) throw new ArgumentNullException(nameof(dictionary));
        if (key is null) throw new ArgumentNullException(nameof(key));

        if (!dictionary.ContainsKey(key)) return false;
        
        return dictionary.Remove(key);
    }
}