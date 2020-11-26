using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace Checkout.Tests.Webhooks
{
    public class RetrieveWebhooksTests : ApiTestFixture
    {
        [Fact]
        public async Task CanRetrieveWebhooks()
        {
            var webhooksRetrievalResponse = await Api.Webhooks.RetrieveWebhooks();

            if(webhooksRetrievalResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                webhooksRetrievalResponse.Content.ShouldNotBeNull();
                webhooksRetrievalResponse.Content.Count.ShouldBeGreaterThan(0);
            }
            else
            {
                webhooksRetrievalResponse.StatusCode.ShouldBe(System.Net.HttpStatusCode.NoContent);
                webhooksRetrievalResponse.Content.ShouldBeNull();
            }
        }
    }
}
