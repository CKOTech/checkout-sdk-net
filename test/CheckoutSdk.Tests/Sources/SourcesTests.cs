using Checkout.Common;
using Checkout.Sources;
using Shouldly;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Checkout.Tests.Sources
{
    public class SourcesTests : IClassFixture<ApiTestFixture>
    {
        private readonly ICheckoutApi _api;

        public SourcesTests(ApiTestFixture fixture, ITestOutputHelper outputHelper)
        {
            fixture.CaptureLogsInTestOutput(outputHelper);
            _api = fixture.Api;
        }

        [Fact]
        public async Task CanRequestSepaSource()
        {
            var sourceRequest = CreateSepaSourceRequest();
            var sourceResponse = await _api.Sources.AddAPaymentSource(sourceRequest);

            sourceResponse.ShouldNotBeNull();
            var source = sourceResponse.Content.Source;
            source.Customer.ShouldBeOfType<CustomerResponse>();
            source.Id.ShouldNotBeNullOrEmpty();
            source.Links.ShouldBeOfType<Dictionary<string, Link>>();
            source.ResponseCode.ShouldBe("10000");
            source.ResponseData.ShouldNotBeNull();
            source.Type.ShouldBe(sourceRequest.Type, StringCompareShould.IgnoreCase);
            source.ResponseData.ShouldContainKey("mandate_reference");
        }

        private static SourceRequest CreateSepaSourceRequest()
        {
            return new SourceRequest(
                type: "sepa",
                billingAddress: new Address()
                {
                    AddressLine1 = "Checkout.com",
                    AddressLine2 = "90 Tottenham Court Road",
                    City = "London",
                    State = "London",
                    Zip = "W1T 4TJ",
                    Country = "GB"
                })
            {
                Reference = ".NET SDK test",
                Phone = new Phone()
                {
                    CountryCode = "+1",
                    Number = "415 555 2671"
                },
                SourceData = new SourceData()
                {
                    { "first_name", "Marcus" },
                    { "last_name", "Barrilius Maximus" },
                    { "account_iban", "DE68100100101234567895" },
                    { "bic", "PBNKDEFFXXX" },
                    { "billing_descriptor", ".NET SDK test" },
                    { "mandate_type", "single" }
                }
            };
        }
    }
}
