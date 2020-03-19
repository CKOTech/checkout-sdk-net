using System;
using System.Collections.Generic;
using System.Net;
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
        /// <typeparam name="TResult">The expected response type to be deserialized.</typeparam>
        /// <returns>A task that upon completion contains the specified API response data.</returns>
        Task<TResult> GetAsync<TResult>(string path, IApiCredentials credentials, CancellationToken cancellationToken);
        
        /// <summary>
        /// Executes a POST request to the specified <paramref="path"/>. 
        /// </summary>
        /// <param name="path">The API resource path.</param>
        /// <param name="credentials">The credentials used to authenticate the request.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the underlying HTTP request.</param>
        /// <param name="request">Optional data that should be sent in the request body.</param>
        /// <typeparam name="TResult">The expected response type to be deserialized.</typeparam>
        /// <returns>A task that upon completion contains the specified API response data.</returns>
        Task<TResult> PostAsync<TResult>(string path, IApiCredentials credentials, CancellationToken cancellationToken, object request = null, string idempotencyKey = null);
        
        /// <summary>
        /// Executes a POST request to the specified <paramref="path"/>. 
        /// </summary>
        /// <param name="path">The API resource path.</param>
        /// <param name="credentials">The credentials used to authenticate the request.</param>
        /// <param name="resultTypeMappings">A dictionary of type mappings for different response codes.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the underlying HTTP request.</param>
        /// <returns>A task that upon completion contains the specified API response data.</returns>
        /// <param name="request">Optional data that should be sent in the request body.</param>
        /// <returns>A task that upon completion contains the response type as determined by the <paramref="resultTypeMappings"/>.</returns>
        Task<dynamic> PostAsync(string path, IApiCredentials credentials, Dictionary<HttpStatusCode, Type> resultTypeMappings, CancellationToken cancellationToken, object request = null, string idempotencyKey = null);
    }
}