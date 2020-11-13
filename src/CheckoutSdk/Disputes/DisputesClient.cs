using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Checkout.Disputes
{
    /// <summary>
    /// Default implementation of <see cref="IDisputesClient"/>.
    /// </summary>
    public class DisputesClient : IDisputesClient
    {
        private readonly IApiClient _apiClient;
        private readonly IApiCredentials _credentials;
        private const string path = "disputes";

        public DisputesClient(IApiClient apiClient, CheckoutConfiguration configuration)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            _credentials = new SecretKeyCredentials(configuration);
        }

        public Task<(HttpStatusCode StatusCode, HttpResponseHeaders Headers, GetDisputesResponse Content)> GetDisputes(GetDisputesRequest getDisputesRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (getDisputesRequest == null) throw new ArgumentNullException(nameof(getDisputesRequest));
            var pathWithQuery = getDisputesRequest.PathWithQuery(path);
            
            return _apiClient.GetAsync<GetDisputesResponse>(pathWithQuery, _credentials, cancellationToken);
        }

        public Task<(HttpStatusCode StatusCode, HttpResponseHeaders Headers, Dispute Content)> GetDisputeDetails(string disputeId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _apiClient.GetAsync<Dispute>($"{path}/{disputeId}", _credentials, cancellationToken);
        }

        public Task<(HttpStatusCode StatusCode, HttpResponseHeaders Headers, dynamic Content)> AcceptDispute(string disputeId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _apiClient.PostAsync<dynamic>($"{path}/{disputeId}/accept", _credentials, cancellationToken, null);
        }

        public Task<(HttpStatusCode StatusCode, HttpResponseHeaders Headers, dynamic Content)> ProvideDisputeEvidence(string disputeId, DisputeEvidence disputeEvidence, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _apiClient.PutAsync<dynamic>($"{path}/{disputeId}/evidence", _credentials, cancellationToken, disputeEvidence);
        }

        public Task<(HttpStatusCode StatusCode, HttpResponseHeaders Headers, dynamic Content)> SubmitDisputeEvidence(string disputeId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _apiClient.PostAsync<dynamic>($"{path}/{disputeId}/evidence", _credentials, cancellationToken, null);
        }

        public Task<(HttpStatusCode StatusCode, HttpResponseHeaders Headers, DisputeEvidenceResponse Content)> GetDisputeEvidence(string disputeId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _apiClient.GetAsync<DisputeEvidenceResponse>($"{path}/{disputeId}/evidence", _credentials, cancellationToken);
        }
    }
}
