using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using HoneyBear.HalClient.Http;
using HoneyBear.HalClient.Models;

namespace HoneyBear.HalClient
{
    /// <summary>
    /// A lightweight fluent .NET client for navigating and consuming HAL APIs.
    /// </summary>
    public class HalClient : IHalClient
    {
        /// <summary>
        /// Gets the instance of the implementation of <see cref="IJsonHttpClient"/> used by the <see cref="HalClient"/>.
        /// </summary>
        public IJsonHttpClient Client { get { return _client; } }

        /// <summary>
        /// The most recently navigated resource.
        /// </summary>
        public IEnumerable<IResource> Current { get { return _current; } }

        /// <summary>
        /// The list of <see cref="MediaTypeFormatter"/>s in use.
        /// </summary>
        public IEnumerable<MediaTypeFormatter> Formatters { get { return _formatters; } }

        private readonly IJsonHttpClient _client;
        private readonly IEnumerable<MediaTypeFormatter> _formatters;
        private readonly IEnumerable<IResource> _current = Enumerable.Empty<IResource>();

        private static readonly ICollection<MediaTypeFormatter> _defaultFormatters =
            new[] { new HalJsonMediaTypeFormatter() };

        /// <summary>
        /// Creates an instance of the <see cref="HoneyBear.HalClient"/> class.
        /// </summary>
        /// <param name="client">The <see cref="System.Net.Http.HttpClient"/> to use.</param>
        /// <param name="formatters">
        /// Specifies the list of <see cref="MediaTypeFormatter"/>s to use.
        /// Default is <see cref="HalJsonMediaTypeFormatter"/>.
        /// </param>
        public HalClient(
            HttpClient client,
            ICollection<MediaTypeFormatter> formatters)
        {
            _client = new JsonHttpClient(client);
            _formatters = formatters == null || !formatters.Any() ? _defaultFormatters : formatters;
        }

        /// <summary>
        /// Creates an instance of the <see cref="HoneyBear.HalClient"/> class.
        /// </summary>
        /// <param name="client">The <see cref="System.Net.Http.HttpClient"/> to use.</param>
        public HalClient(
            HttpClient client)
            : this(client, _defaultFormatters)
        {

        }

        /// <summary>
        /// Creates an instance of the <see cref="HoneyBear.HalClient"/> class.
        /// Uses a default instance of <see cref="System.Net.Http.HttpClient"/>.
        /// </summary>
        public HalClient()
            : this(new HttpClient())
        {

        }

        /// <summary>
        /// Creates an instance of the <see cref="HoneyBear.HalClient"/> class.
        /// </summary>
        /// <param name="client">The implementation of <see cref="IJsonHttpClient"/> to use.</param>
        /// <param name="formatters">
        /// Specifies the list of <see cref="MediaTypeFormatter"/>s to use.
        /// Default is <see cref="HalJsonMediaTypeFormatter"/>.
        /// </param>
        public HalClient(
            IJsonHttpClient client,
            ICollection<MediaTypeFormatter> formatters)
        {
            _client = client;
            _formatters = formatters == null || !formatters.Any() ? _defaultFormatters : formatters;
        }

        /// <summary>
        /// Creates an instance of the <see cref="HoneyBear.HalClient"/> class.
        /// </summary>
        /// <param name="client">The implementation of <see cref="IJsonHttpClient"/> to use.</param>
        public HalClient(
            IJsonHttpClient client)
            : this(client, _defaultFormatters)
        {

        }

        /// <summary>
        /// Creates a copy of the specified client with given resources.
        /// </summary>
        /// <param name="client">The client to copy.</param>
        /// <param name="current">The new resources.</param>
        public HalClient(IHalClient client, IEnumerable<IResource> current)
        {
            _client = client.Client;
            _formatters = client.Formatters;
            _current = current;
        }
    }
}
