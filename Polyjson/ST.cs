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
			STSettings.Converters.Add(new STJsonConverter<Animal>());
		}

		public static string SerializeTest()
		{
			return JsonSerializer.Serialize(1, STSettings);
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
}
