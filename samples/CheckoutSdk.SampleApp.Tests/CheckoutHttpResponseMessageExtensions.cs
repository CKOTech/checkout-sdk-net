namespace Checkout.SampleApp.Tests
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
            checkoutHttpResponseMessage.SetHeader("Cko-Request-Id", "bcfe03bd-2a8a-4340-8ce5-214761fae065");
            checkoutHttpResponseMessage.SetHeader("Cko-Version", "3.46.1");

            return checkoutHttpResponseMessage;
        }
    }
}
