using System.Collections.Generic;

namespace GraphQLDoorNet.Models
{
    public class MutationResult
    {
        public bool DidSuccess { get; set; }
        public string ErrorCode { get; set; }
    }
}