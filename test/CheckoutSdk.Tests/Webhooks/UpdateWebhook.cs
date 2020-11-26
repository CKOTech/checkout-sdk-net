using System.Collections.Generic;
using System.Threading.Tasks;
using Checkout.Webhooks;
using Shouldly;
using Xunit;

namespace Checkout.Tests.Webhooks
{
    public class UpdateWebhookTests : ApiTestFixture
    {
        [Fact]
        public async Task CanUpdateWebhook()
        {
            var webhook = TestHelper.CreateWebhook();

            var webhookRegistrationResponse = await Api.Webhooks.RegisterWebhook(new RegisterWebhookRequest(webhook));

            webhookRegistrationResponse.ShouldNotBeNull();

            webhookRegistrationResponse.Content.Headers.TryGetValue("authorization", out string authorization);
            webhook.Url += "/updated";
            webhook.Active = !webhookRegistrationResponse.Content.Active;
            webhook.EventTypes = new List<string>
            {
                "payment_refunded"
            };
            webhook.Headers.Add("Authorization", authorization);

            var webhookUpdateResponse = await Api.Webhooks.UpdateWebhook(webhookRegistrationResponse.Content.Id, new UpdateWebhookRequest(webhook));

            webhookUpdateResponse.ShouldNotBeNull();
            webhookUpdateResponse.Content.Id.ShouldBe(webhookRegistrationResponse.Content.Id);
            webhookUpdateResponse.Content.Url.ShouldEndWith("/updated");
            webhookUpdateResponse.Content.Active.ShouldNotBe(webhookRegistrationResponse.Content.Active);
            webhookUpdateResponse.Content.EventTypes.ShouldBe(webhook.EventTypes);

            await Api.Webhooks.RemoveWebhook(webhookUpdateResponse.Content.Id);
        }
    }
}
