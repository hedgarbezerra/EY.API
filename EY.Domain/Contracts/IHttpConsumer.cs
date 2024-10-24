using EY.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EY.Domain.Contracts
{
    /// <summary>
    /// Interface representing an HTTP consumer responsible for making HTTP requests and retrieving the responses.
    /// </summary>
    public interface IHttpConsumer
    {
        /// <summary>
        /// Sends a HTTP DELETE request to the specified URL with optional headers.
        /// </summary>
        /// <typeparam name="T">The type to which the response will be deserialized.</typeparam>
        /// <param name="url">The URL of the request.</param>
        /// <param name="headers">Optional headers to include in the request.</param>
        /// <param name="cancellationToken">Optional cancellation token to cancel the request.</param>
        /// <returns>The task result contains a Result object, which includes the response deserialized into the specified type.</returns>
        Task<Result<T>> Delete<T>(string url, List<KeyValuePair<string, string>> headers = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a HTTP GET request to the specified URL with optional headers.
        /// </summary>
        /// <typeparam name="T">The type to which the response will be deserialized.</typeparam>
        /// <param name="url">The URL of the request.</param>
        /// <param name="headers">Optional headers to include in the request.</param>
        /// <param name="cancellationToken">Optional cancellation token to cancel the request.</param>
        /// <returns>The task result contains a Result object, which includes the response deserialized into the specified type.</returns>
        Task<Result<T>> Get<T>(string url, List<KeyValuePair<string, string>> headers = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a HTTP POST request to the specified URL with a list of parameters and optional headers.
        /// </summary>
        /// <typeparam name="T">The type to which the response will be deserialized.</typeparam>
        /// <param name="url">The URL of the request.</param>
        /// <param name="param">The list of parameters to include in the request body.</param>
        /// <param name="headers">Optional headers to include in the request.</param>
        /// <param name="cancellationToken">Optional cancellation token to cancel the request.</param>
        /// <returns>The task result contains a Result object, which includes the response deserialized into the specified type.</returns>
        Task<Result<T>> Post<T>(string url, List<KeyValuePair<string, object>> param, List<KeyValuePair<string, string>> headers = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends an HTTP POST request to the specified URL with an object as the request body and optional headers.
        /// </summary>
        /// <typeparam name="T">The type to which the response will be deserialized.</typeparam>
        /// <param name="url">The URL of the request.</param>
        /// <param name="param">The object to include in the request body.</param>
        /// <param name="headers">Optional headers to include in the request.</param>
        /// <param name="cancellationToken">Optional cancellation token to cancel the request.</param>
        /// <returns>The task result contains a Result object, which includes the response deserialized into the specified type.</returns>
        Task<Result<T>> Post<T>(string url, object param, List<KeyValuePair<string, string>> headers = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a HTTP PUT request to the specified URL with a list of parameters and optional headers.
        /// </summary>
        /// <typeparam name="T">The type to which the response will be deserialized.</typeparam>
        /// <param name="url">The URL of the request.</param>
        /// <param name="param">The list of parameters to include in the request body.</param>
        /// <param name="headers">Optional headers to include in the request.</param>
        /// <param name="cancellationToken">Optional cancellation token to cancel the request.</param>
        /// <returns>The task result contains a Result object, which includes the response deserialized into the specified type.</returns>
        Task<Result<T>> Put<T>(string url, List<KeyValuePair<string, object>> param, List<KeyValuePair<string, string>> headers = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a HTTP PUT request to the specified URL with an object as the request body and optional headers.
        /// </summary>
        /// <typeparam name="T">The type to which the response will be deserialized.</typeparam>
        /// <param name="url">The URL of the request.</param>
        /// <param name="param">The object to include in the request body.</param>
        /// <param name="headers">Optional headers to include in the request.</param>
        /// <param name="cancellationToken">Optional cancellation token to cancel the request.</param>
        /// <returns>The task result contains a Result object, which includes the response deserialized into the specified type.</returns>
        Task<Result<T>> Put<T>(string url, object param, List<KeyValuePair<string, string>> headers = null, CancellationToken cancellationToken = default);
    }

}
