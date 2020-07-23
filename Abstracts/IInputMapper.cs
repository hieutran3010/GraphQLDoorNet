namespace GraphQLDoorNet.Abstracts
{
    public interface IInputMapper
    {
        TDest Map<TSource, TDest>(TSource source);
        void MapUpdate<TSource, TDest>(TSource source, TDest dest);
    }
}