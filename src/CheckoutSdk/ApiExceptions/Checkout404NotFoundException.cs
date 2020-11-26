using System.Net;

namespace Checkout.Exceptions
{
    /// <summary>
    /// Exception thrown following a HTTP 404 (Not Found) response.
    /// </summary>
    public class Checkout404NotFoundException : CheckoutApiException
    {
        /// <summary>
        /// Creates a new <see cref="Checkout404NotFoundException"/> instance.
        /// </summary>
        /// <param name="ckoRequestId">The unique identifier of the API request.</param>
        /// <param name="ckoVersion">The version of the API gateway.</param>
        public Checkout404NotFoundException(string ckoRequestId, string ckoVersion) 
            : base(HttpStatusCode.NotFound, ckoRequestId, ckoVersion)
        {
        }
    }
}
