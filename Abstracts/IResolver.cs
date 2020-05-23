namespace GraphQLDoorNet.Abstracts
{
    using System.Threading.Tasks;
    using System.IO;
    using Models;
    
    public interface IResolver
    {
        Task<Response> Resolve(Stream body);
    }
}