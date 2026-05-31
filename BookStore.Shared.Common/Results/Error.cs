using BookStore.Shared.Common.Results;

public sealed record Error(string Code, string Message, ErrorType Type)
{
    public static readonly Error None = new("", "", ErrorType.Failure);

    public static Error Validation(string message)
        => new("Validation", message, ErrorType.Validation);

    public static Error NotFound(string message)
        => new("NotFound", message, ErrorType.NotFound);

    public static Error Failure(string message)
        => new("Failure", message, ErrorType.Failure);
}