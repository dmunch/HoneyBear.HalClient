using System.Collections.Generic;
using System.Net.Http.Formatting;
using HoneyBear.HalClient.Http;
using HoneyBear.HalClient.Models;

namespace HoneyBear.HalClient
{
    /// <summary>
    /// A lightweight fluent .NET client for navigating and consuming HAL APIs without state.
    /// </summary>
    public interface IHalClientBase
    {
        /// <summary>
        /// Gets the instance of the implementation of <see cref="IJsonHttpClient"/> used by the <see cref="HalClient"/>.
        /// </summary>
        IJsonHttpClient Client { get; }
        
        /// <summary>
        /// The list of <see cref="MediaTypeFormatter"/>s in use.
        /// </summary>
        IEnumerable<MediaTypeFormatter> Formatters { get; }
    }

    /// <summary>
    /// A lightweight fluent .NET client for navigating and consuming HAL APIs containing the current state.
    /// </summary>
    public interface IHalClient : IHalClientBase
    {
        /// <summary>
        /// The most recently navigated resource.
        /// </summary>
        /// 
        IEnumerable<IResource> Current { get; }
    }
}