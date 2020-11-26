using System.Net;

namespace Checkout.Exceptions
{
    /// <summary>
    /// Exception thrown following a HTTP 501 (Not Implemented) response.
    /// </summary>
    public class Checkout501NotImplementedException : CheckoutApiException
    {
        /// <summary>
        /// Creates a new <see cref="Checkout501NotImplementedException"/> instance.
        /// </summary>
        /// <param name="ckoRequestId">The unique identifier of the API request.</param>
        /// <param name="ckoVersion">The version of the API gateway.</param>
        public Checkout501NotImplementedException(string ckoRequestId, string ckoVersion)
         : base(HttpStatusCode.NotImplemented, ckoRequestId, ckoVersion)
        {
        }
    }
}
