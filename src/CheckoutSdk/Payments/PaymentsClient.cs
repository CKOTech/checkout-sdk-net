using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Checkout.Payments
{
    /// <summary>
    /// Default implementation of <see cref="IPaymentsClient"/>.
    /// </summary>
    public class PaymentsClient : IPaymentsClient
    {
        private static readonly Dictionary<HttpStatusCode, Type> PaymentResponseMappings = new Dictionary<HttpStatusCode, Type>
        {
            { HttpStatusCode.Accepted, typeof(PaymentPending)},
            { HttpStatusCode.Created, typeof(PaymentProcessed)}
        };

        private readonly IApiClient _apiClient;
        private readonly IApiCredentials _credentials;

        public PaymentsClient(IApiClient apiClient, CheckoutConfiguration configuration)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            _credentials = new SecretKeyCredentials(configuration);
        }

        public Task<PaymentResponse> RequestAsync<TRequestSource>(PaymentRequest<TRequestSource> paymentRequest, CancellationToken cancellationToken = default(CancellationToken)) 
            where TRequestSource : IRequestSource
        {
            return RequestPaymentAsync(paymentRequest, PaymentResponseMappings, cancellationToken);
        }

        public Task<GetPaymentResponse> GetAsync(string paymentId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _apiClient.GetAsync<GetPaymentResponse>(GetPaymentUrl(paymentId), _credentials, cancellationToken);
        }

        public Task<IEnumerable<PaymentAction>> GetActionsAsync(string paymentId, CancellationToken cancellationToken = default(CancellationToken))
        {
            const string path = "/actions";
            return _apiClient.GetAsync<IEnumerable<PaymentAction>>(GetPaymentUrl(paymentId) + path, _credentials, cancellationToken);
        }

        public Task<CaptureResponse> CaptureAsync(string paymentId, CaptureRequest captureRequest = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            const string path = "/captures";
            return _apiClient.PostAsync<CaptureResponse>(GetPaymentUrl(paymentId) + path, _credentials, cancellationToken, captureRequest);
        }

        public Task<RefundResponse> RefundAsync(string paymentId, RefundRequest refundRequest = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            const string path = "/refunds";
            return _apiClient.PostAsync<RefundResponse>(GetPaymentUrl(paymentId) + path, _credentials, cancellationToken, refundRequest);
        }

        public Task<VoidResponse> VoidAsync(string paymentId, VoidRequest voidRequest = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            const string path = "/voids";
            return _apiClient.PostAsync<VoidResponse>(GetPaymentUrl(paymentId) + path, _credentials, cancellationToken, voidRequest);
        }

        private async Task<PaymentResponse> RequestPaymentAsync<TRequestSource>(PaymentRequest<TRequestSource> paymentRequest, Dictionary<HttpStatusCode, Type> resultTypeMappings, CancellationToken cancellationToken) where TRequestSource : IRequestSource
        {
            const string path = "/payments";
            var apiResponse = await _apiClient.PostAsync(path, _credentials, resultTypeMappings, cancellationToken, paymentRequest);
            return apiResponse;
        }

        private static string GetPaymentUrl(string paymentId)
        {
            const string path = "payments/";
            return path + paymentId;
        }
    }
}
