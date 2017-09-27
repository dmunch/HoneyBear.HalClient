namespace HoneyBear.HalClient
{
    using Models;

    /// <summary>
    /// Extension methods implementing HTTP PATCH operations
    /// </summary>
    public static class HalClientPatchExtensions
    {
        /// <summary>
        /// Makes a HTTP PATCH request to the given templated link relation on the most recently navigated resource.
        /// </summary>
        /// <param name="client">The instance of the client used for the request.</param>
        /// <param name="rel">The templated link relation to follow.</param>
        /// <param name="value">The payload to PATCH.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        /// <exception cref="FailedToResolveRelationship" />
        public static IHalClient Patch(this IHalClient client, string rel, object value) => 
            client.Patch(rel, value, null, null);

        /// <summary>
        /// Makes a HTTP PATCH request to the given link relation on the most recently navigated resource.
        /// </summary>
        /// <param name="client">The instance of the client used for the request.</param>
        /// <param name="rel">The link relation to follow.</param>
        /// <param name="value">The payload to PATCH.</param>
        /// <param name="curie">The curie of the link relation.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        /// <exception cref="FailedToResolveRelationship" />
        public static IHalClient Patch(this IHalClient client, string rel, object value, string curie) => 
            client.Patch(rel, value, null, curie);

        /// <summary>
        /// Makes a HTTP PATCH request to the given templated link relation on the most recently navigated resource.
        /// </summary>
        /// <param name="client">The instance of the client used for the request.</param>
        /// <param name="rel">The templated link relation to follow.</param>
        /// <param name="value">The payload to PATCH.</param>
        /// <param name="parameters">An anonymous object containing the template parameters to apply.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        /// <exception cref="FailedToResolveRelationship" />
        /// <exception cref="TemplateParametersAreRequired" />
        public static IHalClient Patch(this IHalClient client, string rel, object value, object parameters) => 
            client.Patch(rel, value, parameters, null);

        /// <summary>
        /// Makes a HTTP PATCH request to the given templated link relation on the most recently navigated resource.
        /// </summary>
        /// <param name="client">The instance of the client used for the request.</param>
        /// <param name="rel">The templated link relation to follow.</param>
        /// <param name="value">The payload to PATCH.</param>
        /// <param name="parameters">An anonymous object containing the template parameters to apply.</param>
        /// <param name="curie">The curie of the link relation.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        /// <exception cref="FailedToResolveRelationship" />
        /// <exception cref="TemplateParametersAreRequired" />
        public static IHalClient Patch(this IHalClient client, string rel, object value, object parameters, string curie)
        {
            var relationship = HalClientExtensions.Relationship(rel, curie);

            return client.BuildAndExecute(relationship, parameters, uri => client.Client.PatchAsync(uri, value));
        }
    }
}
