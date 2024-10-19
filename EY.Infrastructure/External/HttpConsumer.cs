using EY.Domain.Contracts;
using EY.Domain.Models;
using EY.Domain.Models.Options;
using EY.Shared.Attributes;
using EY.Shared.Extensions;
using Polly;
using Polly.Registry;
using RestSharp;

namespace EY.Infrastructure.External
{
    [BindInterface(typeof(IHttpConsumer))]
    public class HttpConsumer : IHttpConsumer
    {
        private readonly ResiliencePipeline _pipeline;
        private readonly RestClient _restClient;
        public HttpConsumer(ResiliencePipelineProvider<string> pipelineProvider)
        {
            _pipeline = pipelineProvider.GetPipeline(RetryPolicyOptions.DEFAULT_PIPELINE);
            _restClient = new RestClient();
        }

        public async Task<Result<T>> Delete<T>(string url, List<KeyValuePair<string, string>> headers = null, CancellationToken cancellationToken = default)
        {
            var request = new RestRequest(url, Method.Delete);
            if (headers is not null)
                request.AddOrUpdateHeaders(headers);

            var response = await _pipeline.ExecuteAsync(async pipeCt => await _restClient.ExecuteAsync<T>(request, pipeCt), cancellationToken);
            var result = response.FromRestResponse();

            return result;
        }

        public async Task<Result<T>> Get<T>(string url, List<KeyValuePair<string, string>> headers = null, CancellationToken cancellationToken = default)
        {
            var request = new RestRequest(url, Method.Get);
            if (headers is not null)
                request.AddOrUpdateHeaders(headers);

            var response = await _pipeline.ExecuteAsync(async pipeCt => await _restClient.ExecuteAsync<T>(request, pipeCt), cancellationToken);
            var result = response.FromRestResponse();

            return result;
        }

        public async Task<Result<T>> Post<T>(string url, List<KeyValuePair<string, object>> param, List<KeyValuePair<string, string>> headers = null, CancellationToken cancellationToken = default)
        {
            var request = new RestRequest(url, Method.Post);
            if (headers is not null)
                request.AddOrUpdateHeaders(headers);

            if (param is not null)
            {
                var body = param.Select(p => Parameter.CreateParameter(p.Key, p.Value, ParameterType.RequestBody));
                request.AddOrUpdateParameters(body);
            }

            var response = await _pipeline.ExecuteAsync(async pipeCt => await _restClient.ExecuteAsync<T>(request, pipeCt), cancellationToken);
            var result = response.FromRestResponse();

            return result;
        }

        public async Task<Result<T>> Post<T>(string url, object param, List<KeyValuePair<string, string>> headers = null, CancellationToken cancellationToken = default)
        {
            var request = new RestRequest(url, Method.Delete);
            if (headers is not null)
                request.AddOrUpdateHeaders(headers);

            if(param is not null)
                request.AddBody(param);

            var response = await _pipeline.ExecuteAsync(async pipeCt => await _restClient.ExecuteAsync<T>(request, pipeCt), cancellationToken);
            var result = response.FromRestResponse();

            return result;
        }

        public async Task<Result<T>> Put<T>(string url, List<KeyValuePair<string, object>> param, List<KeyValuePair<string, string>> headers = null, CancellationToken cancellationToken = default)
        {
            var request = new RestRequest(url, Method.Delete);
            if (headers is not null)
                request.AddOrUpdateHeaders(headers);

            if (param is not null)
            {
                var body = param.Select(p => Parameter.CreateParameter(p.Key, p.Value, ParameterType.RequestBody));
                request.AddOrUpdateParameters(body);
            }

            var response = await _pipeline.ExecuteAsync(async pipeCt => await _restClient.ExecuteAsync<T>(request, pipeCt), cancellationToken);
            var result = response.FromRestResponse();

            return result;
        }

        public async Task<Result<T>> Put<T>(string url, object param, List<KeyValuePair<string, string>> headers = null, CancellationToken cancellationToken = default)
        {
            var request = new RestRequest(url, Method.Delete);
            if (headers is not null)
                request.AddOrUpdateHeaders(headers);

            if(param is not null)
                request.AddBody(param);

            var response = await _pipeline.ExecuteAsync(async pipeCt => await _restClient.ExecuteAsync<T>(request, pipeCt), cancellationToken);
            var result = response.FromRestResponse();

            return result;
        }
    }
}
