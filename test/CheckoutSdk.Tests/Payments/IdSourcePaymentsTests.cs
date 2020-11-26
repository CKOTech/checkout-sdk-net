using Shouldly;
using System.Threading.Tasks;
using Checkout.Common;
using Checkout.Payments;
using Xunit;
using Xunit.Abstractions;

namespace Checkout.Tests.Payments
{
    public class IdSourcePaymentsTests : IClassFixture<ApiTestFixture>
    {
        private readonly ICheckoutApi _api;

        public IdSourcePaymentsTests(ApiTestFixture fixture, ITestOutputHelper outputHelper)
        {
            fixture.CaptureLogsInTestOutput(outputHelper);
            _api = fixture.Api;
        }

        [Fact]
        public async Task CanRequestCardPayment()
        {
            var firstCardPayment = TestHelper.CreateCardPaymentRequest();
            var firstCardPaymentResponse = await _api.Payments.RequestAPayment(firstCardPayment);
            var idSource = new IdSource(firstCardPaymentResponse.Content.Payment.Source.AsCard().Id);
            var cardIdPaymentRequest = new PaymentRequest<IdSource>(
                idSource,
                Currency.GBP,
                100
            )
            {
                Capture = false,
                Customer = ToRequest(firstCardPaymentResponse.Content.Payment.Customer)
            };

            var apiResponseForCustomerSourcePayment = await _api.Payments.RequestAPayment(cardIdPaymentRequest);

            apiResponseForCustomerSourcePayment.Content.Payment.ShouldNotBeNull();
            apiResponseForCustomerSourcePayment.Content.Payment.Approved.ShouldBeTrue();
            apiResponseForCustomerSourcePayment.Content.Payment.Id.ShouldNotBeNullOrEmpty();
            apiResponseForCustomerSourcePayment.Content.Payment.Id.ShouldNotBe(firstCardPaymentResponse.Content.Payment.Id);
            apiResponseForCustomerSourcePayment.Content.Payment.ActionId.ShouldNotBeNullOrEmpty();
            apiResponseForCustomerSourcePayment.Content.Payment.Amount.ShouldBe(cardIdPaymentRequest.Amount.Value);
            apiResponseForCustomerSourcePayment.Content.Payment.Currency.ShouldBe(cardIdPaymentRequest.Currency);
            apiResponseForCustomerSourcePayment.Content.Payment.Reference.ShouldBe(cardIdPaymentRequest.Reference);
            apiResponseForCustomerSourcePayment.Content.Payment.Customer.ShouldNotBeNull();
            apiResponseForCustomerSourcePayment.Content.Payment.Customer.Id.ShouldNotBeNullOrEmpty();
            apiResponseForCustomerSourcePayment.Content.Payment.Customer.Email.ShouldNotBeNullOrEmpty();
            apiResponseForCustomerSourcePayment.Content.Payment.Customer.Id.ShouldBe(firstCardPaymentResponse.Content.Payment?.Customer?.Id);
            apiResponseForCustomerSourcePayment.Content.Payment.Source.AsCard().Id.ShouldBe(firstCardPaymentResponse.Content.Payment?.Source?.AsCard().Id);
            apiResponseForCustomerSourcePayment.Content.Payment.CanCapture().ShouldBeTrue();
            apiResponseForCustomerSourcePayment.Content.Payment.CanVoid().ShouldBeTrue();
        }

        private CustomerRequest ToRequest(CustomerResponse customer)
        {
            return new CustomerRequest() { Id = customer.Id, Email = customer.Email, Name = customer.Name };
        }
    }
}