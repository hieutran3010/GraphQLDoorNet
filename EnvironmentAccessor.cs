using System;

namespace GraphQLDoorNet
{
    public class EnvironmentAccessor
    {
        public static EnvironmentAccessor Instance { get; } = new EnvironmentAccessor();

        public int MaximumItemsPerBatch
        {
            get
            {
                var config = Environment.GetEnvironmentVariable("MAXIMUM_ITEMS_PER_BATCH");
                if (int.TryParse(config, out var value))
                {
                    return value;
                }

                return 50;
            }
        }
    }
}