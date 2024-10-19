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
                return Result<T>.Create(false, default, [], []);

            if (response.IsSuccessful)
                return Result<T>.Create(true, response.Data, [], []);
            else
                return Result<T>.Create(false, default, [response.ErrorMessage], []);

        }
    }
}
