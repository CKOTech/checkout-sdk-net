using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Checkout
{
    /// <summary>
    /// Defines the operations used for communicating with Checkout.com APIs.
    /// </summary>
    public interface IApiClient
    {
        /// <summary>
        /// Executes a GET request to the specified <paramref="path"/>. 
        /// </summary>
        /// <param name="path">The API resource path.</param>
        /// <param name="credentials">The credentials used to authenticate the request.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the underlying HTTP request.</param>
        /// <typeparam name="TContentResult">The expected response type to be deserialized.</typeparam>
        /// <returns>A task that upon completion contains the specified API response data.</returns>
        Task<CheckoutHttpResponseMessage<TContentResult>> GetAsync<TContentResult>(string path, IApiCredentials credentials, CancellationToken cancellationToken);

        /// <summary>
        /// Executes a POST request to the specified <paramref="path"/>. 
        /// </summary>
        /// <param name="path">The API resource path.</param>
        /// <param name="credentials">The credentials used to authenticate the request.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the underlying HTTP request.</param>
        /// <param name="request">Optional data that should be sent in the request body.</param>
        /// <param name="idempotencyKey">Optional idempotency key to safely retry payment requests.</param>
        /// <typeparam name="TContentResult">The expected response type to be deserialized.</typeparam>
        /// <returns>A task that upon completion contains the specified API response data.</returns>
        Task<CheckoutHttpResponseMessage<TContentResult>> PostAsync<TContentResult>(string path, IApiCredentials credentials, CancellationToken cancellationToken, object request = null, string idempotencyKey = null);

        /// <summary>
        /// Executes a POST request to the specified <paramref="path"/>. 
        /// </summary>
        /// <param name="path">The API resource path.</param>
        /// <param name="credentials">The credentials used to authenticate the request.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the underlying HTTP request.</param>
        /// <param name="httpContent">Optional data that should be sent in the request body.</param>
        /// <param name="idempotencyKey">Optional idempotency key to safely retry payment requests.</param>
        /// <typeparam name="TContentResult">The expected response type to be deserialized.</typeparam>
        /// <returns>A task that upon completion contains the specified API response data.</returns>
        Task<CheckoutHttpResponseMessage<TContentResult>> PostAsync<TContentResult>(string path, IApiCredentials credentials, CancellationToken cancellationToken, HttpContent httpContent = null, string idempotencyKey = null);

        /// <summary>
        /// Executes a POST request to the specified <paramref="path"/>. 
        /// </summary>
        /// <param name="path">The API resource path.</param>
        /// <param name="credentials">The credentials used to authenticate the request.</param>
        /// <param name="resultTypeMappings">A dictionary of type mappings for different response codes.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the underlying HTTP request.</param>
        /// <param name="request">Optional data that should be sent in the request body.</param>
        /// <param name="idempotencyKey">Optional idempotency key to safely retry payment requests.</param>
        /// <returns>A task that upon completion contains the response type as determined by the <paramref="resultTypeMappings"/>.</returns>
        Task<CheckoutHttpResponseMessage<dynamic>> PostAsync(string path, IApiCredentials credentials, Dictionary<HttpStatusCode, Type> resultTypeMappings, CancellationToken cancellationToken, object request = null, string idempotencyKey = null);

        /// <summary>
        /// Executes a PUT request to the specified <paramref="path"/>. 
        /// </summary>
        /// <param name="path">The API resource path.</param>
        /// <param name="credentials">The credentials used to authenticate the request.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the underlying HTTP request.</param>
        /// <param name="request">Optional data that should be sent in the request body.</param>
        /// <typeparam name="TContentResult">The expected response type to be deserialized.</typeparam>
        /// <returns>A task that upon completion contains the specified API response data.</returns>
        Task<CheckoutHttpResponseMessage<TContentResult>> PutAsync<TContentResult>(string path, IApiCredentials credentials, CancellationToken cancellationToken, object request = null);

        /// <summary>
        /// Executes a PATCH request to the specified <paramref="path"/>. 
        /// </summary>
        /// <param name="path">The API resource path.</param>
        /// <param name="credentials">The credentials used to authenticate the request.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the underlying HTTP request.</param>
        /// <param name="request">Optional data that should be sent in the request body.</param>
        /// <typeparam name="TContentResult">The expected response type to be deserialized.</typeparam>
        /// <returns>A task that upon completion contains the specified API response data.</returns>
        Task<CheckoutHttpResponseMessage<TContentResult>> PatchAsync<TContentResult>(string path, IApiCredentials credentials, CancellationToken cancellationToken, object request = null);

        /// <summary>
        /// Executes a DELETE request to the specified <paramref="path"/>. 
        /// </summary>
        /// <param name="path">The API resource path.</param>
        /// <param name="credentials">The credentials used to authenticate the request.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the underlying HTTP request.</param>
        /// <typeparam name="TContentResult">The expected response type to be deserialized.</typeparam>
        /// <returns>A task that upon completion contains the specified API response data.</returns>
        Task<CheckoutHttpResponseMessage<TContentResult>> DeleteAsync<TContentResult>(string path, IApiCredentials credentials, CancellationToken cancellationToken);
    }
}
