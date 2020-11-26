using System.Threading.Tasks;
using Checkout.Webhooks;
using Shouldly;
using Xunit;

namespace Checkout.Tests.Webhooks
{
    public class RetrieveWebhookTests : ApiTestFixture
    {
        [Fact]
        public async Task CanRetrieveWebhook()
        {
            var webhook = TestHelper.CreateWebhook();

            var webhookRegistrationResponse = await Api.Webhooks.RegisterWebhook(new RegisterWebhookRequest(webhook));

            webhookRegistrationResponse.ShouldNotBeNull();

            var webhookRetrievalResponse = await Api.Webhooks.RetrieveWebhook(webhookRegistrationResponse.Content.Id);

            webhookRetrievalResponse.ShouldNotBeNull();
            webhookRetrievalResponse.Content.Id.ShouldBe(webhookRegistrationResponse.Content.Id);

            await Api.Webhooks.RemoveWebhook(webhookRegistrationResponse.Content.Id);
        }
    }
}
