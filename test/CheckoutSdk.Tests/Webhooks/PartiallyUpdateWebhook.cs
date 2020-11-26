using System.Threading.Tasks;
using Checkout.Webhooks;
using Shouldly;
using Xunit;

namespace Checkout.Tests.Webhooks
{
    public class PartiallyUpdateWebhookTests : ApiTestFixture
    {
        [Fact]
        public async Task CanPartiallyUpdateWebhook()
        {
            var webhook = TestHelper.CreateWebhook();

            var webhookRegistrationResponse = await Api.Webhooks.RegisterWebhook(new RegisterWebhookRequest(webhook));

            webhookRegistrationResponse.ShouldNotBeNull();

            webhook.Url += "/partially/updated";
            webhook.Headers = null;
            webhook.EventTypes = null;

            var webhookPartialUpdateResponse = await Api.Webhooks.PartiallyUpdateWebhook(webhookRegistrationResponse.Content.Id, new PartialUpdateWebhookRequest(webhook));

            webhookPartialUpdateResponse.ShouldNotBeNull();
            webhookPartialUpdateResponse.Content.Id.ShouldBe(webhookRegistrationResponse.Content.Id);
            webhookPartialUpdateResponse.Content.Url.ShouldEndWith("/partially/updated");
            webhookPartialUpdateResponse.Content.Active.ShouldBe(webhookRegistrationResponse.Content.Active);
            webhookPartialUpdateResponse.Content.Headers.ShouldBe(webhookRegistrationResponse.Content.Headers);
            webhookPartialUpdateResponse.Content.EventTypes.ShouldBe(webhookRegistrationResponse.Content.EventTypes);

            await Api.Webhooks.RemoveWebhook(webhookPartialUpdateResponse.Content.Id);
        }
    }
}
