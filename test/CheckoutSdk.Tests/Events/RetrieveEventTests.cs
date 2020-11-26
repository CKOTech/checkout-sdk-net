using System.Threading.Tasks;
using Shouldly;
using Xunit;
using Moq;
using Checkout.Events;
using System;
using System.Collections.Generic;
using Checkout.Payments;
using Checkout.Common;
using System.Threading;
using System.Net;
using Checkout.Exceptions;

namespace Checkout.Tests.Events
{
    public class RetrieveEventTests : ApiTestFixture
    {
        public RetrieveEventTests()
        {
            // 200 Event retrieved successfully
            var eventResponse = new EventResponse()
            {
                Id = "evt_c2qelfixai2u3es3ksovngkx3e",
                Type = "payment_captured",
                Version = "2.0",
                CreatedOn = DateTime.Parse("2020-04-24T11:50:11Z"),
                Data = new Dictionary<string, object>()
                {
                    {"action_id", "act_puqzoc4sna7uvhjledmp6vjjby" },
                    {"response_code", "10000" },
                    {"response_summary", "Approved" },
                    {"amount", 1000 },
                    {"metadata", new Dictionary<string, string>() },
                    {"processing", new ProcessingResponse()
                        {
                            AcquirerTransactionId = "1159094323",
                            RetrievalReferenceNumber = "874479685178"
                        }
                    },
                    {"id", "pay_xskg4u47uabenhpxfvpmqduume" },
                    {"currency", "USD" },
                    {"processed_on", DateTime.Parse("2020-04-24T11:50:11Z") }
                },
                Notifications = new List<EventNotificationSummary>(),
                Links = new Dictionary<string, Link>()
                    {
                        { "self", new Link(){ Href = "https://api.sandbox.checkout.com/events/evt_c2qelfixai2u3es3ksovngkx3e" } },
                        { "webhooks-retry", new Link(){ Href = "https://api.sandbox.checkout.com/events/evt_c2qelfixai2u3es3ksovngkx3e/webhooks/retry" } }
                    }
            };
            var retrieveEventResponse_200 = new CheckoutHttpResponseMessage<EventResponse>(HttpStatusCode.OK, eventResponse).MockHeaders();
            ApiMock.Setup(eventsClient => eventsClient.Events.RetrieveEvent("evt_c2qelfixai2u3es3ksovngkx3e", default(CancellationToken))).ReturnsAsync(() => retrieveEventResponse_200);
            
            // TODO: 401 Unaithorized

            // 404 Event not found
            ApiMock.Setup(eventsClient => eventsClient.Events.RetrieveEvent(It.IsNotIn(new string[] { "evt_c2qelfixai2u3es3ksovngkx3e" }), default(CancellationToken))).ThrowsAsync(new Checkout404NotFoundException(TestHelper.CkoRequestId, TestHelper.CkoVersion));
        }

        // 200 Event retrieved successfully
        [Fact]
        public async Task CanRetrieveEvent()
        {
            var eventRetrievalResponse = await ApiMock.Object.Events.RetrieveEvent("evt_c2qelfixai2u3es3ksovngkx3e");

            eventRetrievalResponse.Content.ShouldNotBeNull();
            eventRetrievalResponse.Content.ShouldBeOfType<EventResponse>();
        }

        // TODO: 401 Unauthorized

        // 404 Event not found
        [Fact]
        public async Task Returns404ifEventDoesNotExist()
        {
            await Assert.ThrowsAsync<Checkout404NotFoundException>(async () => await ApiMock.Object.Events.RetrieveEvent("evt_doesNotExist"));
        }
    }
}
