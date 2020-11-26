using System.IO;
using System.Threading.Tasks;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace Checkout.Tests.Files
{
    public class UploadFileTest : IClassFixture<ApiTestFixture>
    {
        private readonly ICheckoutApi _api;

        public UploadFileTest(ApiTestFixture fixture, ITestOutputHelper outputHelper)
        {
            fixture.CaptureLogsInTestOutput(outputHelper);
            _api = fixture.Api;
        }

        [Fact]
        public async Task CanUploadFile()
        {
            var pathToFile = @"test_file.png";
            var uploadFileResponse = await _api.Files.UploadFile(pathToFile: pathToFile, purpose: "dispute_evidence");

            uploadFileResponse.Content.ShouldNotBeNull();
            uploadFileResponse.Content.Id.ShouldStartWith("file_");
        }

        [Fact]
        public void GivenInexistingFileShouldThrowFileNotFoundException()
        {
            var pathToFile = @"file_does_not_exist.png";
            var fileNotFoundException = Should.Throw<FileNotFoundException>(async () => await _api.Files.UploadFile(pathToFile: pathToFile, purpose: "dispute_evidence"));

            fileNotFoundException.ShouldNotBeNull();
        }

        [Fact]
        public void GivenUnsupportedFileTypeShouldThrowIOException()
        {
            var pathToFile = @"invalid_extension_test_file.txt";
            var IOException = Should.Throw<IOException>(async () => await _api.Files.UploadFile(pathToFile: pathToFile, purpose: "dispute_evidence"));

            IOException.ShouldNotBeNull();
        }
    }
}
