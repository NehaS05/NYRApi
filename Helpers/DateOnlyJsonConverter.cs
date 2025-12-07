using System.Text.Json;
using System.Text.Json.Serialization;

namespace NYR.API.Helpers
{
    public class DateOnlyJsonConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateTime.Parse(reader.GetString()!);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            // Write date in local time without timezone conversion
            writer.WriteStringValue(value.ToString("yyyy-MM-dd"));
        }
    }
}
