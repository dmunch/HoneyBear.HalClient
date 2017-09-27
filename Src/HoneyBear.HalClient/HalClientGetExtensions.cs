using HoneyBear.HalClient.Models;
using System.Linq;
using System.Threading.Tasks;

namespace HoneyBear.HalClient
{
    /// <summary>
    /// Extension methods implementing HTTP GET operations
    /// </summary>
    public static class HalClientGetExtensions
    {
        /// <summary>
        /// Navigates the given link relation and stores the the returned resource(s).
        /// </summary>
        /// <param name="client">The instance of the client used for the request.</param>
        /// <param name="rel">The link relation to follow.</param>
        /// <returns>A new instance of <see cref="IHalClient"/> with updated resources.</returns>
        /// <exception cref="FailedToResolveRelationship" />
        public static IHalClient Get(this IHalClient client, string rel) =>
            client.Get(rel, null, null);

        /// <summary>
        /// Navigates the given link relation and stores the the returned resource(s).
        /// </summary>
        /// <param name="client">The instance of the client used for the request.</param>
        /// <param name="rel">The link relation to follow.</param>
        /// <param name="curie">The curie of the link relation.</param>
        /// <returns>A new instance of <see cref="IHalClient"/> with updated resources.</returns>
        /// <exception cref="FailedToResolveRelationship" />
        public static IHalClient Get(this IHalClient client, string rel, string curie) => 
            client.Get(rel, null, curie);

        /// <summary>
        /// Navigates the given templated link relation and stores the the returned resource(s).
        /// </summary>
        /// <param name="client">The instance of the client used for the request.</param>
        /// <param name="rel">The templated link relation to follow.</param>
        /// <param name="parameters">An anonymous object containing the template parameters to apply.</param>
        /// <returns>A new instance of <see cref="IHalClient"/> with updated resources.</returns>
        /// <exception cref="FailedToResolveRelationship" />
        /// <exception cref="TemplateParametersAreRequired" />
        public static IHalClient Get(this IHalClient client, string rel, object parameters) => 
            client.Get(rel, parameters, null);

        /// <summary>
        /// Navigates the given templated link relation and stores the the returned resource(s).
        /// </summary>
        /// <param name="client">The instance of the client used for the request.</param>
        /// <param name="rel">The templated link relation to follow.</param>
        /// <param name="parameters">An anonymous object containing the template parameters to apply.</param>
        /// <param name="curie">The curie of the link relation.</param>
        /// <returns>A new instance of <see cref="IHalClient"/> with updated resources.</returns>
        /// <exception cref="FailedToResolveRelationship" />
        /// <exception cref="TemplateParametersAreRequired" />
        public static IHalClient Get(this IHalClient client, string rel, object parameters, string curie)
        {
            var relationship = HalClientExtensions.Relationship(rel, curie);

            var embedded = client.Current.FirstOrDefault(r => r.Embedded.Any(e => e.Rel == relationship));
            if (embedded != null)
            {
                var current = embedded.Embedded.Where(e => e.Rel == relationship);
                return new HalClient(client, current);
            }

            return client.BuildAndExecute(relationship, parameters, uri => client.Client.GetAsync(uri));
        }

        /// <summary>
        /// Navigates the given link relation and stores the the returned resource(s).
        /// </summary>
        /// <param name="client">The instance of the client used for the request.</param>
        /// <param name="resource">The current <see cref="IResource"/>.</param>
        /// <param name="rel">The link relation to follow.</param>
        /// <returns>A new instance of <see cref="IHalClient"/> with updated resources.</returns>
        /// <exception cref="FailedToResolveRelationship" />
        public static IHalClient Get(this IHalClient client, IResource resource, string rel) => 
            client.Get(resource, rel, null, null);

        /// <summary>
        /// Navigates the given link relation and stores the the returned resource(s).
        /// </summary>
        /// <param name="client">The instance of the client used for the request.</param>
        /// <param name="resource">The current <see cref="IResource"/>.</param>
        /// <param name="rel">The link relation to follow.</param>
        /// <param name="curie">The curie of the link relation.</param>
        /// <returns>A new instance of <see cref="IHalClient"/> with updated resources.</returns>
        /// <exception cref="FailedToResolveRelationship" />
        public static IHalClient Get(this IHalClient client, IResource resource, string rel, string curie) => 
            client.Get(resource, rel, null, curie);

