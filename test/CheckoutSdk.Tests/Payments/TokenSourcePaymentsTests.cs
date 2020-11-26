using Checkout.Payments;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Checkout.Tests.Payments
{
    public class TokenSourcePaymentsTests : IClassFixture<ApiTestFixture>
    {
        private readonly ICheckoutApi _api;

        public TokenSourcePaymentsTests(ApiTestFixture fixture, ITestOutputHelper outputHelper)
        {
            fixture.CaptureLogsInTestOutput(outputHelper);
            _api = fixture.Api;
        }

        [Fact]
        public async Task CanRequestNonThreeDsCardPayment()
        {
            var cardTokenRequest = TestHelper.CreateCardTokenRequest();
            var cardTokenResponse = await _api.Tokens.RequestAToken(cardTokenRequest);
            var paymentRequest = TestHelper.CreateTokenPaymentRequest(cardTokenResponse.Content.Token);
            paymentRequest.ThreeDS = false;

            var paymentResponse = await _api.Payments.RequestAPayment(paymentRequest);

            paymentResponse.Content.Payment.ShouldNotBeNull();
            paymentResponse.Content.Payment.Approved.ShouldBeTrue();
            paymentResponse.Content.Payment.Id.ShouldNotBeNullOrEmpty();
            paymentResponse.Content.Payment.ActionId.ShouldNotBeNullOrEmpty();
            paymentResponse.Content.Payment.Amount.ShouldBe(paymentRequest.Amount.Value);
            paymentResponse.Content.Payment.Currency.ShouldBe(paymentRequest.Currency);
            paymentResponse.Content.Payment.Reference.ShouldBe(paymentRequest.Reference);
            paymentResponse.Content.Payment.CanCapture().ShouldBeTrue();
            paymentResponse.Content.Payment.CanVoid().ShouldBeTrue();
            paymentResponse.Content.Payment.Source.AsCard().ShouldNotBeNull();
        }

        [Fact]
        public async Task CanRequestThreeDsCardPayment()
        {
            var cardTokenRequest = TestHelper.CreateCardTokenRequest();
            var cardTokenResponse = await _api.Tokens.RequestAToken(cardTokenRequest);
            var paymentRequest = TestHelper.CreateTokenPaymentRequest(cardTokenResponse.Content.Token);
            paymentRequest.ThreeDS = true;
            var apiResponse = await _api.Payments.RequestAPayment(paymentRequest);

            apiResponse.Content.IsPending.ShouldBe(true);
            var pending = apiResponse.Content.Pending;

            pending.ShouldNotBeNull();

            pending.Id.ShouldNotBeNullOrEmpty();
            pending.Reference.ShouldBe(paymentRequest.Reference);
            pending.ThreeDS.ShouldNotBeNull();
            pending.ThreeDS.Downgraded.ShouldBe(false);
            pending.ThreeDS.Enrolled.ShouldNotBeNullOrEmpty();
            pending.RequiresRedirect().ShouldBe(true);
            pending.GetRedirectLink().ShouldNotBeNull();
        }

        [Fact]
        public async Task CanCapturePayment()
        {
            var cardTokenRequest = TestHelper.CreateCardTokenRequest();
            var cardTokenResponse = await _api.Tokens.RequestAToken(cardTokenRequest);
            var paymentRequest = TestHelper.CreateTokenPaymentRequest(cardTokenResponse.Content.Token);
            var paymentResponse = await _api.Payments.RequestAPayment(paymentRequest);
            paymentResponse.Content.Payment.CanCapture().ShouldBe(true);

            var captureRequest = new CaptureRequest
            {
                Reference = Guid.NewGuid().ToString()
            };

            var captureResponse = await _api.Payments.CaptureAPayment(paymentResponse.Content.Payment.Id, captureRequest);

            captureResponse.Content.ActionId.ShouldNotBeNullOrEmpty();
            captureResponse.Content.Reference.ShouldBe(captureRequest.Reference);
        }

        [Fact]
        public async Task CanVoidPayment()
        {
            var paymentRequest = TestHelper.CreateCardPaymentRequest();
            var paymentResponse = await _api.Payments.RequestAPayment(paymentRequest);
            paymentResponse.Content.Payment.CanVoid().ShouldBe(true);

            var voidRequest = new VoidRequest
            {
                Reference = Guid.NewGuid().ToString()
            };

            var voidResponse = await _api.Payments.VoidAPayment(paymentResponse.Content.Payment.Id, voidRequest);

            voidResponse.Content.ActionId.ShouldNotBeNullOrEmpty();
            voidResponse.Content.Reference.ShouldBe(voidRequest.Reference);
        }

        [Fact]
        public async Task CanRefundPayment()
        {
            var cardTokenRequest = TestHelper.CreateCardTokenRequest();
            var cardTokenResponse = await _api.Tokens.RequestAToken(cardTokenRequest);
            var paymentRequest = TestHelper.CreateTokenPaymentRequest(cardTokenResponse.Content.Token);
            var paymentResponse = await _api.Payments.RequestAPayment(paymentRequest);
            paymentResponse.Content.Payment.CanCapture().ShouldBe(true);

            await _api.Payments.CaptureAPayment(paymentResponse.Content.Payment.Id);

            var refundRequest = new RefundRequest
            {
                Reference = Guid.NewGuid().ToString()
            };

            var refundResponse = await _api.Payments.RefundAPayment(paymentResponse.Content.Payment.Id, refundRequest);

            refundResponse.Content.ActionId.ShouldNotBeNullOrEmpty();
            refundResponse.Content.Reference.ShouldBe(refundRequest.Reference);
        }
    }
}