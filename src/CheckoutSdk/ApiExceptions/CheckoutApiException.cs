using System.Net;

namespace Checkout.Exceptions
{
    /// <summary>
    /// Exception type for errors resulting from API operations.
    /// </summary>
    public class CheckoutApiException : CheckoutException
    {
        /// <summary>
        /// Creates a new <see cref="CheckoutApiException"/> instance.
        /// </summary>
        /// <param name="statusCode">The HTTP status code of the API response.</param>
        /// <param name="ckoRequestId">The unique identifier of the API request.</param>
        /// <param name="ckoVersion">The version of the API gateway.</param>
        /// <param name="additionalInformation">Additional details about the error.</param>
        public CheckoutApiException(HttpStatusCode statusCode, string ckoRequestId, string ckoVersion, string additionalInformation = null) 
            : base(GenerateMessage(statusCode, additionalInformation))
        {
            StatusCode = statusCode;
            CkoRequestId = ckoRequestId;
            CkoVersion = ckoVersion;
        }

        /// <summary>
        /// Gets the HTTP status code of the API response.
        /// </summary>
        /// <value></value>
        public HttpStatusCode StatusCode { get; }
        
        /// <summary>
        /// Gets the unique identifier of the API request.
        /// </summary>
        public string CkoRequestId { get; }

        /// <summary>
        /// Gets the version of the API gateway.
        /// </summary>
        public string CkoVersion { get; }

        private static string GenerateMessage(HttpStatusCode httpStatusCode, string additionalInformation = null)
        {
            var message = $"The API response status code ({httpStatusCode}) does not indicate success.";
            
            if (!string.IsNullOrWhiteSpace(additionalInformation))
                return message + " " + additionalInformation;
            
            return message;
        }
    }
}