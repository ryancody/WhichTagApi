using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WhichTag.TwitterClient;
using WhichTag.TwitterClient.Models.Tweets;
using WhichTag.TwitterClient.Models.Users;
using WhichTagApi.Mappers;
using WhichTagApi.Models.TrendData.Twitter;

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
		public async Task<IActionResult> GetQuery (string query)
		{
			var tweets = await twitter.GetTweets(query);
			var ids = tweets.Data?.Select(t => t.author_id);

			if (ids != null)
			{
				var users = await twitter.GetUsers(ids);
				return Ok(TwitterDataMapper.Map(query, tweets, users));
			}
			else
			{
				return NoContent();
			}
		}
	}
}
