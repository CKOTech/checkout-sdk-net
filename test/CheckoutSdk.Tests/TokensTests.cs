using Shouldly;
using System;
using System.Threading.Tasks;
using Checkout.Common;
using Checkout.Tokens;
using Xunit;
using Xunit.Abstractions;

namespace Checkout.Tests
{
    public class TokensTests : IClassFixture<ApiTestFixture>
    {
        private readonly ICheckoutApi _api;

        public TokensTests(ApiTestFixture fixture, ITestOutputHelper outputHelper)
        {
            fixture.CaptureLogsInTestOutput(outputHelper);
            _api = fixture.Api;
        }

        [Fact]
        public async Task CanTokenizeCard()
        {
            CardTokenRequest request = CreateValidRequest();

            var token = await _api.Tokens.RequestAToken(request);

            token.Content.ShouldNotBeNull();
            token.Content.Token.ShouldNotBeNullOrEmpty();
            token.Content.ExpiresOn.ShouldBeGreaterThan(DateTime.UtcNow);
            token.Content.BillingAddress.ShouldNotBeNull();
            token.Content.BillingAddress.AddressLine1.ShouldBe(request.BillingAddress.AddressLine1);
            token.Content.BillingAddress.AddressLine2.ShouldBe(request.BillingAddress.AddressLine2);
            token.Content.BillingAddress.City.ShouldBe(request.BillingAddress.City);
            token.Content.BillingAddress.State.ShouldBe(request.BillingAddress.State);
            token.Content.BillingAddress.Zip.ShouldBe(request.BillingAddress.Zip);
            token.Content.BillingAddress.Country.ShouldBe(request.BillingAddress.Country);
            token.Content.Phone.ShouldNotBeNull();
            token.Content.Phone.CountryCode.ShouldBe(request.Phone.CountryCode);
            token.Content.Phone.Number.ShouldBe(request.Phone.Number);
            token.Content.Type.ShouldBe("card");
            token.Content.ExpiryMonth.ShouldBe(request.ExpiryMonth);
            token.Content.ExpiryYear.ShouldBe(request.ExpiryYear);
        }

        private CardTokenRequest CreateValidRequest()
        {
            return new CardTokenRequest(TestCardSource.Visa.Number, TestCardSource.Visa.ExpiryMonth, TestCardSource.Visa.ExpiryYear)
            {
                Cvv = TestCardSource.Visa.Cvv,
                BillingAddress = new Address
                {
                    AddressLine1 = "Checkout.com",
                    AddressLine2 = "90 Tottenham Court Road",
                    City = "London",
                    State = "London",
                    Zip = "W1T 4TJ",
                    Country = "GB"
                },
                Phone = new Phone
                {
                    CountryCode = "44",
                    Number = "020 222333"
                }
            };
        }
    }
}