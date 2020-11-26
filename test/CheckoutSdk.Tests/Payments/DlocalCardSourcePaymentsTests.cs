using Shouldly;
using System;
using System.Threading.Tasks;
using Checkout.Payments;
using Xunit;
using Xunit.Abstractions;

namespace Checkout.Tests.Payments
{
    public class DlocalCardSourcePaymentsTests : IClassFixture<ApiTestFixture>
    {
        private readonly ICheckoutApi _api;

        public DlocalCardSourcePaymentsTests(ApiTestFixture fixture, ITestOutputHelper outputHelper)
        {
            fixture.CaptureLogsInTestOutput(outputHelper);
            _api = fixture.Api;
        }

        [Fact]
        public async Task CanRequestNonThreeDsCardPayment()
        {
            var paymentRequest = TestHelper.CreateDlocalCardPaymentRequest();
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

            // ToDo : Why is this test expecting a returned processing object, should TPA's be returning it?
            // paymentResponse.Payment.Processing.ShouldNotBeNull();
            // paymentResponse.Payment.Processing.AcquirerTransactionId.ShouldNotBeNullOrWhiteSpace();
            // paymentResponse.Payment.Processing.RetrievalReferenceNumber.ShouldNotBeNullOrWhiteSpace();
        }


        [Fact]
        public async Task CanVoidPayment()
        {
            // Auth
            var paymentRequest = TestHelper.CreateDlocalCardPaymentRequest();
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
            var paymentRequest = TestHelper.CreateDlocalCardPaymentRequest();
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
