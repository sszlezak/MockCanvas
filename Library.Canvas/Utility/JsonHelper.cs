using Newtonsoft.Json;

namespace Library.Canvas.Utility
{
	public static class JsonHelper
	{
		public static JsonSerializerSettings Settings => new JsonSerializerSettings
		{
			TypeNameHandling = TypeNameHandling.Auto
		};
	}
}