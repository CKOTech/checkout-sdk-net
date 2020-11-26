using Checkout.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Checkout.Common;
using Checkout.Exceptions;

namespace Checkout
{
    /// <summary>
    /// Handles the authentication, serialization and sending of HTTP requests to Checkout APIs.
    /// </summary>
    public class ApiClient : IApiClient
    {
        private const HttpStatusCode Unprocessable = (HttpStatusCode)422;
        private const HttpStatusCode TooManyRequests = (HttpStatusCode)429;
        private static readonly ILog Logger = LogProvider.For<ApiClient>();

        private readonly CheckoutConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly ISerializer _serializer;

        /// <summary>
        /// Creates a new <see cref="ApiClient"/> instance with the provided configuration.
        /// </summary>
        /// <param name="configuration">The Checkout configuration required to configure the client.</param>
        public ApiClient(CheckoutConfiguration configuration)
            : this(configuration, new DefaultHttpClientFactory(), new JsonSerializer())
        {
        }

        /// <summary>
        /// Creates a new <see cref="ApiClient"/> instance with the provided configuration and HTTP client factory.
        /// </summary>
        /// <param name="configuration">The Checkout configuration required to configure the client.</param>
        /// <param name="httpClientFactory">A factory for creating HTTP client instances.</param>
        public ApiClient(CheckoutConfiguration configuration, IHttpClientFactory httpClientFactory)
            : this(configuration, httpClientFactory, new JsonSerializer())
        {
        }

        /// <summary>
        /// Creates a new <see cref="ApiClient"/> instance with the provided configuration and serializer.
        /// </summary>
        /// <param name="configuration">The Checkout configuration required to configure the client.</param>
        /// <param name="serializer">A serializer used to serialize and deserialize HTTP payloads.</param>

        public ApiClient(CheckoutConfiguration configuration, ISerializer serializer)
            : this(configuration, new DefaultHttpClientFactory(), serializer)
        {
        }

