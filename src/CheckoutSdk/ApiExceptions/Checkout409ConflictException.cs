using System.Net;

namespace Checkout.Exceptions
{
    /// <summary>
    /// Exception thrown following a HTTP 409 (Conflict) response.
    /// </summary>
    public class Checkout409ConflictException : CheckoutApiException
    {
        /// <summary>
        /// Creates a new <see cref="Checkout409ConflictException"/> instance.
        /// </summary>
        /// <param name="ckoRequestId">The unique identifier of the API request.</param>
        /// <param name="ckoVersion">The version of the API gateway.</param>
        public Checkout409ConflictException(string ckoRequestId, string ckoVersion) 
            : base(HttpStatusCode.Conflict, ckoRequestId, ckoVersion)
        {
        }
    }
}
