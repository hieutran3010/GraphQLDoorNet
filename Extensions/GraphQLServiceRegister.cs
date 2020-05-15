
namespace GraphQLDoorNet.Extensions
{
    using Abstracts;
    using global::GraphQL.Conventions;
    using Microsoft.Extensions.DependencyInjection;
    using System.Linq;
    using System.Reflection;
    using Attributes;


    public static class GraphQLServiceRegister
    {
        public static void AddGraphQLServices<TQuery, TMutation>(this IServiceCollection services)
            where TQuery : IQuery
            where TMutation : IMutation
        {
            services.AddSingleton(provider => new GraphQLEngine()
                .WithFieldResolutionStrategy(FieldResolutionStrategy.Normal)
                .BuildSchema(typeof(SchemaDefinition<TQuery, TMutation>)));

            services.AddScoped<IDependencyInjector, Injector>();

            services.AddScoped(typeof(TQuery));
            services.AddScoped(typeof(EntityQueryBase<>));

            services.AddScoped(typeof(TMutation));
            services.AddScoped(typeof(EntityMutationBase<,>));
            
            var mutationTypes = Assembly.GetCallingAssembly().GetTypes()
                .Where(t => t.GetCustomAttributes(typeof(MutationAttribute), true).Length > 0);

            foreach (var mutationType in mutationTypes)
            {
                services.AddScoped(mutationType);
            }
        }
    }
}