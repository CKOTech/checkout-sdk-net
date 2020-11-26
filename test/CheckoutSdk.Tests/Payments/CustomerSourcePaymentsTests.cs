using Shouldly;
using System.Threading.Tasks;
using Checkout.Common;
using Checkout.Payments;
using Xunit;
using Xunit.Abstractions;

namespace Checkout.Tests.Payments
{
    public class CustomerSourcePaymentsTests : IClassFixture<ApiTestFixture>
    {
        private readonly ICheckoutApi _api;

        public CustomerSourcePaymentsTests(ApiTestFixture fixture, ITestOutputHelper outputHelper)
        {
            fixture.CaptureLogsInTestOutput(outputHelper);
            _api = fixture.Api;
        }

        [Fact]
        public async Task CanRequestCardPayment()
        {
            var firstCardPayment = TestHelper.CreateCardPaymentRequest();
            var firstCardPaymentResponse = await _api.Payments.RequestAPayment(firstCardPayment);
            var customerSource = new CustomerSource(firstCardPayment.Customer.Id, firstCardPayment.Customer.Email);
            var customerPaymentRequest = new PaymentRequest<CustomerSource>(
                customerSource,
                Currency.GBP,
                100
            )
            {
                Capture = false,
            };

            var paymentResponse = await _api.Payments.RequestAPayment(customerPaymentRequest);

            paymentResponse.Content.Payment.ShouldNotBeNull();
            paymentResponse.Content.Payment.Approved.ShouldBeTrue();
            paymentResponse.Content.Payment.Id.ShouldNotBeNullOrEmpty();
            paymentResponse.Content.Payment.Id.ShouldNotBe(firstCardPaymentResponse.Content.Payment.Id);
            paymentResponse.Content.Payment.ActionId.ShouldNotBeNullOrEmpty();
            paymentResponse.Content.Payment.Amount.ShouldBe(customerPaymentRequest.Amount.Value);
            paymentResponse.Content.Payment.Currency.ShouldBe(customerPaymentRequest.Currency);
            paymentResponse.Content.Payment.Reference.ShouldBe(customerPaymentRequest.Reference);
            paymentResponse.Content.Payment.Customer.ShouldNotBeNull();
            paymentResponse.Content.Payment.Customer.Id.ShouldNotBeNullOrEmpty();
            paymentResponse.Content.Payment.Customer.Email.ShouldNotBeNullOrEmpty();
            paymentResponse.Content.Payment.Customer.Id.ShouldBe(firstCardPaymentResponse.Content.Payment?.Customer?.Id);
            paymentResponse.Content.Payment.Source.AsCard().ShouldNotBeNull();
            paymentResponse.Content.Payment.Source.AsCard().Id.ShouldBe(firstCardPaymentResponse.Content.Payment?.Source?.AsCard().Id);
            paymentResponse.Content.Payment.CanCapture().ShouldBeTrue();
            paymentResponse.Content.Payment.CanVoid().ShouldBeTrue();
        }
    }
}