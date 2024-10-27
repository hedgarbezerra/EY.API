using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EY.Domain.Models
{
    public class Result<T> : Result
    {
        public T? Data { get; set; }
        [JsonConstructor]
        private Result(bool successful, T? data, List<string> errors, List<string> successes) :base(successful, errors, successes)
        {
            Successful = successful;
            Data = data;
            Errors = errors;
            Successes = successes;
        }

        public static Result<T> SuccessWithWarnings(T? data, List<string> successes = null, List<string> errors = null) =>
            new Result<T>(true, data, errors ?? [], successes ?? []);

        public static Result<T> Success(T? data, List<string> successes = null) =>
            new Result<T>(true, data, [], successes ?? []);
        public new static Result<T> Failure(List<string> errors = null) =>
            new Result<T>(false, default, errors ?? [], []);
    }

    public class Result
    {
        public bool Successful { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public List<string> Successes { get; set; } = new List<string>();

        [JsonConstructor]
        protected Result(bool successful, List<string> errors, List<string> successes)
        {
            Successful = successful;
            Errors = errors;
            Successes = successes;
        }

        public static Result SuccessWithWarnings(List<string> successes = null, List<string> errors = null) =>
            new Result(true, errors ?? [], successes ?? []);

        public static Result Success(List<string> successes = null) =>
            new Result(true, [], successes ?? []);
        public static Result Failure(List<string> errors = null) =>
            new Result(false, errors ?? [], []);
    }
}
