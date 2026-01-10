using Trace.GraphQL.Mutations;
using Trace.GraphQL.Mutations.Domains;
using Trace.GraphQL.Mutations.Files;
using Trace.GraphQL.Mutations.Folders;
using Trace.GraphQL.Mutations.Tags;
using Trace.GraphQL.Queries;
using Trace.GraphQL.Queries.Files;
using Trace.GraphQL.Queries.Folders;
using Trace.GraphQL.Queries.Profile;
using Trace.GraphQL.Queries.Search;
using Trace.GraphQL.Queries.Tag;
using Trace.GraphQL.Queries.Domains;    
using Trace.GraphQL.Subscriptions;

namespace Trace.Registrations
{
    public static class GraphQL_Registration
    {
        public static IServiceCollection Register_GraphQLServer(this IServiceCollection services)
        {
            services.AddGraphQLServer()
                .AddAuthorization()
                .AddQueryType<Query>()
                .AddTypeExtension<QueryDomains>()       
                .AddTypeExtension<QueryTags>()
                .AddTypeExtension<QueryFolders>()
                .AddTypeExtension<QueryFiles>()
                 .AddTypeExtension<QueryProfile>()  
                .AddMutationType<Mutation>()
                .AddTypeExtension<DomainsMutation>()    
                .AddTypeExtension<TagsMutation>()
                .AddTypeExtension<FoldersMutation>()
                .AddTypeExtension<FilesMutation>()
                .AddSubscriptionType<FolderSubscription>()
                .AddTypeExtension<QuerySearch>()
                .AddInMemorySubscriptions()
                .AddSocketSessionInterceptor<JwtWebSocketAuthInterceptor>()
                .ModifyRequestOptions(opt => opt.IncludeExceptionDetails = true);

            return services;
        }
    }
}
