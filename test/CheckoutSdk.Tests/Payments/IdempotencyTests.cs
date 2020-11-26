using System.Collections.Generic;
using System.Threading.Tasks;
using Checkout.Payments;
using Shouldly;
using Xunit;

namespace Checkout.Tests.Payments
{
    public class IdempotencyTests : ApiTestFixture
    {
        const string idempotencyKey = "1234";

        [Fact]
        public async Task CanIdempotentlyRequestPayment()
        {
            var paymentRequest = TestHelper.CreateCardPaymentRequest(amount: 1000);
            paymentRequest.Reference = "Idempotent Payment Request";
            paymentRequest.Metadata = new Dictionary<string, object> { { nameof(IdempotencyTests), nameof(CanIdempotentlyRequestPayment) } };

            var paymentResponse1 = await Api.Payments.RequestAPayment(paymentRequest, idempotencyKey: idempotencyKey);
            paymentResponse1.ShouldNotBeNull();

            var paymentResponse2 = await Api.Payments.RequestAPayment(paymentRequest, idempotencyKey: idempotencyKey);
            paymentResponse2.ShouldNotBeNull();

            paymentResponse2.Content.Payment.Id.ShouldBe(paymentResponse1.Content.Payment.Id);
        }

        [Fact]
        public async Task CanIdempotentlyCapturePayment()
        {
            var payment = await MakePaymentAsync(capture: false);

            var captureRequest = new CaptureRequest
            {
                Reference = "Idempotent Capture",
                Metadata = new Dictionary<string, object> { { nameof(IdempotencyTests), nameof(CanIdempotentlyCapturePayment) } }
            };

            var captureResponse1 = await Api.Payments.CaptureAPayment(payment.Id, captureRequest, idempotencyKey: idempotencyKey);
            captureResponse1.ShouldNotBeNull();

            var captureResponse2 = await Api.Payments.CaptureAPayment(payment.Id, captureRequest, idempotencyKey: idempotencyKey);
            captureResponse2.ShouldNotBeNull();

            captureResponse2.Content.ActionId.ShouldBe(captureResponse1.Content.ActionId);
        }

        [Fact]
        public async Task CanIdempotentlyVoidPayment()
        {
            var payment = await MakePaymentAsync(capture: false);

            var voidRequest = new VoidRequest
            {
                Reference = "Idempotent Void",
                Metadata = new Dictionary<string, object> { { nameof(IdempotencyTests), nameof(CanIdempotentlyVoidPayment) } }
            };

            var voidResponse1 = await Api.Payments.VoidAPayment(payment.Id, voidRequest, idempotencyKey: idempotencyKey);
            voidResponse1.ShouldNotBeNull();

            var voidResponse2 = await Api.Payments.VoidAPayment(payment.Id, voidRequest, idempotencyKey: idempotencyKey);
            voidResponse2.ShouldNotBeNull();

            voidResponse2.Content.ActionId.ShouldBe(voidResponse1.Content.ActionId);
        }

        [Fact]
        public async Task CanIdempotentlyRefundPayment()
        {
            var payment = await MakePaymentAsync(capture: false);

            await Api.Payments.CaptureAPayment(payment.Id);

            var paymentResponse = await Api.Payments.GetPaymentDetails(payment.Id);
            paymentResponse.Content.Status.ShouldBe("Captured");

            var refundRequest = new RefundRequest
            {
                Reference = "Idempotent Refund",
                Metadata = new Dictionary<string, object> { { nameof(IdempotencyTests), nameof(CanIdempotentlyRefundPayment) } }
            };

            var refundResponse1 = await Api.Payments.RefundAPayment(payment.Id, refundRequest, idempotencyKey: idempotencyKey);
            refundResponse1.ShouldNotBeNull();

            var refundResponse2 = await Api.Payments.RefundAPayment(payment.Id, refundRequest, idempotencyKey: idempotencyKey);
            refundResponse2.ShouldNotBeNull();

            refundResponse2.Content.ActionId.ShouldBe(refundResponse1.Content.ActionId);
        }

        async Task<PaymentProcessed> MakePaymentAsync(bool capture = true)
        {
            var paymentRequest = TestHelper.CreateCardPaymentRequest(amount: 1000);
            paymentRequest.Capture = capture;

            var paymentResponse = await Api.Payments.RequestAPayment(paymentRequest);
            paymentResponse.Content.Payment.CanCapture().ShouldBeTrue();

            return paymentResponse.Content.Payment;
        }
    }
}