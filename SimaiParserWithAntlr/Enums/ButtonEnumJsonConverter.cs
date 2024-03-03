using Newtonsoft.Json;

namespace SimaiParserWithAntlr.Enums;

public class ButtonEnumJsonConverter : JsonConverter<ButtonEnum>
{
    public override void WriteJson(JsonWriter writer, ButtonEnum value, JsonSerializer serializer)
    {
        int number = ButtonEnumExt.ToButtonNumber(value);
        writer.WriteValue(number);
    }

    public override ButtonEnum ReadJson(JsonReader reader, Type objectType, ButtonEnum existingValue, bool hasExistingValue,
        JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
        {
            return ButtonEnum.Unknown;
        }

        if (reader.TokenType != JsonToken.Integer) {
            throw new JsonSerializationException($"Unexpected token type. Expected Integer, got {reader.TokenType}");
        }

        int value = Convert.ToInt32(reader.Value);
        if (!ButtonEnumExt.TryParse(value, out var result)) {
            throw new JsonSerializationException($"Unable to parse {value} to FooEnum");
        }

        return result;
    }
}
