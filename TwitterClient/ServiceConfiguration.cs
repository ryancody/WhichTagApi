using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Twitter.Models;

namespace Twitter
{
	public static class ServiceConfiguration
	{
		private static readonly string authEndpoint = "token";

		public static void AddTwitterClient (this IServiceCollection services, TwitterClientConfiguration config)
		{
			if (config == null || !config.IsValid)
			{
				throw new Exception($"{nameof(config)} is invalid");
			}

			var bearerToken = GetBearerToken(config).GetAwaiter().GetResult();

			services.AddHttpClient<TwitterClient>(client =>
			{
				client.BaseAddress = new Uri($"{config.BaseUrl}/api");
				client.DefaultRequestHeaders.Add("Authorization", $"Bearer {bearerToken}");
			});
		}

		private static async Task<string> GetBearerToken (TwitterClientConfiguration config)
		{
			var base64key = Base64Encode($"{config.Key}:{config.Secret}");
			
			var client = new HttpClient();
			client.BaseAddress = new Uri(config.AuthUrl);
			client.DefaultRequestHeaders.Add("Authorization", $"Basic {base64key}");

			var content = new StringContent("", Encoding.UTF8, "application/json");

			HttpResponseMessage response;

			try
			{
				response = await client.PostAsync("oauth2/token?grant_type=client_credentials", content);
			}
			catch (Exception e)
			{
				throw new Exception($"Unable to get bearer token authorization: {e.Message}", e);
			}

			var responseJson = await response.Content.ReadAsStringAsync();

			if (!response.IsSuccessStatusCode)
			{
				throw new Exception($"Failed to get bearer token authorization. Returned status ${response.IsSuccessStatusCode} from ${config.AuthUrl}/${authEndpoint}");
			}

			return JsonSerializer.Deserialize<TwitterAuthorizationResponse>(responseJson).access_token;
		}

		public static string Base64Encode (string plainText)
		{
			var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
			return Convert.ToBase64String(plainTextBytes);
		}
	}
}
