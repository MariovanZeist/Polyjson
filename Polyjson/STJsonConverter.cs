using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Polyjson
{
	public class STJsonConverter<T> : JsonConverter<T>
		where T : class
	{
		const string TypeInfoName = "$$Type";
		static ConcurrentDictionary<Type, ConverterInfo> _converters = new ConcurrentDictionary<Type, ConverterInfo>();

		public override bool CanConvert(Type typeToConvert) => typeof(T).IsAssignableFrom(typeToConvert);

		public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			if (reader.TokenType != JsonTokenType.StartObject)
			{
				throw new JsonException();
			}

			reader.Read();
			if (reader.TokenType != JsonTokenType.PropertyName)
			{
				throw new JsonException();
			}

			string propertyName = reader.GetString();
			if (propertyName != TypeInfoName)
			{
				throw new JsonException();
			}

			reader.Read();
			if (reader.TokenType != JsonTokenType.String)
			{
				throw new JsonException();
			}

			var type = ConverterInfo.FindType(reader.GetString());
			var value = Activator.CreateInstance(type) as T;
			var converterInfo = GetConverterInfo(type);

			while (reader.Read())
			{
				if (reader.TokenType == JsonTokenType.EndObject)
				{
					return value;
				}

				if (reader.TokenType == JsonTokenType.PropertyName)
				{
					propertyName = reader.GetString();
					reader.Read();
					var propertyInfo = converterInfo.Properties.FirstOrDefault(p => p.Name == propertyName);
					propertyInfo.SetValue(value, JsonSerializer.Deserialize(ref reader, propertyInfo.PropertyType, options));
				}
			}
			throw new JsonException();
		}

		public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
		{
			writer.WriteStartObject();
			var converterInfo = GetConverterInfo(value.GetType());
			// Poly writer
			writer.WriteString(TypeInfoName, ConverterInfo.GetTypeName(value.GetType()));

			foreach (var propertyInfo in converterInfo.Properties)
			{
				writer.WritePropertyName(propertyInfo.Name);
				JsonSerializer.Serialize(writer, propertyInfo.GetValue(value), propertyInfo.PropertyType, options);
			}
			writer.WriteEndObject();
		}

		static ConverterInfo GetConverterInfo(Type type) => _converters.GetOrAdd(type, t => ConverterInfo.BuildFrom(t));
	}

}
