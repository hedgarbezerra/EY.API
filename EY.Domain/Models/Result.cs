using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EY.Domain.Models
{
    public class Result<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public List<string> Successes { get; set; } = new List<string>();

        private Result(bool success, T? data, List<string> errors, List<string> successes)
        {
            Success = success;
            Data = data;
            Errors = errors;
            Successes = successes;
        }

        public static Result<T> Create(bool success, T? data, List<string> errors, List<string> successes) =>
            new Result<T>(success, data, errors, successes);
    }

    public class Result
    {

        public bool Success { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public List<string> Successes { get; set; } = new List<string>();

        private Result(bool success, List<string> errors, List<string> successes)
        {
            Success = success;
            Errors = errors;
            Successes = successes;
        }

        public static Result Create(bool success, List<string> errors, List<string> successes) =>
            new Result(success, errors, successes);
    }
}
