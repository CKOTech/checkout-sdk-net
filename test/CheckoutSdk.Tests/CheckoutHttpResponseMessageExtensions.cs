namespace Checkout.Tests
{
    /// <summary>
    /// Extensions for <see cref="CheckoutHttpResponseMessage{TContent}"/>.
    /// </summary>
    public static class CheckoutHttpResponseMessageExtensions
    {
        /// <summary>
        /// Adds mocked headers to the provided <paramref="checkoutHttpResponseMessage"/>.
        /// </summary>
        /// <param name="checkoutHttpResponseMessage">The <see cref="CheckoutHttpResponseMessage{TContent}"/>.</param>
        /// <returns>The <see cref="CheckoutHttpResponseMessage{TContent}"/> with added headers for mocking.</returns>
        public static CheckoutHttpResponseMessage<TContent> MockHeaders<TContent>(this CheckoutHttpResponseMessage<TContent> checkoutHttpResponseMessage)
        {
            checkoutHttpResponseMessage.SetHeader("Cko-Request-Id", TestHelper.CkoRequestId);
            checkoutHttpResponseMessage.SetHeader("Cko-Version", TestHelper.CkoVersion);

            return checkoutHttpResponseMessage;
        }
    }
}
