using System.Threading.Tasks;

namespace Checkout.Sources
{
    /// <summary>
    /// Defines the operations available on the Checkout.com Sources API.
    /// </summary>
    public interface ISourcesClient
    {
        /// <summary>
        /// Add a reusable payment source that can be used later to make one or more payments.
        /// Payment sources are linked to a specific customer and cannot be shared between customers.
        /// </summary>
        /// <param name="sourceRequest">The source details such as type and billing address.</param>
        /// <returns>A task that upon completion contains the source response.</returns>
        Task<CheckoutHttpResponseMessage<SourceResponse>> AddAPaymentSource(SourceRequest sourceRequest);
    }
}
