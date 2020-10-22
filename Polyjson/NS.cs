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
