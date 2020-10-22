using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Polyjson
{
	public class STJsonConverter : JsonConverter<Animal>
	{
		const string TypeInfoName = "$$Type";
		static ConcurrentDictionary<Type, ConverterInfo> _converters = new ConcurrentDictionary<Type, ConverterInfo>();

		public override bool CanConvert(Type typeToConvert)
		{
			return typeof(Animal).IsAssignableFrom(typeToConvert);
		}

		public override Animal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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
			var animal = Activator.CreateInstance(type) as Animal;
			var ci = GetConverterInfo(type);

			while (reader.Read())
			{
				if (reader.TokenType == JsonTokenType.EndObject)
				{
					return animal;
				}

				if (reader.TokenType == JsonTokenType.PropertyName)
				{
					propertyName = reader.GetString();
					reader.Read();
					var ps = ci.PropertyConverters.FirstOrDefault(p => p.PropertyInfo.Name == propertyName) as ConvertSTProperty;
					ps.ReadProperty(ref reader, animal);
				}
			}
			throw new JsonException();
		}

		public override void Write(Utf8JsonWriter writer, Animal value, JsonSerializerOptions options)
		{
			writer.WriteStartObject();
			var ci = GetConverterInfo(value.GetType());
			// Poly writer
			writer.WriteString(TypeInfoName, ConverterInfo.GetTypeName(value.GetType()));
			foreach (var item in ci.PropertyConverters)
			{
				if (item is ConvertSTProperty c)
				{
					c.WriteProperty(writer, value);
				}
			}
			writer.WriteEndObject();
		}

		static ConverterInfo GetConverterInfo(Type type) => _converters.GetOrAdd(type, t => ConvertSTInfo.Build(t));
	}
}
