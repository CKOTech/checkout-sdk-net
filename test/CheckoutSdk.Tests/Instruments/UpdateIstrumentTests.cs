using Checkout.Instruments;
using Shouldly;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Checkout.Tests.Instruments
{
    public class UpdateInstrumentTests : IClassFixture<ApiTestFixture>
    {
        private readonly ICheckoutApi _api;

        public UpdateInstrumentTests(ApiTestFixture fixture, ITestOutputHelper outputHelper)
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

            var updateInstrumentRequest = new UpdateInstrumentRequest() { ExpiryYear = (cardTokenRequest.ExpiryYear + 1)};

            var updateInstrumentResponse = await _api.Instruments.UpdateInstrumentDetails(createInstrumentResponse.Content.Id, updateInstrumentRequest);

            updateInstrumentResponse.Content.ShouldNotBeNull();
            updateInstrumentResponse.Content.Type.ShouldBe(createInstrumentResponse.Content.Type);

            var getInstrumentResponse = await _api.Instruments.GetInstrumentDetails(createInstrumentResponse.Content.Id);

            getInstrumentResponse.Content.ShouldNotBeNull();
            getInstrumentResponse.Content.ExpiryYear.ShouldBe(cardTokenRequest.ExpiryYear + 1);
        }
    }
}
