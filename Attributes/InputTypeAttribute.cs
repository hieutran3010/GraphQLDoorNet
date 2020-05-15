namespace GraphQLDoorNet.Attributes
{
    using System;
    
    [AttributeUsage(AttributeTargets.Class)]
    public class InputTypeAttribute: global::GraphQL.Conventions.InputTypeAttribute
    {
        public Type EntityType { get; set; }
    }
}