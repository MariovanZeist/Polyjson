using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace Polyjson
{
	public static class ST
	{
		//S.T.Json

		static JsonSerializerOptions STSettings = new JsonSerializerOptions
		{
			WriteIndented = true,
		};

		static ST()
		{
			STSettings.Converters.Add(new STJsonConverter());
		}

		public static string Serialize<T>(T data )
		{
			return JsonSerializer.Serialize(data, STSettings);
		}

		public static T DeSerialize<T>(string json)
		{
			return JsonSerializer.Deserialize<T>(json, STSettings);
		}

		public static ReadOnlyMemory<byte> SerializeUtf8<T>(T data)
		{
			var buffer = new ArrayBufferWriter<byte>();
			using var wr = new Utf8JsonWriter(buffer);
			JsonSerializer.Serialize(wr, data, STSettings);
			return buffer.WrittenMemory;
		}

		public static T DeSerializeUtf8<T>(ReadOnlySpan<byte> utf8Json)
		{
			return JsonSerializer.Deserialize<T>(utf8Json, STSettings);
		}
	}


	public class ConvertSTInfo : ConverterInfo
	{
		static (Type PropertyType, Type ConverterType)[] _supportedTypes = new[] { (typeof(int), typeof(ConvertSTIntProperty)), (typeof(bool), typeof(ConvertSTBoolProperty)), (typeof(string), typeof(ConvertSTStringProperty)) };

		public ConvertSTInfo(IEnumerable<ConvertProperty> propertyConverters) : base(propertyConverters)
		{
		}

		public static ConverterInfo Build(Type type)
		{
			var list = new List<ConvertProperty>();
			foreach (var property in type.GetProperties())
			{
				var (propertyType, converterType) = _supportedTypes.FirstOrDefault(t => t.PropertyType == property.PropertyType);
				if (converterType != null && property.CanRead && property.CanWrite)
				{
					var convertProperty = Activator.CreateInstance(converterType) as ConvertProperty;
					convertProperty.PropertyInfo = property;
					list.Add(convertProperty);
				}
			}
			return new ConvertSTInfo(list);
		}
	}

	public abstract class ConvertProperty
	{
		public PropertyInfo PropertyInfo { get; set; }
	}

	public abstract class ConvertSTProperty : ConvertProperty
	{
		public abstract void ReadProperty(ref Utf8JsonReader reader, object value);
		public abstract void WriteProperty(Utf8JsonWriter writer, object value);
	}

	public class ConvertSTIntProperty : ConvertSTProperty
	{
		public override void ReadProperty(ref Utf8JsonReader reader, object value) => PropertyInfo.SetValue(value, reader.GetInt32());

		public override void WriteProperty(Utf8JsonWriter writer, object value) => writer.WriteNumber(PropertyInfo.Name, (int)PropertyInfo.GetValue(value));
	}

	public class ConvertSTStringProperty : ConvertSTProperty
	{
		public override void ReadProperty(ref Utf8JsonReader reader, object value) => PropertyInfo.SetValue(value, reader.GetString());
		public override void WriteProperty(Utf8JsonWriter writer, object value) => writer.WriteString(PropertyInfo.Name, (string)PropertyInfo.GetValue(value));
	}

	public class ConvertSTBoolProperty : ConvertSTProperty
	{
		public override void ReadProperty(ref Utf8JsonReader reader, object value) => PropertyInfo.SetValue(value, reader.GetBoolean());
		public override void WriteProperty(Utf8JsonWriter writer, object value) => writer.WriteBoolean(PropertyInfo.Name, (bool)PropertyInfo.GetValue(value));
	}
}
