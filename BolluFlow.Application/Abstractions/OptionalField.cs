using System.Text.Json;
using System.Text.Json.Serialization;

namespace BolluFlow.Application.Abstractions;

[JsonConverter(typeof(OptionalFieldJsonConverterFactory))]
public readonly record struct OptionalField<T>
{
    public bool IsSet { get; init; }
    public T? Value { get; init; }

    public static OptionalField<T> FromValue(T? value) => new()
    {
        IsSet = true,
        Value = value
    };
}

public class OptionalFieldJsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(OptionalField<>);
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var valueType = typeToConvert.GetGenericArguments()[0];
        var converterType = typeof(OptionalFieldJsonConverter<>).MakeGenericType(valueType);
        return (JsonConverter)Activator.CreateInstance(converterType)!;
    }
}

public class OptionalFieldJsonConverter<T> : JsonConverter<OptionalField<T>>
{
    public override bool HandleNull => true;

    public override OptionalField<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = JsonSerializer.Deserialize<T>(ref reader, options);
        return OptionalField<T>.FromValue(value);
    }

    public override void Write(Utf8JsonWriter writer, OptionalField<T> value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.Value, options);
    }
}