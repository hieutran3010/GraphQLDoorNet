using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using GraphQL.Conventions;
using GraphQLDoorNet.Abstracts;
using GraphQLDoorNet.Models;

namespace GraphQLDoorNet
{
    public class DefaultResolver : IResolver
    {
        private readonly GraphQLEngine _engine;
        private readonly IDependencyInjector _injector;
        private readonly IUserContext _userContext;

        public DefaultResolver(GraphQLEngine engine, IUserContext userContext, IDependencyInjector injector)
        {
            _engine = engine;
            _userContext = userContext;
            _injector = injector;
        }

        public async Task<Response> Resolve(Stream body)
        {
            string requestBody;
            using (var reader = new StreamReader(body))
            {
                requestBody = await reader.ReadToEndAsync();
            }

            var result = await _engine
                .NewExecutor()
                .WithUserContext(_userContext)
                .WithDependencyInjector(_injector)
                .WithRequest(requestBody)
                .ExecuteAsync();

            var responseBody = await _engine.SerializeResultAsync(result);

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