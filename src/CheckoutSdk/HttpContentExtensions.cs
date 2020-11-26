using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Checkout
{
    /// <summary>
    /// Extensions for <see cref="HttpContent"/>.
    /// </summary>
    public static class HttpContentExtensions
    {
        static private ISerializer _serializer = new JsonSerializer();
        static private HttpContent _contentInput;
        static private dynamic _contentOutput = null;
        static private string _json;

        /// <summary>
        /// Converts a provided <paramref name="httpResponseMessage"/> into Checkout HTTP response content.
        /// </summary>
        /// <param name="httpContent">The <see cref="HttpContent"/> to be deserialized.</param>
        /// <returns>The Checkout HTTP response content.</returns>
        public static async Task<TContent> DeserializeToCheckoutContent<TContent>(this HttpContent httpContent)
        {
            return await Deserialize<TContent>(httpContent, typeof(TContent));
        }

        /// <summary>
        /// Converts a provided <paramref name="httpResponseMessage"/> Checkout HTTP response content.
        /// </summary>
        /// <param name="httpContent">The <see cref="HttpContent"/> to be deserialized.</param>
        /// <param name="responseType">The type of the response.</param>
        /// <returns>The Checkout HTTP response content.</returns>
        public static async Task<dynamic> DeserializeToCheckoutContent(this HttpContent httpContent, Type responseType)
        {
            return await Deserialize<dynamic>(httpContent, responseType);
        }

        /// <summary>
        /// Composes a Checkout HTTP response content from a given <paramref name="httpResponseMessage"/> and <paramref name="type"/>.
        /// </summary>
        /// <param name="httpContent">The <see cref="HttpContent"/> to be deserialized.</param>
        /// <param name="type">The <see cref="Type"/> for the deserialization of the content.</param>
        /// <returns>The Checkout HTTP response content.</returns>
        private async static Task<T> Deserialize<T>(HttpContent httpContent, Type type)
        {
            _contentInput = httpContent;

            if (_contentInput != null)
            {
                _json = await _contentInput.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(_json))
                    _contentOutput = (dynamic)_serializer.Deserialize(_json, type);
            }
            return _contentOutput;
        }
    }
}