        /// <summary>
        /// Navigates the given templated link relation and stores the the returned resource(s).
        /// </summary>
        /// <param name="client">The instance of the client used for the request.</param>
        /// <param name="resource">The current <see cref="IResource"/>.</param>
        /// <param name="rel">The templated link relation to follow.</param>
        /// <param name="parameters">An anonymous object containing the template parameters to apply.</param>
        /// <returns>A new instance of <see cref="IHalClient"/> with updated resources.</returns>
        /// <exception cref="FailedToResolveRelationship" />
        /// <exception cref="TemplateParametersAreRequired" />
        public static IHalClient Get(this IHalClient client, IResource resource, string rel, object parameters) => 
            client.Get(resource, rel, parameters, null);

        /// <summary>
        /// Navigates the given templated link relation and stores the the returned resource(s).
        /// </summary>
        /// <param name="client">The instance of the client used for the request.</param>
        /// <param name="resource">The current <see cref="IResource"/>.</param>
        /// <param name="rel">The templated link relation to follow.</param>
        /// <param name="parameters">An anonymous object containing the template parameters to apply.</param>
        /// <param name="curie">The curie of the link relation.</param>
        /// <returns>A new instance of <see cref="IHalClient"/> with updated resources.</returns>
        /// <exception cref="FailedToResolveRelationship" />
        /// <exception cref="TemplateParametersAreRequired" />
        public static IHalClient Get(this IHalClient client, IResource resource, string rel, object parameters, string curie)
        {
            var relationship = HalClientExtensions.Relationship(rel, curie);

            if (resource.Embedded.Any(e => e.Rel == relationship))
            {
                var current = resource.Embedded.Where(e => e.Rel == relationship);
                return new HalClient(client, current);
            }

            var link = resource.Links.FirstOrDefault(l => l.Rel == relationship);
            if (link == null)
                throw new FailedToResolveRelationship(relationship);

            return client.Execute(HalClientExtensions.Construct(link, parameters), uri => client.Client.GetAsync(uri));
        }

        /// <summary>
        /// Navigates the given link relation asynchronously and stores the the returned resource(s).
        /// </summary>
        /// <param name="clientTask">The Task yielding then client when awaited.</param>
        /// <param name="rel">The link relation to follow.</param>
        /// <returns>A new instance of <see cref="IHalClient"/> with updated resources.</returns>
        /// <exception cref="FailedToResolveRelationship" />
        public static Task<IHalClient> GetAsync(this Task<IHalClient> clientTask, string rel) =>
            clientTask.GetAsync(rel, null, null);

        /// <summary>
        /// Navigates the given link relation asynchronously and stores the the returned resource(s).
        /// </summary>
        /// <param name="clientTask">The Task yielding then client when awaited.</param>
        /// <param name="rel">The link relation to follow.</param>
        /// <param name="curie">The curie of the link relation.</param>
        /// <returns>A new instance of <see cref="IHalClient"/> with updated resources.</returns>
        /// <exception cref="FailedToResolveRelationship" />
        public static Task<IHalClient> GetAsync(this Task<IHalClient> clientTask, string rel, string curie) =>
            clientTask.GetAsync(rel, null, curie);

        /// <summary>
        /// Navigates the given templated link relation asynchronously and stores the the returned resource(s).
        /// </summary>
        /// <param name="clientTask">The Task yielding then client when awaited.</param>
        /// <param name="rel">The templated link relation to follow.</param>
        /// <param name="parameters">An anonymous object containing the template parameters to apply.</param>
        /// <returns>A new instance of <see cref="IHalClient"/> with updated resources.</returns>
        /// <exception cref="FailedToResolveRelationship" />
        /// <exception cref="TemplateParametersAreRequired" />
        public static Task<IHalClient> GetAsync(this Task<IHalClient> clientTask, string rel, object parameters) =>
            clientTask.GetAsync(rel, parameters, null);

