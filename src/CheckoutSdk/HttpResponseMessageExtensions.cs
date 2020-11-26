using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Checkout
{
    /// <summary>
    /// Extensions for <see cref="HttpResponseMessage"/>.
    /// </summary>
    public static class HttpResponseMessageExtensions
    {
        static private ISerializer _serializer = new JsonSerializer();
        static private HttpStatusCode _statusCode;
        static private HttpResponseHeaders _headers;
        static private HttpContent _contentInput;
        static private dynamic _contentOutput = null;
        static private string _json;

        /// <summary>
        /// Converts a provided <paramref name="httpResponseMessage"/> into a <see cref="CheckoutHttpResponseMessage{dynamic}"/>.
        /// </summary>
        /// <param name="httpResponseMessage">The <see cref="HttpResponseMessage"/> to be transformed.</param>
        /// <returns>The <see cref="CheckoutHttpResponseMessage{TContent}"/> representation of the <see cref="HttpResponseMessage"/>.</returns>
        public static async Task<CheckoutHttpResponseMessage<TContent>> ConvertToCheckoutHttpResponseMessage<TContent>(this HttpResponseMessage httpResponseMessage)
        {
            return await ComposeCheckoutHttpResponseMessage<TContent>(httpResponseMessage, typeof(TContent));
        }

        /// <summary>
        /// Converts a provided <paramref name="httpResponseMessage"/> into a <see cref="CheckoutHttpResponseMessage{dynamic}"/>.
        /// </summary>
        /// <param name="httpResponseMessage">The <see cref="HttpResponseMessage"/> to be transformed.</param>
        /// <param name="responseType">The type of the response.</param>
        /// <returns>The <see cref="CheckoutHttpResponseMessage{dynamic}"/> representation of the <see cref="HttpResponseMessage"/>.</returns>
        public static async Task<CheckoutHttpResponseMessage<dynamic>> ConvertToChekoutHttpResponseMessage(this HttpResponseMessage httpResponseMessage, Type responseType)
        {
            return await ComposeCheckoutHttpResponseMessage<dynamic>(httpResponseMessage, responseType);
        }

        /// <summary>
        /// Composes a <see cref="CheckoutHttpResponseMessage{T}"/> from a given <paramref name="httpResponseMessage"/> and <paramref name="type"/>.
        /// </summary>
        /// <param name="httpResponseMessage">The <see cref="HttpResponseMessage"/> to be transformed.</param>
        /// <param name="type">The <see cref="Type"/> for the deserialization of the content.</param>
        /// <returns>The <see cref="CheckoutHttpResponseMessage{T}"/> representation of the <see cref="HttpResponseMessage"/>.</returns>
        private async static Task<CheckoutHttpResponseMessage<T>> ComposeCheckoutHttpResponseMessage<T>(HttpResponseMessage httpResponseMessage, Type type)
        {
            _statusCode = httpResponseMessage.StatusCode;
            _headers = httpResponseMessage.Headers;
            _contentInput = httpResponseMessage.Content;

            if (_contentInput != null) _contentOutput = await _contentInput.DeserializeToCheckoutContent(type);

            return new CheckoutHttpResponseMessage<T>(_statusCode, _headers, _contentOutput);
        }
    }
}
