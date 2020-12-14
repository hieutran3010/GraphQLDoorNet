using System.Threading;
using System.Threading.Tasks;
using GraphQL.DataLoader;

namespace GraphQLDoorNet
{
    using GraphQL.Conventions;
    
    public class UserContext : IUserContext, IDataLoaderContextProvider
    {
        public DataLoaderContext Context { get; private set; }

        public UserContext(DataLoaderContext context)
        {
            Context = context;
        }

        public Task FetchData(CancellationToken token)
        {
            return Context.DispatchAllAsync(token);
        }
    }
}