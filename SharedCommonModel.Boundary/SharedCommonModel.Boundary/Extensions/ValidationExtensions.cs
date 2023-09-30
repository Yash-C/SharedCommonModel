namespace SharedCommonModel.Boundary.Extensions;

public static class ValidationExtensions
{
    public static string ValidateArgumentNull(this string messageBody)
    {
        if (string.IsNullOrEmpty(messageBody) || string.IsNullOrWhiteSpace(messageBody))
            throw new ArgumentNullException(nameof(messageBody));

        return messageBody;
    }

    public static void ToValidateArgumentNullExceptionForStringList(this IEnumerable<string> messageBody)
    {
        if(messageBody.IsNullOrEmpty())
            throw new ArgumentNullException(nameof(messageBody));
    }

    private static bool isFragmented(IEnumerable<string> fragment)
    {
        return fragment.Any();
    }


    private static System.Collections.Generic.IEnumerable<string> first = new List<string>();

}