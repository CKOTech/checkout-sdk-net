using System.Net;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Checkout.Instruments
{
    /// <summary>
    /// Defines the operations available on the Checkout.com Instruments API.
    /// </summary>
    public interface IInstrumentsClient
    {
        /// <summary>
        /// Create a payment instrument that you can later use as the source or destination for one or more payments.
        /// </summary>
        /// <param name="instrumentRequest">The payment instrument details such as type and token.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the underlying HTTP request.</param>
        /// <returns>A task that upon completion contains the <see cref="CreateInstrumentResponse"/>.</returns>
        Task<CheckoutHttpResponseMessage<CreateInstrumentResponse>> CreateAnInstrument(InstrumentRequest instrumentRequest, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Returns details of a payment instrument.
        /// </summary>
        /// <param name="instrumentId">The unique identifier of the payment instrument.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the underlying HTTP request.</param>
        /// <returns>A task that upon completion contains the <see cref="GetInstrumentResponse"/>.</returns>
        Task<CheckoutHttpResponseMessage<GetInstrumentResponse>> GetInstrumentDetails(string instrumentId, CancellationToken cancellationToken = default(CancellationToken));


        /// <summary>
        /// Update details of a payment instrument.
        /// </summary>
        /// <param name="instrumentId">>The unique identifier of the payment instrument.</param>
        /// <param name="updateInstrumentRequest">The payment instrument details such as type and token.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the underlying HTTP request.</param>
        /// <returns>A task that upon completion contains the <see cref="UpdateInstrumentResponse"/>.</returns>
        Task<CheckoutHttpResponseMessage<UpdateInstrumentResponse>> UpdateInstrumentDetails(string instrumentId, UpdateInstrumentRequest updateIstrumentRequest, CancellationToken cancellationToken = default(CancellationToken));

    }
}