        /// <summary>
        /// Creates a new <see cref="ApiClient"/> instance with the provided configuration, HTTP client factory and serializer.
        /// </summary>
        /// <param name="configuration">The Checkout configuration required to configure the client.</param>
        /// <param name="httpClientFactory">A factory for creating HTTP client instances.</param>
        /// <param name="serializer">A serializer used to serialize and deserialize HTTP payloads.</param>
        public ApiClient(
            CheckoutConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            ISerializer serializer)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            if (httpClientFactory == null) throw new ArgumentNullException(nameof(httpClientFactory));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));

            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<CheckoutHttpResponseMessage<TResult>> GetAsync<TResult>(string path, IApiCredentials credentials, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
            if (credentials == null) throw new ArgumentNullException(nameof(credentials));

            using (var httpResponse = await SendRequestAsync(
                httpMethod: HttpMethod.Get,
                path: path,
                credentials: credentials,
                httpContent: null,
                cancellationToken: cancellationToken,
                idempotencyKey: null
                ))
            {
                return await httpResponse.ConvertToCheckoutHttpResponseMessage<TResult>();
            }
        }

        public async Task<CheckoutHttpResponseMessage<TResult>> PostAsync<TResult>(string path, IApiCredentials credentials, CancellationToken cancellationToken, object request = null, string idempotencyKey = null)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
            if (credentials == null) throw new ArgumentNullException(nameof(credentials));

            using (var httpResponse = await SendJsonRequestAsync(
                httpMethod: HttpMethod.Post,
                path: path,
                credentials: credentials,
                request: request,
                cancellationToken: cancellationToken,
                idempotencyKey: idempotencyKey
                ))
            {
                return await httpResponse.ConvertToCheckoutHttpResponseMessage<TResult>();
            }
        }

        public async Task<CheckoutHttpResponseMessage<TResult>> PostAsync<TResult>(string path, IApiCredentials credentials, CancellationToken cancellationToken, HttpContent httpContent = null, string idempotencyKey = null)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
            if (credentials == null) throw new ArgumentNullException(nameof(credentials));

            using (var httpResponse = await SendRequestAsync(
                httpMethod: HttpMethod.Post,
                path: path,
                credentials: credentials,
                httpContent: httpContent,
                cancellationToken: cancellationToken,
                idempotencyKey: idempotencyKey
                ))
            {
                return await httpResponse.ConvertToCheckoutHttpResponseMessage<TResult>();
            }
        }

        public async Task<CheckoutHttpResponseMessage<dynamic>> PostAsync(string path, IApiCredentials credentials, Dictionary<HttpStatusCode, Type> resultTypeMappings, CancellationToken cancellationToken, object request = null, string idempotencyKey = null)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
            if (credentials == null) throw new ArgumentNullException(nameof(credentials));
            if (resultTypeMappings == null) throw new ArgumentNullException(nameof(resultTypeMappings));

            using (var httpResponse = await SendJsonRequestAsync(
                httpMethod: HttpMethod.Post,
                path: path,
                credentials: credentials,
                request: request,
                cancellationToken: cancellationToken,
                idempotencyKey: idempotencyKey
                ))
            {
                if (!resultTypeMappings.TryGetValue(httpResponse.StatusCode, out Type resultType))
                    throw new KeyNotFoundException($"The status code {httpResponse.StatusCode} is not mapped to a result type");
                return await httpResponse.ConvertToChekoutHttpResponseMessage(resultType);
            }
        }

        public async Task<CheckoutHttpResponseMessage<TResult>> PutAsync<TResult>(string path, IApiCredentials credentials, CancellationToken cancellationToken, object request = null)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
            if (credentials == null) throw new ArgumentNullException(nameof(credentials));

            using (var httpResponse = await SendJsonRequestAsync(
                httpMethod: HttpMethod.Put,
                path: path,
                credentials: credentials,
                request: request,
                cancellationToken: cancellationToken,
                idempotencyKey: null
                ))
            {
                return await httpResponse.ConvertToCheckoutHttpResponseMessage<TResult>();
            }
        }

        public async Task<CheckoutHttpResponseMessage<TResult>> PatchAsync<TResult>(string path, IApiCredentials credentials, CancellationToken cancellationToken, object request = null)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
            if (credentials == null) throw new ArgumentNullException(nameof(credentials));

            using (var httpResponse = await SendJsonRequestAsync(
                httpMethod: new HttpMethod("PATCH"),
                path: path,
                credentials: credentials,
                request: request,
                cancellationToken: cancellationToken,
                idempotencyKey: null
                ))
            {
                return await httpResponse.ConvertToCheckoutHttpResponseMessage<TResult>();
            }
        }

        public async Task<CheckoutHttpResponseMessage<TResult>> DeleteAsync<TResult>(string path, IApiCredentials credentials, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
            if (credentials == null) throw new ArgumentNullException(nameof(credentials));

            using (var httpResponse = await SendRequestAsync(
                httpMethod: HttpMethod.Delete,
                path: path,
                credentials: credentials,
                httpContent: null,
                cancellationToken: cancellationToken,
                idempotencyKey: null
                ))
            {
                return await httpResponse.ConvertToCheckoutHttpResponseMessage<TResult>();
            }
        }

        private async Task<HttpResponseMessage> SendRequestAsync(HttpMethod httpMethod, string path, IApiCredentials credentials, HttpContent httpContent, CancellationToken cancellationToken, string idempotencyKey)
        {
            const string product = "checkout-sdk-net";
            var productVersion = "undefined" ?? ReflectionUtils.GetAssemblyVersion<ApiClient>();

            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));

            var httpRequest = new HttpRequestMessage(httpMethod, GetRequestUri(path))
            {
                Content = httpContent
            };

            httpRequest.Headers.UserAgent.ParseAdd($"{product}/{productVersion}");
            if(!string.IsNullOrWhiteSpace(idempotencyKey)) httpRequest.Headers.Add("Cko-Idempotency-Key", idempotencyKey);

            await credentials.AuthorizeAsync(httpRequest);

            Logger.Info("{HttpMethod} {Uri}", httpMethod, httpRequest.RequestUri.AbsoluteUri);

            var httpResponse = await _httpClient.SendAsync(httpRequest, cancellationToken);
            await ValidateResponseAsync(httpResponse);

            return httpResponse;
        }

        private Task<HttpResponseMessage> SendJsonRequestAsync(HttpMethod httpMethod, string path, IApiCredentials credentials, object request, CancellationToken cancellationToken, string idempotencyKey)
        {
            HttpContent httpContent = null;
            if (request != null)
            {
                httpContent = new StringContent(_serializer.Serialize(request), Encoding.UTF8, "application/json");
            }
            return SendRequestAsync(httpMethod, path, credentials, httpContent, cancellationToken, idempotencyKey);
        }

        private async Task ValidateResponseAsync(HttpResponseMessage httpResponse)
        {
            if (!httpResponse.Headers.TryGetValues("Cko-Request-Id", out var ckoRequestIdHeader)) throw new KeyNotFoundException("Key \"Cko-Request-Id\" was not present in the HTTP response header.");
            if (!httpResponse.Headers.TryGetValues("Cko-Version", out var ckoVersionHeader)) throw new KeyNotFoundException("Key \"Cko-Version\" was not present in the HTTP response header.");

            if (!httpResponse.IsSuccessStatusCode)
            {
                var ckoRequestId = ckoRequestIdHeader?.FirstOrDefault();
                var ckoVersion = ckoVersionHeader?.FirstOrDefault();

                try
                {
                    switch (httpResponse.StatusCode)
                    {
                        case HttpStatusCode.BadRequest:
                            throw new Checkout400BadRequestException(ckoRequestId, ckoVersion);
                        case HttpStatusCode.Unauthorized:
                            throw new Checkout401UnauthorizedException(ckoRequestId, ckoVersion);
                        case HttpStatusCode.Forbidden:
                            throw new Checkout403ForbiddenException(ckoRequestId, ckoVersion);
                        case HttpStatusCode.NotFound:
                            throw new Checkout404NotFoundException(ckoRequestId, ckoVersion);
                        case HttpStatusCode.Conflict:
                            throw new Checkout409ConflictException(ckoRequestId, ckoVersion);
                        case Unprocessable:
                            throw new Checkout422UnprocessableException(ckoRequestId, ckoVersion, await httpResponse.Content.DeserializeToCheckoutContent<ErrorResponse>());
                        case TooManyRequests:
                            throw new Checkout429TooManyRequestsException(ckoRequestId, ckoVersion, await httpResponse.Content.DeserializeToCheckoutContent<ErrorResponse>());
                        case HttpStatusCode.InternalServerError:
                            throw new Checkout500InternalServerErrorException(ckoRequestId, ckoVersion);
                        case HttpStatusCode.NotImplemented:
                            throw new Checkout501NotImplementedException(ckoRequestId, ckoVersion);
                        case HttpStatusCode.BadGateway:
                            throw new Checkout502BadGatewayException(ckoRequestId, ckoVersion);
                        default:
                            throw new NotImplementedException($"The HTTP status code {httpResponse.StatusCode} is not defined by the Checkout.com API specification.");
                    }
                }
                catch (Exception exception)
                {
                    Logger.DebugException(exception.Message, exception);
                    throw exception;
                }
            }
        }

        private Uri GetRequestUri(string path)
        {
            var baseUri = new Uri(_configuration.Uri);
            Uri.TryCreate(baseUri, path, out var uri);

            return uri;
        }
    }
}
