using Microsoft.Extensions.Logging;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MTG.Core.Helper;

public interface IResult
{
    public Result<T> ToFailure<T>();
}

public class Result
{
    private static readonly ILogger<Result> _logger = LogManager.GetLogger<Result>();

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string Error { get; }

    protected Result(bool isSuccess, string error)
    {
        IsSuccess = isSuccess;
        Error = error;

        if (IsFailure)
            _logger.LogError($"{GetType}: {error}"); //TODO first bracket?
    }

    public static Result Success() => new(true, string.Empty);
    public static Result Failure(string error) => new(false, error);
    public Result<U> ToFailure<U>() => Result<U>.Failure(Error);
}

public class Result<T> : Result, IResult
{
    private readonly T? _value;
    private static readonly ILogger<Result> _logger = LogManager.GetLogger<Result>();

    private Result(T? value, bool isSuccess, string error) : base(isSuccess, error)
    {
        _value = value;
    }

    public T Value
    {
        get
        {
            if (IsSuccess)
            {
                return _value!;
            }
            else
            {
                _logger.LogError($"{typeof(T)}: {Error}"); //TODO first bracket?
                throw new InvalidOperationException("Cannot access Value of a failed result!");
            }
        }
    }

    public static Result<T> Success(T value) => new(value, true, string.Empty);
    public static new Result<T> Failure(string error) => new(default, false, error);
    public new Result<U> ToFailure<U>() => Result<U>.Failure(Error);
}
