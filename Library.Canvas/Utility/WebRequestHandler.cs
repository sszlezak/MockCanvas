using Newtonsoft.Json;
using System.Text;

namespace Library.Canvas.Utility
{
	public class WebRequestHandler
	{
		private string host = "localhost";
		private string port = "7191";

		public WebRequestHandler() { }

		public async Task<string> Get(string url)
		{
			var fullUrl = $"https://{host}:{port}{url}";
			try
			{
				using (var client = new HttpClient())
				{
					var response = await client
						.GetStringAsync(fullUrl)
						.ConfigureAwait(false);
					return response;
				}
			}
			catch (Exception e)
			{
			}
			return string.Empty;
		}

		public async Task<string> Post(string url, object obj)
		{
			var fullUrl = $"https://{host}:{port}{url}";
			using (var client = new HttpClient())
			{
				using (var request = new HttpRequestMessage(HttpMethod.Post, fullUrl))
				{
					var json = JsonConvert.SerializeObject(obj, Library.Canvas.Utility.JsonHelper.Settings);
					using (var stringContent = new StringContent(json, Encoding.UTF8, "application/json"))
					{
						request.Content = stringContent;

						using (var response = await client
							.SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
							.ConfigureAwait(false))
						{
							if (response.IsSuccessStatusCode)
							{
								return await response.Content.ReadAsStringAsync();
							}
							return "ERROR";
						}
					}
				}
			}
		}

		public async Task<string> Delete(string url)
		{
			var fullUrl = $"https://{host}:{port}{url}";
			try
			{
				using (var client = new HttpClient())
				{
					using (var request = new HttpRequestMessage(HttpMethod.Delete, fullUrl))
					{
						using (var response = await client
							.SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
							.ConfigureAwait(false))
						{
							if (response.IsSuccessStatusCode)
							{
								return await response.Content.ReadAsStringAsync();
							}
							return "ERROR";
						}
					}
				}
			}
			catch (Exception e)
			{
			}
			return string.Empty;
		}
	}
}