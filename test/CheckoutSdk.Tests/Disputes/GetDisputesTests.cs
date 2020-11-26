using System.Collections;
using System.Threading.Tasks;
using Checkout.Disputes;
using Checkout.Exceptions;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace Checkout.Tests.Disputes
{
    public class GetDisputesTests : IClassFixture<ApiTestFixture>
    {
        private readonly ICheckoutApi _api;

        public GetDisputesTests(ApiTestFixture fixture, ITestOutputHelper outputHelper)
        {
            fixture.CaptureLogsInTestOutput(outputHelper);
            _api = fixture.Api;
        }

        [Fact]
        public async Task CanGetDisputes()
        {
            var getDisputesRequest = new GetDisputesRequest();
            var getDisputesResponse = await _api.Disputes.GetDisputes(getDisputesRequest: getDisputesRequest);

            getDisputesResponse.Content.ShouldNotBeNull();
            getDisputesResponse.Content.Limit.ShouldBe(50);
            getDisputesResponse.Content.Skip.ShouldBe(0);
            getDisputesResponse.Content.ThisChannelOnly.ShouldBeFalse();
        }

        [Fact]
        public async Task GivenInvalidDisputeIdShouldReturnZeroDisputes()
        {
            var getDisputesRequest = new GetDisputesRequest(id: "invalid");
            var getDisputesResponse = await _api.Disputes.GetDisputes(getDisputesRequest: getDisputesRequest);

            getDisputesResponse.Content.ShouldNotBeNull();
            getDisputesResponse.Content.Limit.ShouldBe(50);
            getDisputesResponse.Content.Skip.ShouldBe(0);
            getDisputesResponse.Content.ThisChannelOnly.ShouldBeFalse();
            getDisputesResponse.Content.TotalCount.ShouldBe(0);
            (getDisputesResponse.Content.Data as IList).Count.ShouldBe(0);
        }

        [Fact]
        public void GivenLimitOutOfBoundShouldThrowCheckoutApiException()
        {
            var getDisputesRequest = new GetDisputesRequest(limit: 251);
            var checkoutApiException = Should.Throw<CheckoutApiException>(async () => await _api.Disputes.GetDisputes(getDisputesRequest: getDisputesRequest));
            
            checkoutApiException.ShouldNotBeNull();
            checkoutApiException.StatusCode.ShouldBe((System.Net.HttpStatusCode)422);
        }
    }
}
