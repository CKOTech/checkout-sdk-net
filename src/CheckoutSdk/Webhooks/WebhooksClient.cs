using System;
using System.Threading;
using System.Threading.Tasks;

namespace Checkout.Webhooks
{
    /// <summary>
    /// Default implementation of <see cref="IWebhooksClient"/>.
    /// </summary>
    public class WebhooksClient : IWebhooksClient
    {
        private readonly IApiClient _apiClient;
        private readonly IApiCredentials _credentials;
        private const string path = "webhooks";

        public WebhooksClient(IApiClient apiClient, CheckoutConfiguration configuration)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            _credentials = new SecretKeyCredentials(configuration);
        }

        public Task<CheckoutHttpResponseMessage<WebhooksResponse>> RetrieveWebhooks(CancellationToken cancellationToken = default(CancellationToken))
        {            
            return _apiClient.GetAsync<WebhooksResponse>(path, _credentials, cancellationToken);
        }

        public Task<CheckoutHttpResponseMessage<WebhookResponse>> RegisterWebhook(RegisterWebhookRequest webhookRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _apiClient.PostAsync<WebhookResponse>(path, _credentials, cancellationToken, webhookRequest);
        }

        public Task<CheckoutHttpResponseMessage<WebhookResponse>> RetrieveWebhook(string webhookId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _apiClient.GetAsync<WebhookResponse>($"{path}/{webhookId}", _credentials, cancellationToken);
        }

        public Task<CheckoutHttpResponseMessage<WebhookResponse>> UpdateWebhook (string webhookId, UpdateWebhookRequest webhookRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _apiClient.PutAsync<WebhookResponse>($"{path}/{webhookId}", _credentials, cancellationToken, webhookRequest);
        }

        public Task<CheckoutHttpResponseMessage<WebhookResponse>> PartiallyUpdateWebhook(string webhookId, PartialUpdateWebhookRequest webhookRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _apiClient.PatchAsync<WebhookResponse>($"{path}/{webhookId}", _credentials, cancellationToken, webhookRequest);
        }

        public Task<CheckoutHttpResponseMessage<dynamic>> RemoveWebhook(string webhookId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _apiClient.DeleteAsync<dynamic>($"{path}/{webhookId}", _credentials, cancellationToken);
        }
    }
}
