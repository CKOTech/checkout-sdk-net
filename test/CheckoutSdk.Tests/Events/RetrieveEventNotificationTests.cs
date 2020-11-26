using System.Threading.Tasks;
using Shouldly;
using Xunit;
using Moq;
using Checkout.Events;
using System;
using System.Collections.Generic;
using Checkout.Common;
using System.Threading;
using System.Net;
using Checkout.Exceptions;

namespace Checkout.Tests.Events
{
    public class RetrieveEventNotificationTests : ApiTestFixture
    {
        private readonly ICheckoutApi _api;

        public RetrieveEventNotificationTests()
        {
            _api = true ? ApiMock.Object : Api;

            // 200 Notification retrieved successfully
            var eventNotificationResponse = new EventNotificationResponse()
            {
                Id = "ntf_cuzguxmkcncu5g2tmzghsnbkm4",
                Url = "https://www.example.com/webhooks/incoming/checkout",
                Success = true,
                ContentType = "json",
                Attempts = new List<EventNotificationAttempt>()
                {
                    new EventNotificationAttempt()
                    {
                        StatusCode = 200,
                        SendMode = "automatic",
                        Timestamp = DateTime.Parse("2020-09-15T08:00:29Z")
                    }
                },
                Links = new Dictionary<string, Link>()
                    {
                        { "self", new Link(){ Href = "https://api.sandbox.checkout.com/events/evt_g7danjfojdse5hclmq2pk6csve/notifications/ntf_cuzguxmkcncu5g2tmzghsnbkm4" } },
                        { "webhook-retry", new Link(){ Href = "https://api.sandbox.checkout.com/events/evt_g7danjfojdse5hclmq2pk6csve/webhooks/wh_xttkznlbnf4ehezf4exbszlql4/retry" } }
                    }
            }; ;
            var retrieveEventNotificationResponse_200 = new CheckoutHttpResponseMessage<EventNotificationResponse>(HttpStatusCode.OK, eventNotificationResponse).MockHeaders();
            ApiMock.Setup(eventsClient => eventsClient.Events.RetrieveEventNotification("evt_g7danjfojdse5hclmq2pk6csve", "ntf_cuzguxmkcncu5g2tmzghsnbkm4", default(CancellationToken))).ReturnsAsync(() => retrieveEventNotificationResponse_200);

            // TODO: 401 Unauthorized

            // 404 Event or notification not found
            ApiMock.Setup(eventsClient => eventsClient.Events.RetrieveEventNotification(It.IsNotIn(new string[] { "evt_g7danjfojdse5hclmq2pk6csve" }), It.IsAny<string>(), default(CancellationToken))).ThrowsAsync(new Checkout404NotFoundException(TestHelper.CkoRequestId, TestHelper.CkoVersion));

            // 500 Event or notification badly formatted
            ApiMock.Setup(eventsClient => eventsClient.Events.RetrieveEventNotification("evt_invalidFormat", It.IsAny<string>(), default(CancellationToken))).ThrowsAsync(new Checkout500InternalServerErrorException(TestHelper.CkoRequestId, TestHelper.CkoVersion));
        }

        // 200 Notification retrieved successfully
        [Fact]
        public async Task CanRetrieveEventNotification()
        {
            var eventNotificationRetrievalResponse = await _api.Events.RetrieveEventNotification(eventId: "evt_g7danjfojdse5hclmq2pk6csve", notificationId: "ntf_cuzguxmkcncu5g2tmzghsnbkm4");

            TestHelper.DueDiligenceTests(eventNotificationRetrievalResponse);
            eventNotificationRetrievalResponse.Content.ShouldNotBeNull();
            eventNotificationRetrievalResponse.Content.ShouldBeOfType<EventNotificationResponse>();
            eventNotificationRetrievalResponse.Content.Attempts.Count.ShouldBeGreaterThan(0);
        }

        // TODO: 401 Unauthorized

        // 404 Event or notification not found
        [Fact]
        public async Task Returns404GivenInexistingEventOrNotificationId()
        {
            var eventNotificationRetrievalResponse = await Assert.ThrowsAsync<Checkout404NotFoundException>(async () => await _api.Events.RetrieveEventNotification("evt_g7danjfojdse5hclmq2pk6csvc", "ntf_cuzguxmkcncu5g2tmzghsnbkm4"));

            eventNotificationRetrievalResponse.CkoRequestId.ShouldNotBeNull();
        }

        // 500 Event or notification badly formatted
        [Fact]
        public async Task Returns500GivenMalformattedEventOrNotificationId()
        {
            var eventNotificationRetrievalResponse = await Assert.ThrowsAsync<Checkout500InternalServerErrorException>(async () => await _api.Events.RetrieveEventNotification("evt_invalidFormat", "ntf_cuzguxmkcncu5g2tmzghsnbkm4"));

            eventNotificationRetrievalResponse.CkoRequestId.ShouldNotBeNull();
        }
    }
}
