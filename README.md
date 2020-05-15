# GraphQLDoorNet
When you build an api service, do you feel boring when repeatedly creating the same simple actions: get all, insert, update, delete, and each query type, you have to create a specific action? I'm really tired of that. That's why I rely on GraphQL, I try to build a library that helps to speed up that works. With this library, you can freely query from client with Linq and built-in support GetAll, paging, GetById, insert, update, delete. Of course, you can override the default action for the complex case.
I don't explain detail about GraphQL, you can easy find the clearly documentation at link https://graphql.org/, https://graphql-dotnet.github.io/

## Installation
This library requires .NET Core 3.x, SQL Database(PostgreSQL) - Currently, it tightly depends on [EFPostgresEngagement](https://github.com/hieutran3010/EFPostgresEngagement). In the near future, i will remove this dependency and support for all SQL Databases.

### Via Package manager
```sh
dotnet add package GraphQLDoorNet
```
### Via dotnet CLI
```sh
Install-Package GraphQLDoorNet
```

## Initialization
When mentioning to GraphQL, you have to define the Schema with Query and Mutation.
- Create the Query class and let's it inherits IQuery
```c#
internal sealed class Query: IQuery
{
}
```
- Create the Mutation class and let's it inherits IMutation
```c#
internal sealed class Mutation : IMutation
{
}
```
- Create a controller as a door between GraphQL and database. This is only an example, you can modify anything as you want
```c#
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using global::GraphQL.Conventions;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/graphql")]
    [ApiController]
    public class GraphQLController : ControllerBase
    {
        private readonly GraphQLEngine engine;
        private readonly IDependencyInjector injector;
        private readonly IUserContext userContext;

        public GraphQLController(GraphQLEngine engine, IUserContext userContext, IDependencyInjector injector)
        {
            this.engine = engine;
            this.userContext = userContext;
            this.injector = injector;
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            string requestBody;
            using (var reader = new StreamReader(this.Request.Body))
            {
                requestBody = await reader.ReadToEndAsync();
            }

            var result = await this.engine
                .NewExecutor()
                .WithUserContext(this.userContext)
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

            return new ContentResult
            {
                Content = responseBody,
                ContentType = "application/json; charset=utf-8",
                StatusCode = (int) statusCode
            };
        }
    }
```
- In Startup.cs, add the following code to ConfigureServices method:
```c#
public void ConfigureServices(IServiceCollection services)
{
  ...
  services.AddGraphQLServices<Query, Mutation>();
  ...
}
```
Finally, you completed the initialization for this library. Next step, let's explore how to use it.
# How to use?
Let's example, we develop a service to manage project. We have Project table and client need to CRUD on this table. To support that actions with this library, you follows the step:
- Open the Query class, add a Query object for Project table.
```c#
    internal sealed class Query: IQuery
    {
        public EntityQueryBase<Project> Project([Inject] EntityQueryBase<Project> entityQuery) => entityQuery;
    }
```
  The EntityQueryBase supports actions: QueryMany, GetById, QueryOne, Count. The details will be explained below. This class is freely overrided.
  
- For the CUD, it is mutation behaviors, these actions need the separate models rather than the root entities. So, let's create an input model for Project entity
```c#
[InputType(EntityType = typeof(Project))]
public class ProjectInput: OptionalWrapper
{
  public string Name { get; set; }
  public string Code { get; set; }
}
```
- Open the Mutation class, add an Mutation object for Project table
```c#
    internal sealed class Mutation : IMutation
    {
        public EntityMutationBase<Project, ProjectInput> Project(
            [Inject] EntityMutationBase<Project, ProjectInput> entityMutation) =>
            entityMutation;
    }
```
- DONE!
### Let's try to query by Postman. The endpoint should be: https://your-domain/api/graphql
#### Query all:
![alt text](https://firebasestorage.googleapis.com/v0/b/my-storage-2b677.appspot.com/o/my-libs%2Fgraphql-door-query-all.png?alt=media&token=ae0a2289-ee59-4b80-9573-4d2350e26c7d "Query All")
#### Query all with page 1 and page size = 2:
![alt text](https://firebasestorage.googleapis.com/v0/b/my-storage-2b677.appspot.com/o/my-libs%2Fgraphql-door-query-all-paging.png?alt=media&token=0c72d016-7cf3-4e58-8c55-23e92d152db0 "Query all with page 1 and page size = 2")
#### Query with condition:
![alt text](https://firebasestorage.googleapis.com/v0/b/my-storage-2b677.appspot.com/o/my-libs%2Fgraphql-door-query-by-condition.png?alt=media&token=6c01ad3a-ac74-43a3-bc04-420520175df3 "Query with condition")

*You can easily replace Project with other entity, queryMany with other action - please explore more action in EntityQueryBase*
### Let's try CUD by Postman
#### Add a new Project
![alt text](https://firebasestorage.googleapis.com/v0/b/my-storage-2b677.appspot.com/o/my-libs%2Fgraphql-door-add.png?alt=media&token=7723166c-a09f-453c-9d1b-c280259b563b "Query with condition")

*You can easily replace Project with other entity, add with other action - please explore more action in EntityMutationBase.*
### In the real world, you may need to override the default CUD or add more mutation. Just create a new mutation and inherit EntityMutationBase, then add it to the Mutation
```c#
    internal sealed class Mutation : IMutation
    {
        public ProjectMutation Project([Inject] ProjectMutation entityMutation) => entityMutation;
    }
```
Example, ProjectMutation has a custom action that EstablishProduct. Please don't forget to mark the Mutation attribute for the custom mutation.
```c#
[Mutation]
public class ProjectMutation : EntityMutationBase<Project, ProjectInput>
{
  public ProjectMutation(IUnitOfWork unitOfWork) : base(unitOfWork)
  {
  }

  public async Task<MutationResult> EstablishDatabase(ProjectDatabaseEstablishInput input)
  {
   ...
  }
}
```
![alt text](https://firebasestorage.googleapis.com/v0/b/my-storage-2b677.appspot.com/o/my-libs%2Fgraphql-door-custom-mutation.png?alt=media&token=25bc5931-cad2-418d-a47f-6f1564e4c47e "Custom mutation")
