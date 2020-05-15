namespace GraphQLDoorNet
{
    using System;
    using System.Reflection;
    using global::GraphQL.Conventions;

    internal sealed class Injector : IDependencyInjector
    {
        private readonly IServiceProvider provider;

        public Injector(IServiceProvider provider)
        {
            this.provider = provider;
        }

        public object Resolve(TypeInfo typeInfo) => this.provider.GetService(typeInfo);
    }
}