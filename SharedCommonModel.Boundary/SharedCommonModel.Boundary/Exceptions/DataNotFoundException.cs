namespace SharedCommonModel.Boundary.Exceptions;

public class DataNotFoundException : Exception
{
    public string DataModelType { get; }
    public string QueryParam { get; }


    public DataNotFoundException(string dataModelType, string queryParam) : base(MessageFromData(dataModelType, queryParam))
    {
        if (string.IsNullOrEmpty(dataModelType))
            throw new ArgumentException("Value cannot be null or empty.", nameof(dataModelType));

        if (string.IsNullOrEmpty(queryParam))
            throw new ArgumentException("Value cannot be null or empty.", nameof(queryParam));

        DataModelType = dataModelType;
        QueryParam = queryParam;

        Data.Add(nameof(DataModelType), DataModelType);
        Data.Add(nameof(QueryParam), QueryParam);
    }

    public DataNotFoundException(string dataModelType, string queryParam, string message) : base(MessageFromData(dataModelType, queryParam, message))
    {
        if (string.IsNullOrEmpty(dataModelType))
            throw new ArgumentException("Value cannot be null or empty.", nameof(dataModelType));

        if (string.IsNullOrEmpty(queryParam))
            throw new ArgumentException("Value cannot be null or empty.", nameof(queryParam));

        if (string.IsNullOrEmpty(message))
            throw new ArgumentException("Value cannot be null or empty.", nameof(message));

        DataModelType = dataModelType;
        QueryParam = queryParam;

        Data.Add(nameof(DataModelType), DataModelType);
        Data.Add(nameof(QueryParam), QueryParam);
    }

    private static string MessageFromData(string dataModelType, string queryParam)
        => $"Data not found for DataModel {dataModelType}, using query '{queryParam}'";

    private static string MessageFromData(string dataModelType, string queryParam, string message)
        => $"Data not found for DataModel {dataModelType}, using query '{queryParam}' {message}";
}