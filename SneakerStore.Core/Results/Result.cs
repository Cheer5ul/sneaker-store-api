using SneakerStore.Core.Results.Errors;

namespace SneakerStore.Core.Results;

public class Result
{
    private Result(bool isSuccess, Error error)
    {
        if (isSuccess == true && error != Error.None ||
            !isSuccess && error == Error.None)
        {
            throw new ArgumentException("Invalid error", nameof(error)); 
        }
    }
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; } 
}