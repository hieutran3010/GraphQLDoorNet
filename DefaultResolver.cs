namespace GraphQLDoorNet
{
    using System.Linq;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;
    using GraphQL.Conventions;
    using Abstracts;
    using Models;

    public class DefaultResolver : IResolver
    {
        private readonly GraphQLEngine engine;
        private readonly IDependencyInjector injector;
        private readonly IUserContext userContext;

        public DefaultResolver(GraphQLEngine engine, IUserContext userContext, IDependencyInjector injector)
        {
            this.engine = engine;
            this.userContext = userContext;
            this.injector = injector;
        }

        public async Task<Response> Resolve(Stream body)
        {
            string requestBody;
            using (var reader = new StreamReader(body))
            {
                requestBody = await reader.ReadToEndAsync();
            }

            var result = await this.engine
                .NewExecutor()
                .WithUserContext(userContext)
                .WithDependencyInjector(this.injector)
                .WithRequest(requestBody)
                .Execute();

            var responseBody = this.engine.SerializeResult(result);

            var statusCode = HttpStatusCode.OK;

            if (result.Errors?.Any() ?? false)
            {
                statusCode = HttpStatusCode.InternalServerError;
                if (result.Errors.Any(x => x.Code == "VALIDATION_ERROR"))
                {
                    statusCode = HttpStatusCode.BadRequest;
                }
                else if (result.Errors.Any(x => x.Code == "UNAUTHORIZED_ACCESS"))
                {
                    statusCode = HttpStatusCode.Forbidden;
                }
            }

            return new Response
            {
                Content = responseBody,
                StatusCode = (int) statusCode
            };
        }
    }
}