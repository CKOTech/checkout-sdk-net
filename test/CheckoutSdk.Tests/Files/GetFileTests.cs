using System.IO;
using System.Threading.Tasks;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace Checkout.Tests.Files
{
    public class GetFileTests : IClassFixture<ApiTestFixture>
    {
        private readonly ICheckoutApi _api;

        public GetFileTests(ApiTestFixture fixture, ITestOutputHelper outputHelper)
        {
            fixture.CaptureLogsInTestOutput(outputHelper);
            _api = fixture.Api;
        }

        [Fact]
        public async Task CanGetFile()
        {
            // upload a file first
            var pathToFile = @"test_file.png";
            var fileInfo = new FileInfo(fileName: pathToFile);
            var uploadFileResponse = await _api.Files.UploadFile(pathToFile: fileInfo.FullName, purpose: "dispute_evidence");

            uploadFileResponse.Content.ShouldNotBeNull();

            // use returned fileID to try and get file details
            var getFileResponse = await _api.Files.GetFileInformation(fileId: uploadFileResponse.Content.Id);

            getFileResponse.Content.ShouldNotBeNull();
            getFileResponse.Content.Id.ShouldBe(uploadFileResponse.Content.Id);
            getFileResponse.Content.Filename.ShouldBe(fileInfo.Name);
            getFileResponse.Content.Size.ShouldBe((int)fileInfo.Length);
        }
    }
}
