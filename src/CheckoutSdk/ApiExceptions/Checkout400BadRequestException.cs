using System.Net;

namespace Checkout.Exceptions
{
    /// <summary>
    /// Exception thrown following a HTTP 400 (Bad Request) response.
    /// </summary>
    public class Checkout400BadRequestException : CheckoutApiException
    {
        /// <summary>
        /// Creates a new <see cref="Checkout400BadRequestException"/> instance.
        /// </summary>
        /// <param name="ckoRequestId">The unique identifier of the API request.</param>
        /// <param name="ckoVersion">The version of the API gateway.</param>
        public Checkout400BadRequestException(string ckoRequestId, string ckoVersion) 
            : base(HttpStatusCode.BadRequest, ckoRequestId, ckoVersion)
        {
        }
    }
}
