using Checkout.Instruments;
using Shouldly;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Checkout.Tests.Instruments
{
    public class GetInstrumentTests : IClassFixture<ApiTestFixture>
    {
        private readonly ICheckoutApi _api;

        public GetInstrumentTests(ApiTestFixture fixture, ITestOutputHelper outputHelper)
        {
            fixture.CaptureLogsInTestOutput(outputHelper);
            _api = fixture.Api;
        }

        [Fact]
        public async Task CanCreateInstrument()
        {
            var cardTokenRequest = TestHelper.CreateCardTokenRequest();
            var cardTokenResponse = await _api.Tokens.RequestAToken(cardTokenRequest);
            var instrumentRequest = new InstrumentRequest("token", cardTokenResponse.Content.Token);

            var createInstrumentResponse = await _api.Instruments.CreateAnInstrument(instrumentRequest);

            createInstrumentResponse.Content.ShouldNotBeNull();
            createInstrumentResponse.Content.ExpiryMonth.ShouldBe(cardTokenRequest.ExpiryMonth);
            createInstrumentResponse.Content.ExpiryYear.ShouldBe(cardTokenRequest.ExpiryYear);
            createInstrumentResponse.Content.Last4.ShouldBe(cardTokenResponse.Content.Last4);

            var getInstrumentResponse = await _api.Instruments.GetInstrumentDetails(createInstrumentResponse.Content.Id);

            getInstrumentResponse.Content.ShouldNotBeNull();
            getInstrumentResponse.Content.Id.ShouldBe(createInstrumentResponse.Content.Id);
        }
    }
}
