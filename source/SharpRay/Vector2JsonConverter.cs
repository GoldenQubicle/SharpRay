using System.Text.Json;
using System.Text.Json.Serialization;

namespace SharpRay;

/// <summary>
/// 
/// </summary>
public class Vector2JsonConverter : JsonConverter<Vector2>
{
	public override Vector2 Read(
		ref Utf8JsonReader reader,
		Type typeToConvert,
		JsonSerializerOptions options)
	{
		if (reader.TokenType != JsonTokenType.StartObject)
		{
			throw new JsonException( );
		}

		var values = new List<float>( );

		while (reader.Read( ))
		{
			if (reader.TokenType == JsonTokenType.EndObject)
			{
				break;
			}

			if (reader.TokenType == JsonTokenType.Number && reader.TryGetSingle(out var value))
			{
				values.Add(value);
			}
		}

		return new Vector2(values[0], values[1]);

	}

	public override void Write(Utf8JsonWriter writer, Vector2 value, JsonSerializerOptions options)
	{
		writer.WriteStartObject( );
		writer.WriteNumber("X", value.X);
		writer.WriteNumber("Y", value.Y);
		writer.WriteEndObject( );
	}
}
