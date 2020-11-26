using System.Threading.Tasks;
using Shouldly;
using Xunit;
using Moq;
using Checkout.Events;
using System.Threading;
using System.Net.Http;
using System.Net;
using Checkout.Exceptions;

namespace Checkout.Tests.Events
{
    public class RetryAllWebhooksTests : ApiTestFixture
    {
        private readonly Mock<IEventsClient> _eventsClient;

        public RetryAllWebhooksTests()
        {
            var canRetryAllWebhooksResponse = new CheckoutHttpResponseMessage<object>(HttpStatusCode.Accepted).MockHeaders();

            _eventsClient = new Mock<IEventsClient>();
            _eventsClient.Setup(eventsClient => eventsClient.RetryAllWebhooks("evt_4ddvw5cfb4xurn3mfedxhdtvqa", default(CancellationToken))).ReturnsAsync(() => canRetryAllWebhooksResponse);
            _eventsClient.Setup(eventsClient => eventsClient.RetryAllWebhooks(It.IsNotIn(new string[] { "evt_4ddvw5cfb4xurn3mfedxhdtvqa" }), default(CancellationToken))).ThrowsAsync(new Checkout404NotFoundException(TestHelper.CkoRequestId, TestHelper.CkoVersion));
        }

        [Fact]
        public async Task CanRetryAllWebhooks()
        {
            var retryAllWebhooksResponse = await _eventsClient.Object.RetryAllWebhooks("evt_4ddvw5cfb4xurn3mfedxhdtvqa");

            retryAllWebhooksResponse.ShouldNotBeNull();
            retryAllWebhooksResponse.StatusCode.ShouldBe(HttpStatusCode.Accepted);
            retryAllWebhooksResponse.Headers.TryGetValues("Cko-Request-Id", out var ckoRequestId).ShouldBeTrue();
            retryAllWebhooksResponse.Headers.TryGetValues("Cko-Version", out var ckoVersion).ShouldBeTrue();
        }

        [Fact]
        public async Task Returns404ifEventDoesNotExist()
        {
           await Assert.ThrowsAsync<Checkout404NotFoundException>(async () => await _eventsClient.Object.RetryAllWebhooks("evt_doesNotExist"));
        }
    }
}
