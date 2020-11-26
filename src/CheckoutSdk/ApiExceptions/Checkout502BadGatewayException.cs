using System.Net;

namespace Checkout.Exceptions
{
    /// <summary>
    /// Exception thrown following a HTTP 502 (Bad Gateway) response.
    /// </summary>
    public class Checkout502BadGatewayException : CheckoutApiException
    {
        /// <summary>
        /// Creates a new <see cref="Checkout502BadGatewayException"/> instance.
        /// </summary>
        /// <param name="ckoRequestId">The unique identifier of the API request.</param>
        /// <param name="ckoVersion">The version of the API gateway.</param>
        public Checkout502BadGatewayException(string ckoRequestId, string ckoVersion)
         : base(HttpStatusCode.BadGateway, ckoRequestId, ckoVersion)
        {
        }
    }
}
