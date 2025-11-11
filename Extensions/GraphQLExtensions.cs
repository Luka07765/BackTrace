using Trace.GraphQL.Mutations;
using Trace.GraphQL.Mutations.Files;
using Trace.GraphQL.Mutations.Folders;
using Trace.GraphQL.Queries;
using Trace.GraphQL.Queries.Files;
using Trace.GraphQL.Queries.Folders;
using Trace.GraphQL.Subscriptions;


namespace Trace.Extensions
{
    public static class GraphQLExtensions
    {
        public static IServiceCollection AddGraphQLServerConfig(this IServiceCollection services)
        {
            services.AddGraphQLServer()
                .AddAuthorization()
                .AddQueryType<Query>()
                .AddTypeExtension<QueryFolders>()
                .AddTypeExtension<QueryFiles>()
                .AddMutationType<Mutation>()
                .AddTypeExtension<FoldersMutation>()
                .AddTypeExtension<FilesMutation>()
                .AddSubscriptionType<FolderSubscription>()
                .AddInMemorySubscriptions()
                .AddSocketSessionInterceptor<JwtWebSocketAuthInterceptor>()
                .ModifyRequestOptions(opt => opt.IncludeExceptionDetails = true);

            return services;
        }
    }
}
