using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WhichTag.TwitterClient;
using WhichTag.TwitterClient.Models.Tweets;
using WhichTag.TwitterClient.Models.Users;

namespace WhichTagApi.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class TwitterController : ControllerBase
	{
		private readonly TwitterClient twitter;
		private readonly ILogger<TwitterController> logger;

		public TwitterController (ILogger<TwitterController> logger, TwitterClient twitter)
		{
			this.logger = logger;
			this.twitter = twitter;
		}

		[HttpGet("{query}")]
		public async Task<TweetsResponse> GetQuery (string query)
		{
			return await twitter.GetTweets(query);
		}

	}
}
