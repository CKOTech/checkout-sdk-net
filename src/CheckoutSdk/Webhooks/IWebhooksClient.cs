using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Checkout.Webhooks
{
    /// <summary>
    /// Defines the operations available on the Checkout.com Webhooks API.
    /// </summary>
    public interface IWebhooksClient
    {
        /// <summary>
        /// Retrieves the webhooks configured for the channel identified by your API key
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the underlying HTTP request.</param>
        /// <returns>A task that upon completion contains the configured webhooks.</returns>
        Task<WebhooksResponse> RetrieveWebhooks(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Register a new webhook endpoint that Checkout.com will post all or selected events to
        /// </summary>
        /// <param name="webhookRequest">The webhook configuration details such as url and event types.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the underlying HTTP request.</param>
        /// <returns>A task that upon completion contains the registered webhook.</returns>
        Task<WebhookResponse> RegisterWebhook(RegisterWebhookRequest webhookRequest, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Retrieves the webhook with the specified identifier string
        /// </summary>
        /// <param name="webhookId">The unique webhook identifier.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the underlying HTTP request.</param>
        /// <returns>A task that upon completion contains the webhook with the specified identifier string.</returns>
        Task<WebhookResponse> RetrieveWebhook(string webhookId, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Updates an existing webhook
        /// </summary>
        /// <param name="webhookId">The unique webhook identifier.</param>
        /// <param name="webhookRequest">The webhook configuration details such as url and event types.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the underlying HTTP request.</param>
        /// <returns>A task that upon completion contains the updated webhook with the specified identifier string.</returns>
        Task<WebhookResponse> UpdateWebhook(string webhookId, UpdateWebhookRequest webhookRequest, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Updates all or some of the registered webhook details
        /// </summary>
        /// <param name="webhookId">The unique webhook identifier.</param>
        /// <param name="webhookRequest">The webhook configuration details such as url and event types.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the underlying HTTP request.</param>
        /// <returns>A task that upon completion contains the (partially) updated webhook with the specified identifier string.</returns>
        Task<WebhookResponse> PartiallyUpdateWebhook(string webhookId, PartialUpdateWebhookRequest webhookRequest, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Removes an existing webhook
        /// </summary>
        /// <param name="webhookId">The unique webhook identifier.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the underlying HTTP request.</param>
        Task<HttpResponseMessage> RemoveWebhook(string webhookId, CancellationToken cancellationToken = default(CancellationToken));
    }
}
