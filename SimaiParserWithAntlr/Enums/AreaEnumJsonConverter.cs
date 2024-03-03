using Newtonsoft.Json;

namespace SimaiParserWithAntlr.Enums;

public class AreaEnumJsonConverter : JsonConverter<AreaEnum>
{
    public override void WriteJson(JsonWriter writer, AreaEnum value, JsonSerializer serializer)
    {
        string result = AreaEnumExt.ToFormattedString(value);
        writer.WriteValue(result);
    }

    public override AreaEnum ReadJson(JsonReader reader, Type objectType, AreaEnum existingValue, bool hasExistingValue,
        JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
        {
            return AreaEnum.Unknown;
        }

        if (reader.TokenType != JsonToken.Integer) {
            throw new JsonSerializationException($"Unexpected token type. Expected Integer, got {reader.TokenType}");
        }

        string value = reader.Value!.ToString() ?? string.Empty;
        if (!AreaEnumExt.TryParse(value, out var result)) {
            throw new JsonSerializationException($"Unable to parse {value} to FooEnum");
        }

        return result;
    }
}
