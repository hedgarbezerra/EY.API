using EY.Domain.Models;
using RestSharp;

namespace EY.Shared.Extensions;

public static class ResultExtensions
{
    public static Result<T> FromRestResponse<T>(this RestResponse<T> response)
    {
        if (response is null)
            return Result<T>.Failure("No data available.");

        if (response.IsSuccessful)
        {
            if (typeof(T) == typeof(string))
                return Result<T>.Success((T)(object)response.Content);
            return Result<T>.Success(response.Data);
        }

        return Result<T>.Failure(response.ErrorMessage);
    }
}