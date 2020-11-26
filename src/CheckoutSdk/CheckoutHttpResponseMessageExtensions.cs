namespace Checkout
{
    /// <summary>
    /// Extensions for <see cref="CheckoutHttpResponseMessage{dynamic}"/>.
    /// </summary>
    public static class CheckoutHttpResponseMessageExtensions
    {
        /// <summary>
        /// Casts a provided dynamically typed <paramref name="checkoutHttpResponseMessage"/> into a typed <see cref="CheckoutHttpResponseMessage{TContent}"/>.
        /// </summary>
        /// <param name="checkoutHttpResponseMessage">The <see cref="CheckoutHttpResponseMessage{dynamic}"/>.</param>
        /// <returns>The <see cref="CheckoutHttpResponseMessage{TContent}"/> cast.</returns>
        public static CheckoutHttpResponseMessage<TContent> CastToType<TContent>(this CheckoutHttpResponseMessage<dynamic> checkoutHttpResponseMessage)
        {
            return new CheckoutHttpResponseMessage<TContent>(checkoutHttpResponseMessage.StatusCode, checkoutHttpResponseMessage.Headers, checkoutHttpResponseMessage.Content); ;
        }
    }
}
