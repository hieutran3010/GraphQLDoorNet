namespace GraphQLDoorNet.Models
{
    using global::GraphQL.Conventions;
    using global::GraphQL.Conventions.Attributes.Execution.Wrappers;

    [InputType]
    public class QueryParams: OptionalWrapper
    {
        public string Query { get; set; }
        public string Include { get; set; }
        public string OrderBy { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
    }
}