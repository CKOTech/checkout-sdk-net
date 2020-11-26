using System.Net;

namespace Checkout.Exceptions
{
    /// <summary>
    /// Exception thrown following a HTTP 401 (Unauthorized) response.
    /// </summary>
    public class Checkout401UnauthorizedException : CheckoutApiException
    {
        /// <summary>
        /// Creates a new <see cref="Checkout401UnauthorizedException"/> instance.
        /// </summary>
        /// <param name="ckoRequestId">The unique identifier of the API request.</param>
        /// <param name="ckoVersion">The version of the API gateway.</param>
        public Checkout401UnauthorizedException(string ckoRequestId, string ckoVersion) 
            : base(HttpStatusCode.Unauthorized, ckoRequestId, ckoVersion)
        {
        }
    }
}
