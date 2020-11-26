using System;
using System.Net;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Checkout.Files
{
    /// <summary>
    /// Default implementation of <see cref="IFilesClient"/>.
    /// </summary>
    public class FilesClient : IFilesClient
    {
        private readonly IApiClient _apiClient;
        private readonly IApiCredentials _credentials;
        private const string path = "files";

        public FilesClient(IApiClient apiClient, CheckoutConfiguration configuration)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            _credentials = new SecretKeyCredentials(configuration);
        }

        public Task<CheckoutHttpResponseMessage<UploadFileResponse>> UploadFile(string pathToFile, string purpose, CancellationToken cancellationToken = default(CancellationToken))
        {
            var fileUploadMultipartFormDataContentRequest = new FileUploadMultipartFormDataContentRequest(pathToFile, purpose);

            return _apiClient.PostAsync<UploadFileResponse>(path, _credentials, cancellationToken, fileUploadMultipartFormDataContentRequest);
        }

        public Task<CheckoutHttpResponseMessage<File>> GetFileInformation(string fileId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _apiClient.GetAsync<File>($"{path}/{fileId}", _credentials, cancellationToken);
        }
    }
}
