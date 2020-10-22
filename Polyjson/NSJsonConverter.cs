using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Polyjson
{
	public class NSJsonConverter<T> : JsonConverter<T>
		where T : class
	{
		public override T ReadJson(JsonReader reader, Type objectType, [AllowNull] T existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			if (reader.TokenType != JsonToken.StartObject)
			{
				throw new JsonException();
			}

			reader.Read();
			if (reader.TokenType != JsonToken.PropertyName)
			{
				throw new JsonException();
			}

			if (reader.Value.ToString() != ConverterInfo.TypeInfoName)
			{
				throw new JsonException();
			}

			var type = ConverterInfo.FindType(reader.ReadAsString());
			var value = Activator.CreateInstance(type) as T;
			var converterInfo = ConverterInfo.GetConverterInfo(type);

			while (reader.Read())
			{
				if (reader.TokenType == JsonToken.EndObject)
				{
					return value;
				}

				if (reader.TokenType == JsonToken.PropertyName)
				{
					var propertyName = reader.Value.ToString();
					reader.Read();
					var propertyInfo = converterInfo.Properties.FirstOrDefault(p => p.Name == propertyName);
					propertyInfo.SetValue(value, serializer.Deserialize(reader, propertyInfo.PropertyType));
				}
			}
			throw new JsonException();
		}

		public override void WriteJson(JsonWriter writer, [AllowNull] T value, JsonSerializer serializer)
		{
			writer.WriteStartObject();
			var converterInfo = ConverterInfo.GetConverterInfo(value.GetType());
			// Poly writer
			writer.WritePropertyName(ConverterInfo.TypeInfoName);
			writer.WriteValue(ConverterInfo.GetTypeName(value.GetType()));
			foreach (var propertyInfo in converterInfo.Properties)
			{
				writer.WritePropertyName(propertyInfo.Name);
				serializer.Serialize(writer, propertyInfo.GetValue(value));
			}
			writer.WriteEndObject();
		}
	}
}
