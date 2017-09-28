using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HoneyBear.HalClient.Http;
using HoneyBear.HalClient.Models;
using Tavis.UriTemplates;

namespace HoneyBear.HalClient
{
    static class HalClientExtensions
    {
        /// <summary>
        /// Returns the most recently navigated resource of the specified type. 
        /// </summary>
        /// <typeparam name="T">The type of the resource to return.</typeparam>
        /// <returns>The most recent navigated resource of the specified type.</returns>
        /// <exception cref="NoActiveResource" />
        public static async Task<IResource<T>> ItemAsync<T>(this Task<IHalClient> client) where T : class, new() =>
            (await client).Item<T>();

        public static IResource<T> Item<T>(this IHalClient client) where T : class, new() =>
            Convert<T>(client.Latest());

        /// <summary>
        /// Returns the list of embedded resources in the most recently navigated resource.
        /// </summary>
        /// <typeparam name="T">The type of the resource to return.</typeparam>
        /// <returns>The list of embedded resources in the most recently navigated resource.</returns>
        /// <exception cref="NoActiveResource" />
        //public Task<IEnumerable<IResource<T>>> ItemsAsync<T>() where T : class, new() => _current.Select(Convert<T>);
        public static async Task<IEnumerable<IResource<T>>> ItemsAsync<T>(this Task<IHalClient> client) where T : class, new() =>
            (await client).Items<T>();

        public static IEnumerable<IResource<T>> Items<T>(this IHalClient client) where T : class, new() =>
            client.Current.Select(Convert<T>);

        public static Task<IHalClient> BuildAndExecuteAsync(this IHalClient client, string relationship, object parameters, Func<string, Task<HttpResponseMessage>> command)
        {
            var resource = client.Current.FirstOrDefault(r => r.Links.Any(l => l.Rel == relationship));
            if (resource == null)
                throw new FailedToResolveRelationship(relationship);

            var link = resource.Links.FirstOrDefault(l => l.Rel == relationship);
            return ExecuteAsync(client, Construct(link, parameters), command);
        }

        public static async Task<IHalClient> ExecuteAsync(this Task<IHalClient> clientTask, string uri, Func<IJsonHttpClient, string, Task<HttpResponseMessage>> command)
        {
            var client = await clientTask;
            return await ExecuteAsync(client, uri, _uri => command(client.Client, _uri));
        }

        public static async Task<IHalClient> ExecuteAsync(this IHalClientBase client, string uri, Func<string, Task<HttpResponseMessage>> command)
        {
            var result = await command(uri);

            AssertSuccessfulStatusCode(result);

            var current =
                new[]
                {
                    result.Content == null
                        ? new Resource()
                        : await result.Content.ReadAsAsync<Resource>(client.Formatters)
                };

            return new HalClient(client, current);
        }

        public static IHalClient BuildAndExecute(this IHalClient client, string relationship, object parameters, Func<string, Task<HttpResponseMessage>> command)
        {
            var resource = client.Current.FirstOrDefault(r => r.Links.Any(l => l.Rel == relationship));
            if (resource == null)
                throw new FailedToResolveRelationship(relationship);

            var link = resource.Links.FirstOrDefault(l => l.Rel == relationship);
            return client.Execute(Construct(link, parameters), command);
        }

        public static IHalClient Execute(this IHalClientBase client, string uri, Func<string, Task<HttpResponseMessage>> command)
        {
            var result = command(uri).Result;

            AssertSuccessfulStatusCode(result);

            var current =
                new[]
                {
                    result.Content == null
                        ? new Resource()
                        : result.Content.ReadAsAsync<Resource>(client.Formatters).Result
                };

            return new HalClient(client, current);
        }



        public static string Construct(ILink link, object parameters)
        {
            if (!link.Templated)
                return link.Href;

            if (parameters == null)
                throw new TemplateParametersAreRequired(link);

            var template = new UriTemplate(link.Href, caseInsensitiveParameterNames: true);
            template.AddParameters(parameters);
            return template.Resolve();
        }

        private static IResource<T> Convert<T>(IResource resource)
            where T : class, new() =>
                new Resource<T>
                {
                    Rel = resource.Rel,
                    Href = resource.Href,
                    Name = resource.Name,
                    Data = resource.Data<T>(),
                    Links = resource.Links,
                    Embedded = resource.Embedded
                };

        private static void AssertSuccessfulStatusCode(HttpResponseMessage result)
        {
            if (!result.IsSuccessStatusCode)
                throw new HttpRequestFailed(result.StatusCode);
        }

        private static IResource Latest(this IHalClient client)
        {
            if (client.Current == null || !client.Current.Any())
                throw new NoActiveResource();
            return client.Current.Last();
        }

        public static string Relationship(string rel, string curie) => curie == null ? rel : $"{curie}:{rel}";
    }
}
