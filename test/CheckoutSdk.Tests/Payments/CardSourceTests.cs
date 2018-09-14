using Shouldly;
using System;
using System.Threading.Tasks;
using Checkout.Payments;
using Xunit;
using Xunit.Abstractions;

namespace Checkout.Tests.Payments
{
    public class CardSourceTests : IClassFixture<ApiTestFixture>
    {
        public CardSourceTests(ApiTestFixture fixture, ITestOutputHelper outputHelper)
        {
            fixture.CaptureLogsInTestOutput(outputHelper);
            Api = fixture.Api;
        }

        public ICheckoutApi Api { get; }

        [Fact]
        public async Task RequestNonThreeDsCardPayment()
        {
            PaymentRequest<CardSource> paymentRequest = TestHelper.CreateCardPaymentRequest();
            paymentRequest.ThreeDs = false;

            PaymentResponse apiResponse = await Api.Payments.RequestAsync(paymentRequest);
            
            apiResponse.Payment.ShouldNotBeNull();
            apiResponse.Payment.Approved.ShouldBeTrue();
            apiResponse.Payment.Id.ShouldNotBeNullOrEmpty();
            apiResponse.Payment.ActionId.ShouldNotBeNullOrEmpty();
            apiResponse.Payment.Amount.ShouldBe(paymentRequest.Amount.Value);
            apiResponse.Payment.Currency.ShouldBe(paymentRequest.Currency);
            apiResponse.Payment.Reference.ShouldBe(paymentRequest.Reference);
            apiResponse.Payment.Customer.ShouldNotBeNull();
            apiResponse.Payment.Customer.Id.ShouldNotBeNullOrEmpty();
            apiResponse.Payment.Customer.Email.ShouldNotBeNullOrEmpty();
            apiResponse.Payment.CanCapture().ShouldBeTrue();
            apiResponse.Payment.CanVoid().ShouldBeTrue();
        }

        [Fact]
        public async Task RequestThreeDsCardPayment()
        {
            PaymentRequest<CardSource> paymentRequest = TestHelper.CreateCardPaymentRequest();
            paymentRequest.ThreeDs = true;

            PaymentResponse apiResponse = await Api.Payments.RequestAsync(paymentRequest);

            apiResponse.IsPending.ShouldBe(true);
            var pending = apiResponse.Pending;

            pending.ShouldNotBeNull();

            pending.Id.ShouldNotBeNullOrEmpty();
            pending.Reference.ShouldBe(paymentRequest.Reference);
            pending.Customer.ShouldNotBeNull();
            pending.Customer.Id.ShouldNotBeNullOrEmpty();
            pending.Customer.Email.ShouldBe(paymentRequest.Customer.Email);
            pending.ThreeDs.ShouldNotBeNull();
            pending.ThreeDs.Downgraded.ShouldBe(false);
            //pending.ThreeDs.Enrolled.ShouldNotBeNullOrEmpty(); //todo uncomment after 2018-09-20
            pending.RequiresRedirect().ShouldBe(true);
            pending.GetRedirectLink().ShouldNotBeNull();
        }

        [Fact]
        public async Task ItCanCapturePayment()
        {
            // Auth
            var paymentRequest = TestHelper.CreateCardPaymentRequest();
            var paymentResponse = await Api.Payments.RequestAsync(paymentRequest);
            paymentResponse.Payment.CanCapture().ShouldBe(true);

            CaptureRequest captureRequest = new CaptureRequest
            {
                Reference = Guid.NewGuid().ToString()
            };

            // Capture
            var captureResponse = await Api.Payments.CaptureAsync(paymentResponse.Payment.Id, captureRequest);

            captureResponse.ActionId.ShouldNotBeNullOrEmpty();
            captureResponse.Reference.ShouldBe(captureRequest.Reference);
        }

        [Fact]
        public async Task ItCanVoidPayment()
        {
            // Auth
            var paymentRequest = TestHelper.CreateCardPaymentRequest();
            var paymentResponse = await Api.Payments.RequestAsync(paymentRequest);
            paymentResponse.Payment.CanVoid().ShouldBe(true);

            VoidRequest voidRequest = new VoidRequest
            {
                Reference = Guid.NewGuid().ToString()
            };

            // Void Auth
            var voidResponse = await Api.Payments.VoidAsync(paymentResponse.Payment.Id, voidRequest);

            voidResponse.ActionId.ShouldNotBeNullOrEmpty();
            voidResponse.Reference.ShouldBe(voidRequest.Reference);
        }

        [Fact]
        public async Task ItCanRefundPayment()
        {
            // Auth
            var paymentRequest = TestHelper.CreateCardPaymentRequest();
            var paymentResponse = await Api.Payments.RequestAsync(paymentRequest);
            paymentResponse.Payment.CanCapture().ShouldBe(true);

            // Capture
            await Api.Payments.CaptureAsync(paymentResponse.Payment.Id);

            var refundRequest = new RefundRequest
            {
                Reference = Guid.NewGuid().ToString()
            };

            // Refund

            var refundResponse = await Api.Payments.RefundAsync(paymentResponse.Payment.Id, refundRequest);

            refundResponse.ActionId.ShouldNotBeNullOrEmpty();
            refundResponse.Reference.ShouldBe(refundRequest.Reference);
        }

        [Fact]
        public async Task ItCanGetPayment()
        {
            var paymentRequest = TestHelper.CreateCardPaymentRequest();
            var paymentResponse = await Api.Payments.RequestAsync(paymentRequest);

            var paymentDetails = await Api.Payments.GetAsync(paymentResponse.Payment.Id);
            paymentDetails.ShouldNotBeNull();
        }
    }
}