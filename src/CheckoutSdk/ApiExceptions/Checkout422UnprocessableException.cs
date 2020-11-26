using Checkout.Common;
using System.Net;

namespace Checkout.Exceptions
{
    /// <summary>
    /// Exception thrown following a HTTP 422 (Unprocessable) response.
    /// </summary>
    public class Checkout422UnprocessableException : CheckoutApiException
    {
        /// <summary>
        /// Creates a new <see cref="Checkout422UnprocessableException"/> instance.
        /// </summary>
        /// <param name="ckoRequestId">The unique identifier of the API request.</param>
        /// <param name="ckoVersion">The version of the API gateway.</param>
        /// <param name="error">The validation error details.</param>
        public Checkout422UnprocessableException(string ckoRequestId, string ckoVersion, ErrorResponse error = null)
         : base((HttpStatusCode)422, ckoRequestId, ckoVersion, GenerateDetailsMessage(error))
        {
            Error = error;
        }

        /// <summary>
        /// Gets the error response.
        /// </summary>
        public ErrorResponse Error { get; }

        private static string GenerateDetailsMessage(ErrorResponse error)
        {
            if(error != null)
                return $"A validation error of type {error.ErrorType} occurred with error codes [{string.Join(",", error.ErrorCodes)}].";
            return null;
        }
    }
}
