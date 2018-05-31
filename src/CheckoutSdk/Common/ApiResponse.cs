using System.Net;

namespace Checkout
{
    public class ApiResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public Error Error { get; set; }

        public bool HasError => Error != null;
    }

    public class ApiResponse<TResult> : ApiResponse
    {
        public TResult Result { get; set; }

        public static implicit operator TResult(ApiResponse<TResult> apiResponse)
        {
            return apiResponse.Result;
        }
    }
}