        /// <summary>
        /// Navigates the given templated link relation asynchronously and stores the the returned resource(s).
        /// </summary>
        /// <param name="clientTask">The Task yielding then client when awaited.</param>
        /// <param name="rel">The templated link relation to follow.</param>
        /// <param name="parameters">An anonymous object containing the template parameters to apply.</param>
        /// <param name="curie">The curie of the link relation.</param>
        /// <returns>A new instance of <see cref="IHalClient"/> with updated resources.</returns>
        /// <exception cref="FailedToResolveRelationship" />
        /// <exception cref="TemplateParametersAreRequired" />
        public static async Task<IHalClient> GetAsync(this Task<IHalClient> clientTask, string rel, object parameters, string curie)
        {
            var relationship = HalClientExtensions.Relationship(rel, curie);
            var client = await clientTask;

            var embedded = client.Current.FirstOrDefault(r => r.Embedded.Any(e => e.Rel == relationship));
            if (embedded != null)
            {
                var current = embedded.Embedded.Where(e => e.Rel == relationship);
                return new HalClient(client, current);
            }

            return await client.BuildAndExecuteAsync(relationship, parameters, uri => client.Client.GetAsync(uri));
        }

        /// <summary>
        /// Navigates the given link relation and stores the the returned resource(s).
        /// </summary>
        /// <param name="clientTask">The Task yielding then client when awaited.</param>
        /// <param name="resource">The current <see cref="IResource"/>.</param>
        /// <param name="rel">The link relation to follow.</param>
        /// <returns>A new instance of <see cref="IHalClient"/> with updated resources.</returns>
        /// <exception cref="FailedToResolveRelationship" />
        public static Task<IHalClient> GetAsync(this Task<IHalClient> clientTask, IResource resource, string rel) =>
            clientTask.GetAsync(resource, rel, null, null);

        /// <summary>
        /// Navigates the given link relation and stores the the returned resource(s).
        /// </summary>
        /// <param name="clientTask">The Task yielding then client when awaited.</param>
        /// <param name="resource">The current <see cref="IResource"/>.</param>
        /// <param name="rel">The link relation to follow.</param>
        /// <param name="curie">The curie of the link relation.</param>
        /// <returns>A new instance of <see cref="IHalClient"/> with updated resources.</returns>
        /// <exception cref="FailedToResolveRelationship" />
        public static Task<IHalClient> GetAsync(this Task<IHalClient> clientTask, IResource resource, string rel, string curie) =>
            clientTask.GetAsync(resource, rel, null, curie);

        /// <summary>
        /// Navigates the given templated link relation and stores the the returned resource(s).
        /// </summary>
        /// <param name="clientTask">The Task yielding then client when awaited.</param>
        /// <param name="resource">The current <see cref="IResource"/>.</param>
        /// <param name="rel">The templated link relation to follow.</param>
        /// <param name="parameters">An anonymous object containing the template parameters to apply.</param>
        /// <returns>A new instance of <see cref="IHalClient"/> with updated resources.</returns>
        /// <exception cref="FailedToResolveRelationship" />
        /// <exception cref="TemplateParametersAreRequired" />
        public static Task<IHalClient> GetAsync(this Task<IHalClient> clientTask, IResource resource, string rel, object parameters) =>
            clientTask.GetAsync(resource, rel, parameters, null);

        /// <summary>
        /// Navigates the given templated link relation and stores the the returned resource(s).
        /// </summary>
        /// <param name="clientTask">The Task yielding then client when awaited.</param>
        /// <param name="resource">The current <see cref="IResource"/>.</param>
        /// <param name="rel">The templated link relation to follow.</param>
        /// <param name="parameters">An anonymous object containing the template parameters to apply.</param>
        /// <param name="curie">The curie of the link relation.</param>
        /// <returns>A new instance of <see cref="IHalClient"/> with updated resources.</returns>
        /// <exception cref="FailedToResolveRelationship" />
        /// <exception cref="TemplateParametersAreRequired" />
        public static async Task<IHalClient> GetAsync(this Task<IHalClient> clientTask, IResource resource, string rel, object parameters, string curie)
        {
            var relationship = HalClientExtensions.Relationship(rel, curie);

            if (resource.Embedded.Any(e => e.Rel == relationship))
            {
                var current = resource.Embedded.Where(e => e.Rel == relationship);
                var client = await clientTask;

                return new HalClient(client, current);
            }

            var link = resource.Links.FirstOrDefault(l => l.Rel == relationship);
            if (link == null)
                throw new FailedToResolveRelationship(relationship);

            var client2 = await clientTask;
            return client2.Execute(HalClientExtensions.Construct(link, parameters), uri => client2.Client.GetAsync(uri));
        }
    }
}
