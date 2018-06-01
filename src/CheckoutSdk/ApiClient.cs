using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Checkout.Logging;

namespace Checkout
{
    public class ApiClient : IApiClient
    {
        private const HttpStatusCode Unprocessable = (HttpStatusCode)422;
        private static readonly ILog Logger = LogProvider.For<ApiClient>();

        private readonly CheckoutConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly ISerializer _serializer;

        public ApiClient(CheckoutConfiguration configuration)
            : this(configuration, new DefaultHttpClientFactory(), new JsonSerializer())
        {
        }

        public ApiClient(CheckoutConfiguration configuration, IHttpClientFactory httpClientFactory, ISerializer serializer)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            if (httpClientFactory == null) throw new ArgumentNullException(nameof(httpClientFactory));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));

            _httpClient = httpClientFactory.Create();
        }


        public async Task<ApiResponse<TResult>> PostAsync<TResult>(string path, object request = null)
        {
            var httpResponse = await SendRequestAsync(HttpMethod.Post, path, request);

            var apiResponse = new ApiResponse<TResult>
            {
                StatusCode = httpResponse.StatusCode // Pass in ctor
            };

            if (httpResponse.StatusCode == Unprocessable)
                apiResponse.Error = await DeserializeJsonAsync<Error>(httpResponse);

            if (httpResponse.IsSuccessStatusCode)
                apiResponse.Result = await DeserializeJsonAsync<TResult>(httpResponse);

            return apiResponse;
        }

        public async Task<ApiResponse<dynamic>> PostAsync(string path, object request, Dictionary<HttpStatusCode, Type> resultTypeMappings)
        {
            var httpResponse = await SendRequestAsync(HttpMethod.Post, path, request);

            var apiResponse = new ApiResponse<dynamic>
            {
                StatusCode = httpResponse.StatusCode // Pass in ctor
            };

            if (httpResponse.StatusCode == Unprocessable)
                apiResponse.Error = await DeserializeJsonAsync<Error>(httpResponse);

            if (resultTypeMappings.TryGetValue(httpResponse.StatusCode, out Type resultType))
                apiResponse.Result = await DeserializeJsonAsync(httpResponse, resultType);

            return apiResponse;
        }

        private async Task<TResult> DeserializeJsonAsync<TResult>(HttpResponseMessage httpResponse)
        {
            var result = await DeserializeJsonAsync(httpResponse, typeof(TResult));
            return (TResult)result;
        }

        private async Task<dynamic> DeserializeJsonAsync(HttpResponseMessage httpResponse, Type resultType)
        {
            if (httpResponse.Content == null)
                return null;

            var json = await httpResponse.Content.ReadAsStringAsync();
            return _serializer.Deserialize(json, resultType);
        }

        private Task<HttpResponseMessage> SendRequestAsync(HttpMethod httpMethod, string path, object request = null)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));
            
            var httpRequest = new HttpRequestMessage(httpMethod, GetRequestUri(path));
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue(_configuration.SecretKey);
            httpRequest.Headers.UserAgent.ParseAdd("checkout-sdk-net/1.0.0");

            if (request != null)
            {
                httpRequest.Content = new StringContent(_serializer.Serialize(request), Encoding.UTF8, "application/json");
            }

            Logger.Info("{HttpMethod} {Uri}", HttpMethod.Post, httpRequest.RequestUri.AbsoluteUri);
            
            return _httpClient.SendAsync(httpRequest);
        }

        private Uri GetRequestUri(string path)
        {
            var baseUri = new Uri(_configuration.Uri);
            Uri.TryCreate(baseUri, path, out var uri);

            return uri;
        }
    }
}