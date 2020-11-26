using System.Threading.Tasks;
using Checkout.Disputes;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace Checkout.Tests.Disputes
{
    public class GetDisputeTests : IClassFixture<ApiTestFixture>
    {
        private readonly ICheckoutApi _api;

        public GetDisputeTests(ApiTestFixture fixture, ITestOutputHelper outputHelper)
        {
            fixture.CaptureLogsInTestOutput(outputHelper);
            _api = fixture.Api;
        }

        [Fact]
        public async Task CanGetDispute()
        {
            // simulate a chargeback payment
            var chargebackPaymentRequest = TestHelper.CreateChargebackCardPaymentRequest();
            var paymentResponse = await _api.Payments.RequestAPayment(chargebackPaymentRequest);

            paymentResponse.Content.ShouldNotBeNull();
            paymentResponse.Content.IsPending.ShouldBeFalse();

            // call disputes endpoint every 10s to wait for payment to transition to disputed state
            var getDisputesRequest = new GetDisputesRequest(paymentId: paymentResponse.Content.Payment.Id);
            DisputeSummary dispute = null;
            while (dispute == null)
            {
                await Task.Delay(TestHelper.PaymentDisputedVerificationInterval());
                var getDisputesResponse = await _api.Disputes.GetDisputes(getDisputesRequest: getDisputesRequest);
                var disputes = getDisputesResponse.Content.Data;
                if (disputes.Count == 1) dispute = disputes[0];
            }

            dispute.PaymentId.ShouldBe(paymentResponse.Content.Payment.Id);
            dispute.Status.ShouldBe("evidence_required");

            // get test dispute details
            var getDisputeResponse = await _api.Disputes.GetDisputeDetails(disputeId: dispute.Id);

            getDisputeResponse.Content.ShouldNotBeNull();
            getDisputeResponse.Content.Id.ShouldBe(dispute.Id);
        }
    }
}
