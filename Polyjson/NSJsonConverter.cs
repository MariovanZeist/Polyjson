using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Polyjson
{
	public class NSJsonConverter<T> : JsonConverter<T>
		where T : class
	{
		public override T ReadJson(JsonReader reader, Type objectType, [AllowNull] T existingValue, bool hasExistingValue, JsonSerializer serializer)
		{

			throw new NotImplementedException();
		}

		public override void WriteJson(JsonWriter writer, [AllowNull] T value, JsonSerializer serializer)
		{

			throw new NotImplementedException();
		}
	}
}
