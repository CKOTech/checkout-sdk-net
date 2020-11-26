using System.Net;

namespace Checkout.Exceptions
{
    /// <summary>
    /// Exception thrown following a HTTP 500 (Internal Server Error) response.
    /// </summary>
    public class Checkout500InternalServerErrorException : CheckoutApiException
    {
        /// <summary>
        /// Creates a new <see cref="Checkout500InternalServerErrorException"/> instance.
        /// </summary>
        /// <param name="ckoRequestId">The unique identifier of the API request.</param>
        /// <param name="ckoVersion">The version of the API gateway.</param>
        public Checkout500InternalServerErrorException(string ckoRequestId, string ckoVersion)
         : base(HttpStatusCode.InternalServerError, ckoRequestId, ckoVersion)
        {
        }
    }
}
