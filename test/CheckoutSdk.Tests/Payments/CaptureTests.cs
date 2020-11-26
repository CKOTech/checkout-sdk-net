using System.Collections.Generic;
using System.Threading.Tasks;
using Checkout.Payments;
using Shouldly;
using Xunit;

namespace Checkout.Tests.Payments
{
    public class CaptureTests : ApiTestFixture
    {
        [Fact]
        public async Task CanFullyCapturePayment()
        {
            var payment = await MakePaymentAsync();

            var captureRequest = new CaptureRequest
            {
                Reference = "Full Capture",
                Metadata = new Dictionary<string, object> { { nameof(CaptureTests), nameof(CanFullyCapturePayment) } }
            };

            var captureResponse = await Api.Payments.CaptureAPayment(payment.Id, captureRequest);
            captureResponse.Content.ShouldNotBeNull();
            captureResponse.Content.ActionId.ShouldNotBeNullOrEmpty();
            captureResponse.Content.Reference.ShouldBe(captureRequest.Reference);
        }

        [Fact]
        public async Task CanPartiallyCapturePayment()
        {
            var payment = await MakePaymentAsync();

            var captureRequest = new CaptureRequest
            {
                Amount = 500,
                Reference = "Partial Capture",
                Metadata = new Dictionary<string, object> { { nameof(CaptureTests), nameof(CanPartiallyCapturePayment) } }
            };

            var captureResponse = await Api.Payments.CaptureAPayment(payment.Id, captureRequest);
            captureResponse.Content.ShouldNotBeNull();
            captureResponse.Content.ActionId.ShouldNotBeNullOrEmpty();
            captureResponse.Content.Reference.ShouldBe(captureRequest.Reference);
        }

        async Task<PaymentProcessed> MakePaymentAsync()
        {
            var paymentRequest = TestHelper.CreateCardPaymentRequest(amount: 1000);
            paymentRequest.Capture = false;

            var paymentResponse = await Api.Payments.RequestAPayment(paymentRequest);
            paymentResponse.Content.Payment.CanCapture().ShouldBeTrue();

            return paymentResponse.Content.Payment;
        }
    }
}