using System.Linq;
using System.Reflection;
using GraphQLDoorNet.Attributes;
using Mapster;

namespace GraphQLDoorNet
{
    public static class ObjectMapper
    {
        public static void Mapping()
        {
            var config = new TypeAdapterConfig();
            var inputTypes = Assembly.GetCallingAssembly().GetTypes()
                .Where(t => t.GetCustomAttributes(typeof(InputTypeAttribute), true).Length > 0);
            foreach (var inputType in inputTypes)
            {
                var inputTypeAttribute = inputType.GetCustomAttribute<InputTypeAttribute>();
                if (inputTypeAttribute != null)
                {
                    config.ForType(inputType, inputTypeAttribute.EntityType);
                }
            }
        }
    }
}