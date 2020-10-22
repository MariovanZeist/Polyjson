using Newtonsoft.Json;

namespace Polyjson
{
	public static class NS
	{
		// NewtonSoft

		static JsonSerializerSettings NSSettings = new JsonSerializerSettings
		{
			Formatting = Formatting.Indented
		};

		static NS()
		{
			NSSettings.Converters.Add(new NSJsonConverter<Animal>());
		}

		public static string Serialize<T>(T data)
		{
			return JsonConvert.SerializeObject(data, NSSettings);
		}

		public static T DeSerialize<T>(string json)
		{
			return JsonConvert.DeserializeObject<T>(json, NSSettings);
		}
	}
}
