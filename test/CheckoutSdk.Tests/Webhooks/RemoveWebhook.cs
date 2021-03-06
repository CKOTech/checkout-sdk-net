using System.Net;
using System.Threading.Tasks;
using Checkout.Webhooks;
using Shouldly;
using Xunit;

namespace Checkout.Tests.Webhooks
{
    public class RemoveWebhookTests : ApiTestFixture
    {
        [Fact]
        public async Task CanRemoveWebhook()
        {
            var webhook = TestHelper.CreateWebhook();

            var webhookRegistrationResponse = await Api.Webhooks.RegisterWebhook(new RegisterWebhookRequest(webhook));

            webhookRegistrationResponse.ShouldNotBeNull();

            await Api.Webhooks.RemoveWebhook(webhookRegistrationResponse.Id);

            var webhookRetrievalResponse = Should.Throw<CheckoutResourceNotFoundException>(async () => await Api.Webhooks.RetrieveWebhook(webhookRegistrationResponse.Id));

            webhookRetrievalResponse.ShouldNotBeNull();
            webhookRetrievalResponse.HttpStatusCode.ShouldBe(HttpStatusCode.NotFound);
        }
    }
}
