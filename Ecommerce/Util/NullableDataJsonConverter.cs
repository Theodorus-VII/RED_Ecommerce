using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Ecommerce.Util
{
    public class NullableDataJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(ApiResponse<>);
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            JsonObjectContract? contract = serializer.ContractResolver.ResolveContract(value.GetType()) as JsonObjectContract ?? throw new JsonSerializationException("Invalid JSON contract.");
            writer.WriteStartObject();

            // Write Success and Message properties
            writer.WritePropertyName(nameof(ApiResponse<object>.Success));
            writer.WriteValue(contract.Properties[nameof(ApiResponse<object>.Success)].ValueProvider?.GetValue(value));

            writer.WritePropertyName(nameof(ApiResponse<object>.Message));
            writer.WriteValue(contract.Properties[nameof(ApiResponse<object>.Message)].ValueProvider?.GetValue(value));

            // Check if Data property is not null and write it
            var dataValue = contract.Properties[nameof(ApiResponse<object>.Data)].ValueProvider?.GetValue(value);
            if (dataValue != null)
            {
                writer.WritePropertyName(nameof(ApiResponse<object>.Data));
                writer.WriteValue(dataValue);
            }

            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer) => throw new NotImplementedException(); // Optional: Implement if needed
    }
}