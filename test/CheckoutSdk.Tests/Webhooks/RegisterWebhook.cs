using System.Net;
using System.Threading.Tasks;
using Checkout.Exceptions;
using Checkout.Webhooks;
using Shouldly;
using Xunit;

namespace Checkout.Tests.Webhooks
{
    public class RegisterWebhookTests : ApiTestFixture
    {
        [Fact]
        public async Task CanRegisterWebhook()
        {
            var webhook = TestHelper.CreateWebhook();

            var webhookRegistrationResponse = await Api.Webhooks.RegisterWebhook(new RegisterWebhookRequest(webhook));

            webhookRegistrationResponse.ShouldNotBeNull();
            webhookRegistrationResponse.Content.Id.ShouldStartWith("wh_");
            webhookRegistrationResponse.Content.Url.ShouldBe(webhook.Url);
            webhookRegistrationResponse.Content.EventTypes.ShouldBe(webhook.EventTypes);

            await Api.Webhooks.RemoveWebhook(webhookRegistrationResponse.Content.Id);
        }

        [Fact]
        public void GivenInvalidUrlShouldReturnValidationException()
        {
            var webhook = TestHelper.CreateWebhook();
            webhook.Url = "invalid";

            var checkoutValidationException = Should.Throw<Checkout422UnprocessableException>(async () => await Api.Webhooks.RegisterWebhook(new RegisterWebhookRequest(webhook)));

            checkoutValidationException.ShouldNotBeNull();
            checkoutValidationException.StatusCode.ShouldBe((HttpStatusCode)422);
            checkoutValidationException.Error.ErrorCodes.ShouldContain("url_invalid");
        }
    }
}
