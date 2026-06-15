using SneakerStore.Core.Results.Errors;
using SneakerStore.Core.Results.Errors.Sneaker;

namespace SneakerStore.Core.Results;

public class Result
{
    protected Result(bool isSuccess, List<Error> errors)
    {
        if (isSuccess && errors.Count != 0 ||
            !isSuccess && errors.Count == 0)
        {
            throw new ArgumentException("Invalid error", nameof(errors)); 
        }
        
        IsSuccess = isSuccess;
        Errors = errors;
    }

    private bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public List<Error> Errors { get; } 
    
    public static Result Success() => new (true, []);
    public static Result Failure(List<Error> errors) => new(false, errors);
}

public class Result<T> : Result
{
    public T? Value { get; }

    private Result(T value) : base(true, [])
    {
        Value = value ?? throw new ArgumentNullException(nameof(value));
    }

    private Result(List<Error> errors) : base(false, errors) { }
    
    public static Result<T> Success(T value) => new(value);
    public new static Result<T> Failure(List<Error> errors) => new(errors);
}
