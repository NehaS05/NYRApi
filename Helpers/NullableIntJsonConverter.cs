using System.Text.Json;
using System.Text.Json.Serialization;

namespace NYR.API.Helpers
{
    public class NullableIntJsonConverter : JsonConverter<int?>
    {
        public override int? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.Number:
                    return reader.GetInt32();
                case JsonTokenType.String:
                    var stringValue = reader.GetString();
                    if (string.IsNullOrEmpty(stringValue) || stringValue.ToLower() == "null")
                        return null;
                    if (int.TryParse(stringValue, out int result))
                        return result;
                    return null;
                case JsonTokenType.Null:
                    return null;
                default:
                    return null;
            }
        }

        public override void Write(Utf8JsonWriter writer, int? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
                writer.WriteNumberValue(value.Value);
            else
                writer.WriteNullValue();
        }
    }
}