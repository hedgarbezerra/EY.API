using EY.Domain.Contracts;
using EY.Domain.Models;
using EY.Domain.Models.Options;
using EY.Shared.Attributes;
using EY.Shared.Extensions;
using Polly;
using Polly.Registry;
using RestSharp;

namespace EY.Infrastructure.External;

[BindInterface(typeof(IHttpConsumer))]
public class HttpConsumer : IHttpConsumer
{
    private readonly ResiliencePipeline _pipeline;
    private readonly IRestClient _restClient;

    public HttpConsumer(ResiliencePipelineProvider<string> pipelineProvider, IRestClient restClient)
    {
        _pipeline = pipelineProvider.GetPipeline(RetryPolicyOptions.DEFAULT_PIPELINE);
        _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
    }

    public async Task<Result<T>> Delete<T>(string url, List<KeyValuePair<string, string>> headers = null,
        CancellationToken cancellationToken = default)
    {
        var request = CreateRequest(url, Method.Delete, headers);
        return await ExecuteRequestAsync<T>(request, cancellationToken);
    }

    public async Task<Result<T>> Get<T>(string url, List<KeyValuePair<string, string>> headers = null,
        CancellationToken cancellationToken = default)
    {
        var request = CreateRequest(url, Method.Get, headers);
        return await ExecuteRequestAsync<T>(request, cancellationToken);
    }

    public async Task<Result<T>> Post<T>(string url, List<KeyValuePair<string, object>> param,
        List<KeyValuePair<string, string>> headers = null, CancellationToken cancellationToken = default)
    {
        var request = CreateRequest(url, Method.Post, headers, param);
        return await ExecuteRequestAsync<T>(request, cancellationToken);
    }

    public async Task<Result<T>> Post<T>(string url, object param, List<KeyValuePair<string, string>> headers = null,
        CancellationToken cancellationToken = default)
    {
        var request = CreateRequest(url, Method.Post, headers, param);
        return await ExecuteRequestAsync<T>(request, cancellationToken);
    }

    public async Task<Result<T>> Put<T>(string url, List<KeyValuePair<string, object>> param,
        List<KeyValuePair<string, string>> headers = null, CancellationToken cancellationToken = default)
    {
        var request = CreateRequest(url, Method.Put, headers, param);
        return await ExecuteRequestAsync<T>(request, cancellationToken);
    }

    public async Task<Result<T>> Put<T>(string url, object param, List<KeyValuePair<string, string>> headers = null,
        CancellationToken cancellationToken = default)
    {
        var request = CreateRequest(url, Method.Put, headers, param);
        return await ExecuteRequestAsync<T>(request, cancellationToken);
    }

    private RestRequest CreateRequest(string url, Method method, List<KeyValuePair<string, string>> headers = null,
        object body = null)
    {
        var request = new RestRequest(url, method);
        if (headers != null)
            foreach (var header in headers)
                request.AddHeader(header.Key, header.Value);

        if (body != null) request.AddJsonBody(body);

        return request;
    }

    private async Task<Result<T>> ExecuteRequestAsync<T>(RestRequest request, CancellationToken cancellationToken)
    {
        var response = await _pipeline.ExecuteAsync(async pipeCt => await _restClient.ExecuteAsync<T>(request, pipeCt),
            cancellationToken);
        return response.FromRestResponse();
    }
}