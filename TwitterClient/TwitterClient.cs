using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using Twitter.Models;
using System.Text;

namespace Twitter
{
	public class TwitterClient
	{
		private readonly HttpClient httpClient;

		public TwitterClient (HttpClient httpClient)
		{
			this.httpClient = httpClient;
		}

		public virtual async Task<TwitterResult> GetTweetResults (string query)
		{
			return await Get<TwitterResult>($"tweets/search/recent?query={query}&max_results=100&tweet.fields=created_at");
		}

		private async Task<T> Get<T> (string uri)
		{
			HttpResponseMessage response;

			try
			{
				response = await httpClient.GetAsync(uri);
			}
			catch (Exception e)
			{
				throw new HttpRequestException($"TwitterClient Get {httpClient.BaseAddress}/{uri} threw an exception: {e.Message}", e);
			}

			var responseJson = await response.Content.ReadAsStringAsync();

			if (!response.IsSuccessStatusCode)
			{
				throw new Exception($"TwitterClient Get {httpClient.BaseAddress}/{uri} returned {response.StatusCode}. Content: '{responseJson}'");
			}

			return JsonSerializer.Deserialize<T>(responseJson);
		}

		private async Task<T> Post<T> (string uri, string body)
		{
			var json = JsonSerializer.Serialize(body);
			var content = new StringContent(json, Encoding.UTF8, "application/json");

			HttpResponseMessage response;

			try
			{
				response = await httpClient.PostAsync(uri, content);
			}
			catch (Exception e)
			{
				throw new HttpRequestException($"TwitterClient Post {httpClient.BaseAddress}/{uri} threw an exception: {e.Message}", e);
			}

			var responseJson = await response.Content.ReadAsStringAsync();

			if (!response.IsSuccessStatusCode)
			{
				throw new Exception($"TwitterClient Post {httpClient.BaseAddress}/{uri} returned {response.StatusCode}. Content: '{responseJson}'");
			}

			return JsonSerializer.Deserialize<T>(responseJson);
		}
	}
}
