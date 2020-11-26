using System.Net;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Checkout.Tokens
{
    /// <summary>
    /// Defines the operations available on the Checkout.com Tokenization API.
    /// </summary>
    public interface ITokensClient
    {
        /// <summary>
        /// Exchange card details for a reference token that can be later used to request a card payment.
        /// </summary>
        /// <param name="cardTokenRequest">The card details.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the underlying HTTP request.</param>
        /// <returns>A task that upon completion contains the card token response</returns>
        Task<CheckoutHttpResponseMessage<CardTokenResponse>> RequestAToken(CardTokenRequest cardTokenRequest, CancellationToken cancellationToken = default(CancellationToken));
        
        /// <summary>
        /// Exchange a digital wallet payment token for a reference token that can be later used to request a card payment.
        /// </summary>
        /// <param name="walletTokenRequest">The wallet token.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the underlying HTTP request.</param>
        /// <returns>A task that upon completion contains the wallet token response</returns>
        Task<CheckoutHttpResponseMessage<TokenResponse>> RequestAToken(WalletTokenRequest walletTokenRequest, CancellationToken cancellationToken = default(CancellationToken));
    }
}