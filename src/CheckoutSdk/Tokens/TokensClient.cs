using System;
using System.Net;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Checkout.Tokens
{
    /// <summary>
    /// Default implementation of <see cref="ITokensClient"/>.
    /// </summary>
    public class TokensClient : ITokensClient
    {
        private readonly IApiClient _apiClient;
        private readonly IApiCredentials _credentials;

        public TokensClient(IApiClient apiClient, CheckoutConfiguration configuration)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            _credentials = new PublicKeyCredentials(configuration);
        }

        public Task<CheckoutHttpResponseMessage<CardTokenResponse>> RequestAToken(CardTokenRequest cardTokenRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (cardTokenRequest == null)
                throw new ArgumentNullException(nameof(cardTokenRequest));

            return _apiClient.PostAsync<CardTokenResponse>("tokens", _credentials, cancellationToken, cardTokenRequest);
        }

        public Task<CheckoutHttpResponseMessage<TokenResponse>> RequestAToken(WalletTokenRequest walletTokenRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (walletTokenRequest == null)
                throw new ArgumentNullException(nameof(walletTokenRequest));

            return _apiClient.PostAsync<TokenResponse>("tokens", _credentials, cancellationToken, walletTokenRequest);
        }
    }
}