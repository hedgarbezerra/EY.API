using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EY.Domain.Models
{
    public class Result<T>
    {
        public bool Successful { get; }
        public T? Data { get; }
        public List<string> Errors { get; } = new List<string>();
        public List<string> Successes { get; } = new List<string>();

        [JsonConstructor]
        private Result(bool successful, T? data, List<string> errors, List<string> successes)
        {
            Successful = successful;
            Data = data;
            Errors = errors;
            Successes = successes;
        }

        public static Result<T> Success(T? data, params string[] successes) => new Result<T>(true, data, [], successes?.ToList() ?? []);
        public new static Result<T> Failure(params string[] errors) =>
            new Result<T>(false, default, errors?.ToList() ?? [], []);

        public static implicit operator Result(Result<T> result) => result.Successful ? Result.Success(result.Successes.ToArray()) : Result.Failure(result.Errors.ToArray());
    }

    public class Result
    {
        public bool Successful { get; }
        public List<string> Errors { get; } = new List<string>();
        public List<string> Successes { get; } = new List<string>();

        [JsonConstructor]
        protected Result(bool successful, List<string> errors, List<string> successes)
        {
            Successful = successful;
            Errors = errors;
            Successes = successes;
        }

        public static Result Success(params string[] successes) => new Result(true, [], successes?.ToList() ?? []);
        public static Result Failure(params string[] errors) => new Result(false, errors?.ToList() ?? [], []);
    }
}
