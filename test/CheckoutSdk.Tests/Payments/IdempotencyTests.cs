using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Checkout.Payments;
using Shouldly;
using Xunit;

namespace Checkout.Tests.Payments
{
    public class IdempotencyTests : ApiTestFixture
    {
        [Fact]
        public async Task CanIdempotentlyAutoCapturePayment()
        {
            var idempotencyKey = Guid.NewGuid().ToString();
            var payment1 = await RequestPaymentAsync(idempotencyKey: idempotencyKey);
            var payment2 = await RequestPaymentAsync(idempotencyKey: idempotencyKey);

            payment1.Id.ShouldBe(payment2.Id);
        }

        [Fact]
        public async Task CanIdempotentlyAuthorizeThenCapturePayment()
        {
            var idempotencyKey = Guid.NewGuid().ToString();
            var authorizedPayment = await RequestPaymentAsync(capture: false, idempotencyKey: idempotencyKey);

            var capture1 = await Api.Payments.CaptureAsync(authorizedPayment.Id, idempotencyKey: idempotencyKey);
            var capture2 = await Api.Payments.CaptureAsync(authorizedPayment.Id, idempotencyKey: idempotencyKey);

            capture1.ActionId.ShouldBe(capture2.ActionId);
        }

        [Fact]
        public async Task CanIdempotentlyVoidPayment()
        {
            var idempotencyKey = Guid.NewGuid().ToString();
            var payment = await RequestPaymentAsync(capture: false, idempotencyKey: idempotencyKey);

            var void1 = await Api.Payments.VoidAsync(payment.Id, idempotencyKey: idempotencyKey);
            var void2 = await Api.Payments.VoidAsync(payment.Id, idempotencyKey: idempotencyKey);

            void1.ActionId.ShouldBe(void2.ActionId);
        }

        [Fact]
        public async Task CanIdempotentlyRefundPayment()
        {
            var idempotencyKey = Guid.NewGuid().ToString();
            var payment = await RequestPaymentAsync(idempotencyKey: idempotencyKey);

            await Task.Delay(3000); //payment must be allowed to transition from authorized to captured before refunding
            var refund1 = await Api.Payments.RefundAsync(payment.Id, idempotencyKey: idempotencyKey);
            var refund2 = await Api.Payments.RefundAsync(payment.Id, idempotencyKey: idempotencyKey);

            refund1.ActionId.ShouldBe(refund2.ActionId);
        }

        async Task<PaymentProcessed> RequestPaymentAsync(bool capture = true,  string idempotencyKey = null)
        {
            var paymentRequest = TestHelper.CreateCardPaymentRequest(amount: 1000);
            paymentRequest.Capture = capture;

            var paymentResponse = await Api.Payments.RequestAsync(paymentRequest, idempotencyKey: idempotencyKey);
            if(!capture) paymentResponse.Payment.CanVoid().ShouldBeTrue();

            return paymentResponse.Payment;
        }
    }
}