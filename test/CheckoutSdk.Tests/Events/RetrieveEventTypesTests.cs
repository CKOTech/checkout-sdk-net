using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Checkout.Events;
using Moq;
using Shouldly;
using Xunit;

namespace Checkout.Tests.Events
{
    public class RetrieveEventTypesTests : ApiTestFixture
    {
        public RetrieveEventTypesTests()
        {
            // 200 Notification retrieved successfully
            var eventTypesResponse = new AvailableEventTypesResponse()
            {
                new EventTypesResponse()
                {
                    Version = "1.0",
                    EventTypes = new List<string>()
                    {
                        "card.updated",
                        "charge.captured",
                        "charge.captured.deferred",
                        "charge.captured.failed",
                        "charge.chargeback",
                        "charge.failed",
                        "charge.pending",
                        "charge.refunded",
                        "charge.refunded.failed",
                        "charge.retrieval",
                        "charge.succeeded",
                        "charge.voided",
                        "charge.voided.failed",
                        "invoice.cancelled"
                    }
                },
                new EventTypesResponse()
                {
                    Version = "2.0",
                    EventTypes = new List<string>()
                    {
                        "card_verification_declined",
                        "card_verified",
                        "dispute_canceled",
                        "dispute_evidence_required",
                        "dispute_expired",
                        "dispute_lost",
                        "dispute_resolved",
                        "dispute_won",
                        "payment_approved",
                        "payment_canceled",
                        "payment_capture_declined",
                        "payment_capture_pending",
                        "payment_captured",
                        "payment_chargeback",
                        "payment_declined",
                        "payment_expired",
                        "payment_paid",
                        "payment_pending",
                        "payment_refund_declined",
                        "payment_refund_pending",
                        "payment_refunded",
                        "payment_retrieval",
                        "payment_void_declined",
                        "payment_voided",
                        "source_updated"
                    }
                }
            };
            var retrieveEventTypesResponse_200 = new CheckoutHttpResponseMessage<AvailableEventTypesResponse>(HttpStatusCode.OK, eventTypesResponse).MockHeaders();
            ApiMock.Setup(eventsClient => eventsClient.Events.RetrieveEventTypes(default(CancellationToken))).ReturnsAsync(() => retrieveEventTypesResponse_200);

            // TODO: 401 Unauthorized
        }

        // 200 Notification retrieved successfully
        [Fact]
        public async Task CanRetrieveEventTypes()
        {
            var eventTypesRetrievalResponse = await ApiMock.Object.Events.RetrieveEventTypes();

            eventTypesRetrievalResponse.Content.ShouldNotBeNull();
            eventTypesRetrievalResponse.Content.ShouldBeOfType<AvailableEventTypesResponse>();
            eventTypesRetrievalResponse.Content.Count.ShouldBeGreaterThan(0);
        }

        // TODO: 401 Unauthorized
    }
}
