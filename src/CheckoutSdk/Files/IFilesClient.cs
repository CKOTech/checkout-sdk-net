using System.Net;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Checkout.Files
{
    /// <summary>
    /// Defines the operations available on the Checkout.com Files API.
    /// </summary>
    public interface IFilesClient
    {
        /// <summary>
        /// Upload a file to use as evidence in a dispute.
        /// Your file must be in either JPEG/JPG, PNG or PDF format, and be no larger than 4MB.
        /// </summary>
        /// <param name="pathToFile">The path of the file to upload.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the underlying HTTP request.</param>
        /// <returns>A task that upon completion returns the file upload response.</returns>
        Task<CheckoutHttpResponseMessage<UploadFileResponse>> UploadFile(string pathToFile, string purpose, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Returns all the details of a file using the file identifier.
        /// </summary>
        /// <param name="fileId">The file identifier string.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the underlying HTTP request.</param>
        /// <returns>A task that upon completion contains the matching file.</returns>
        Task<CheckoutHttpResponseMessage<File>> GetFileInformation(string fileId, CancellationToken cancellationToken = default(CancellationToken));
    }
}
