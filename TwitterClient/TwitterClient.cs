using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text;
using System.Collections.Generic;
using WhichTag.TwitterClient.Models.Tweets;
using WhichTag.TwitterClient.Models.Users;
using System.Linq;

namespace WhichTag.TwitterClient
{
	public class TwitterClient
	{
		private readonly HttpClient httpClient;

		public TwitterClient (HttpClient httpClient)
		{
			this.httpClient = httpClient;
		}

		public virtual async Task<TweetsResponse> GetTweets (string query)
		{
			return await Get<TweetsResponse>($"tweets/search/recent?query={query}&max_results=100&tweet.fields=created_at,public_metrics&expansions=author_id");
		}

		public virtual async Task<UsersResponse> GetUsers (IEnumerable<string> ids)
		{
			var idList = string.Join(',', ids.Distinct());

			return await Get<UsersResponse>($"users?ids={idList}&user.fields=public_metrics");
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

			return JsonSerializer.Deserialize<T>(responseJson, new JsonSerializerOptions()
			{
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase
			});
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

			return JsonSerializer.Deserialize<T>(responseJson, new JsonSerializerOptions()
			{
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase
			});
		}
	}
}
