using System.Net;

namespace Checkout.Exceptions
{
    /// <summary>
    /// Exception thrown following a HTTP 403 (Forbidden) response.
    /// </summary>
    public class Checkout403ForbiddenException : CheckoutApiException
    {
        /// <summary>
        /// Creates a new <see cref="Checkout403ForbiddenException"/> instance.
        /// </summary>
        /// <param name="ckoRequestId">The unique identifier of the API request.</param>
        /// <param name="ckoVersion">The version of the API gateway.</param>
        public Checkout403ForbiddenException(string ckoRequestId, string ckoVersion) 
            : base(HttpStatusCode.Forbidden, ckoRequestId, ckoVersion)
        {
        }
    }
}
