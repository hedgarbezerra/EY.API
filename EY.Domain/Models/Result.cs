using Newtonsoft.Json;

namespace EY.Domain.Models;

public class Result<T>
{
    [JsonConstructor]
    private Result(bool successful, T? data, List<string> errors, List<string> successes)
    {
        Successful = successful;
        Data = data;
        Errors = errors;
        Successes = successes;
    }

    public bool Successful { get; }
    public T? Data { get; }
    public List<string> Errors { get; } = new();
    public List<string> Successes { get; } = new();

    public static Result<T> Success(T? data, params string[] successes)
    {
        return new Result<T>(true, data, [], successes?.ToList() ?? []);
    }

    public new static Result<T> Failure(params string[] errors)
    {
        return new Result<T>(false, default, errors?.ToList() ?? [], []);
    }

    public static implicit operator Result(Result<T> result)
    {
        return result.Successful ? Result.Success(result.Successes.ToArray()) : Result.Failure(result.Errors.ToArray());
    }
}

public class Result
{
    [JsonConstructor]
    protected Result(bool successful, List<string> errors, List<string> successes)
    {
        Successful = successful;
        Errors = errors;
        Successes = successes;
    }

    public bool Successful { get; }
    public List<string> Errors { get; } = new();
    public List<string> Successes { get; } = new();

    public static Result Success(params string[] successes)
    {
        return new Result(true, [], successes?.ToList() ?? []);
    }

    public static Result Failure(params string[] errors)
    {
        return new Result(false, errors?.ToList() ?? [], []);
    }
}