using Shouldly;
using System;
using System.Threading.Tasks;
using Checkout.Payments;
using Xunit;
using Xunit.Abstractions;

namespace Checkout.Tests.Payments
{
    public class CardSourcePaymentsTests : IClassFixture<ApiTestFixture>
    {
        private readonly ICheckoutApi _api;

        public CardSourcePaymentsTests(ApiTestFixture fixture, ITestOutputHelper outputHelper)
        {
            fixture.CaptureLogsInTestOutput(outputHelper);
            _api = fixture.Api;
        }

        [Fact]
        public async Task CanRequestNonThreeDsCardPayment()
        {
            PaymentRequest<CardSource> paymentRequest = TestHelper.CreateCardPaymentRequest();
            paymentRequest.ThreeDS = false;

            var paymentResponse = await _api.Payments.RequestAPayment(paymentRequest);
            
            paymentResponse.Content.Payment.ShouldNotBeNull();
            paymentResponse.Content.Payment.Approved.ShouldBeTrue();
            paymentResponse.Content.Payment.Id.ShouldNotBeNullOrEmpty();
            paymentResponse.Content.Payment.ActionId.ShouldNotBeNullOrEmpty();
            paymentResponse.Content.Payment.Amount.ShouldBe(paymentRequest.Amount.Value);
            paymentResponse.Content.Payment.Currency.ShouldBe(paymentRequest.Currency);
            paymentResponse.Content.Payment.Reference.ShouldBe(paymentRequest.Reference);
            paymentResponse.Content.Payment.Customer.ShouldNotBeNull();
            paymentResponse.Content.Payment.Customer.Id.ShouldNotBeNullOrEmpty();
            paymentResponse.Content.Payment.Customer.Email.ShouldNotBeNullOrEmpty();
            paymentResponse.Content.Payment.CanCapture().ShouldBeTrue();
            paymentResponse.Content.Payment.CanVoid().ShouldBeTrue();
            paymentResponse.Content.Payment.Source.AsCard().ShouldNotBeNull();
            paymentResponse.Content.Payment.Processing.ShouldNotBeNull();
            paymentResponse.Content.Payment.Processing.AcquirerTransactionId.ShouldNotBeNullOrWhiteSpace();
            paymentResponse.Content.Payment.Processing.RetrievalReferenceNumber.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task CanRequestThreeDsCardPayment()
        {
            PaymentRequest<CardSource> paymentRequest = TestHelper.CreateCardPaymentRequest();
            paymentRequest.ThreeDS = true;

            var paymentResponse = await _api.Payments.RequestAPayment(paymentRequest);

            paymentResponse.Content.IsPending.ShouldBe(true);
            var pending = paymentResponse.Content.Pending;

            pending.ShouldNotBeNull();

            pending.Id.ShouldNotBeNullOrEmpty();
            pending.Reference.ShouldBe(paymentRequest.Reference);
            pending.Customer.ShouldNotBeNull();
            pending.Customer.Id.ShouldNotBeNullOrEmpty();
            pending.Customer.Email.ShouldBe(paymentRequest.Customer.Email);
            pending.ThreeDS.ShouldNotBeNull();
            pending.ThreeDS.Downgraded.ShouldBe(false);
            pending.ThreeDS.Enrolled.ShouldNotBeNullOrEmpty();
            pending.RequiresRedirect().ShouldBe(true);
            pending.GetRedirectLink().ShouldNotBeNull();
        }

        [Fact]
        public async Task CanVoidPayment()
        {
            // Auth
            var paymentRequest = TestHelper.CreateCardPaymentRequest();
            var paymentResponse = await _api.Payments.RequestAPayment(paymentRequest);
            paymentResponse.Content.Payment.CanVoid().ShouldBe(true);

            VoidRequest voidRequest = new VoidRequest
            {
                Reference = Guid.NewGuid().ToString()
            };

            // Void Auth
            var voidResponse = await _api.Payments.VoidAPayment(paymentResponse.Content.Payment.Id, voidRequest);

            voidResponse.Content.ActionId.ShouldNotBeNullOrEmpty();
            voidResponse.Content.Reference.ShouldBe(voidRequest.Reference);
        }

        [Fact]
        public async Task CanRefundPayment()
        {
            // Auth
            var paymentRequest = TestHelper.CreateCardPaymentRequest();
            var paymentResponse = await _api.Payments.RequestAPayment(paymentRequest);
            paymentResponse.Content.Payment.CanCapture().ShouldBe(true);

            // Capture
            await _api.Payments.CaptureAPayment(paymentResponse.Content.Payment.Id);

            var refundRequest = new RefundRequest
            {
                Reference = Guid.NewGuid().ToString()
            };

            // Refund

            var refundResponse = await _api.Payments.RefundAPayment(paymentResponse.Content.Payment.Id, refundRequest);

            refundResponse.Content.ActionId.ShouldNotBeNullOrEmpty();
            refundResponse.Content.Reference.ShouldBe(refundRequest.Reference);
        }
    }
}
