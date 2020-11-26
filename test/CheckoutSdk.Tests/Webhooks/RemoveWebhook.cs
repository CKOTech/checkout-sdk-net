using System.Net;
using System.Threading.Tasks;
using Checkout.Exceptions;
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

            await Api.Webhooks.RemoveWebhook(webhookRegistrationResponse.Content.Id);

            var webhookRetrievalResponse = Should.Throw<Checkout404NotFoundException>(async () => await Api.Webhooks.RetrieveWebhook(webhookRegistrationResponse.Content.Id));

            webhookRetrievalResponse.ShouldNotBeNull();
            webhookRetrievalResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }
    }
}
