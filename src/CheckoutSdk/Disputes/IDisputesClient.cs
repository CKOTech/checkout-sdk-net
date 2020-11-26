using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Checkout.Disputes
{
    /// <summary>
    /// Defines the operations available on the Checkout.com Disputes API.
    /// </summary>
    public interface IDisputesClient
    {
        /// <summary>
        /// Returns a list of all disputes against your business.
        /// The results will be returned in reverse chronological order, showing the last modified dispute (for example, where you've recently added a piece of evidence) first.
        /// You can use optional parameters to skip or limit results.
        /// </summary>
        /// <param name="getDisputesRequest">The parameters for filtering the disputes request.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the underlying HTTP request.</param>
        /// <returns>A task that upon completion contains the matching disputes.</returns>
        Task<CheckoutHttpResponseMessage<GetDisputesResponse>> GetDisputes(GetDisputesRequest getDisputesRequest, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Returns all the details of a dispute using the dispute identifier.
        /// </summary>
        /// <param name="disputeId">The dispute identifier string.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the underlying HTTP request.</param>
        /// <returns>A task that upon completion contains the matching dispute.</returns>
        Task<CheckoutHttpResponseMessage<Dispute>> GetDisputeDetails(string disputeId, CancellationToken cancellationToken = default(CancellationToken));


        /// <summary>
        /// If a dispute is legitimate, you can choose to accept it.
        /// This will close it for you and remove it from your list of open disputes.
        /// There are no further financial implications.
        /// </summary>
        /// <param name="disputeId">The dispute identifier string.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the underlying HTTP request.</param>
        /// <returns>A void task.</returns>
        Task<CheckoutHttpResponseMessage<dynamic>> AcceptDispute(string disputeId, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Adds supporting evidence to a dispute. Before using this endpoint, you first need to upload your files using the file uploader.
        /// You will receive a file id (prefixed by file_) which you can then use in your request. Note that this only attaches the evidence to the dispute, it does not send it to us.
        /// Once ready, you will need to submit it.
        /// </summary>
        /// <param name="disputeId">The dispute identifier string.</param>
        /// <param name="disputeEvidence">The dictionary that maps dispute evidence files and their description to evidence types.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the underlying HTTP request.</param>
        /// <returns>A void task.</returns>
        Task<CheckoutHttpResponseMessage<dynamic>> ProvideDisputeEvidence(string disputeId, DisputeEvidence disputeEvidence, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// With this final request, you can submit the evidence that you have previously provided.
        /// Make sure you have provided all the relevant information before using this request.
        /// You will not be able to amend your evidence once you have submitted it.
        /// </summary>
        /// <param name="disputeId">The dispute identifier string.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the underlying HTTP request.</param>
        /// <returns>A void task.</returns>
        Task<CheckoutHttpResponseMessage<dynamic>> SubmitDisputeEvidence(string disputeId, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Retrieves a list of the evidence submitted in response to a specific dispute.
        /// </summary>
        /// <param name="disputeId">The dispute identifier string.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the underlying HTTP request.</param>
        /// <returns>A void task.</returns>
        Task<CheckoutHttpResponseMessage<DisputeEvidenceResponse>> GetDisputeEvidence(string disputeId, CancellationToken cancellationToken = default(CancellationToken));
    }
}
