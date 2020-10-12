using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WhichTag.TwitterClient;
using WhichTagApi.Mappers;
using WhichTagApi.Models.TrendData.Twitter;
using WhichTagApi.Services;

namespace WhichTagApi.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class TwitterController : ControllerBase
	{
		private readonly TwitterClient twitter;
		private readonly MongoService mongoService;
		private readonly ILogger<TwitterController> logger;

		private readonly int READ_FROM_CACHE_SECONDS = 360;

		public TwitterController (ILogger<TwitterController> logger, TwitterClient twitter, MongoService mongoService)
		{
			this.logger = logger;
			this.twitter = twitter;
			this.mongoService = mongoService;
		}

		[HttpPost("{query}")]
		public async Task<IActionResult> PostTrendQuery (string query, [FromBody] QueryRequestBody body)
		{
			var cachedTrend = mongoService.FindLatestTrendQuery(query);
			var querySibling = new QuerySibling
			{
				 Query = query,
				 Siblings = body.Siblings
			};

			await mongoService.InsertSiblingRecord(querySibling);

			if (DateTime.Compare(DateTime.UtcNow, cachedTrend.QueriedAt.AddSeconds(READ_FROM_CACHE_SECONDS)) < 0)
			{
				return Ok(cachedTrend);
			}

			var tweets = await twitter.GetTweets(query);
			var ids = tweets.Data?.Select(t => t.author_id);

			if (ids != null)
			{
				var users = await twitter.GetUsers(ids);
				var twitterTrend = TwitterDataMapper.Map(query, tweets, users);

				await mongoService.Create(twitterTrend);

				return Ok(twitterTrend);
			}
		
			return NoContent();
		}
	}
}
