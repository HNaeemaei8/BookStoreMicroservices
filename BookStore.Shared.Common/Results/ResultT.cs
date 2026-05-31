namespace BookStore.Shared.Common.Results;

public sealed class Result<T> : Result
{
    internal Result(T value, bool isSuccess, Error error)
        : base(isSuccess, error)
    {
        Value = value;
    }

    public T Value { get; }
}