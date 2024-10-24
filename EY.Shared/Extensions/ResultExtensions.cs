using EY.Domain.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EY.Shared.Extensions
{
    public static class ResultExtensions
    {
        public static Result<T> FromRestResponse<T>(this RestResponse<T> response)
        {
            if (response is null)
                return Result<T>.Failure(["No data available."]);

            if (response.IsSuccessful)
            {
                if (typeof(T) == typeof(string))
                    return Result<T>.Success((T)(object)response.Content, []);
                else
                    return Result<T>.Success(response.Data, []);
            }
            else
                return Result<T>.Failure([response.ErrorMessage]);

        }
    }
}
