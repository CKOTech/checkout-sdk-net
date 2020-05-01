using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Checkout.Sources
{
    /// <summary>
    /// Default implementation of <see cref="ISourcesClient"/>.
    /// </summary>
    public class SourcesClient : ISourcesClient
    {
        private static readonly Dictionary<HttpStatusCode, Type> SourceResponseMappings = new Dictionary<HttpStatusCode, Type>
        {
            { HttpStatusCode.Created, typeof(SourceProcessed)}
        };

        private readonly IApiClient _apiClient;
        private readonly IApiCredentials _credentials;

        public SourcesClient(IApiClient apiClient, CheckoutConfiguration configuration)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            _credentials = new SecretKeyCredentials(configuration);
        }

        public Task<SourceResponse> RequestAsync(SourceRequest sourceRequest)
        {
            return RequestSourceAsync(sourceRequest, SourceResponseMappings);
        }

        private async Task<SourceResponse> RequestSourceAsync(SourceRequest sourceRequest, Dictionary<HttpStatusCode, Type> resultTypeMappings, CancellationToken cancellationToken = default(CancellationToken))
        {
            const string path = "sources";
            var apiResponse = await _apiClient.PostAsync(path, _credentials, resultTypeMappings, cancellationToken, sourceRequest).NotOnCapturedContext();
            return apiResponse;
        }
    }
}
