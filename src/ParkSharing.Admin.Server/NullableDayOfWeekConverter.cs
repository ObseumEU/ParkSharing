using System.Text.Json;
using System.Text.Json.Serialization;

public class NullableDayOfWeekConverter : JsonConverter<DayOfWeek?>
{
    public override DayOfWeek? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType == JsonTokenType.String)
        {
            var value = reader.GetString();
            if (Enum.TryParse<DayOfWeek>(value, true, out var dayOfWeek))
            {
                return dayOfWeek;
            }
        }

        throw new JsonException("Invalid DayOfWeek value.");
    }

    public override void Write(Utf8JsonWriter writer, DayOfWeek? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
        {
            writer.WriteStringValue(value.Value.ToString());
        }
        else
        {
            writer.WriteNullValue();
        }
    }
}
