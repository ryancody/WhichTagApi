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
		private readonly TwitterDataMapper mapper;

		private readonly int READ_FROM_CACHE_SECONDS = 360;

		public TwitterController (ILogger<TwitterController> logger, TwitterClient twitter, MongoService mongoService, TwitterDataMapper mapper)
		{
			this.logger = logger;
			this.twitter = twitter;
			this.mongoService = mongoService;
			this.mapper = mapper;
		}

		[HttpPost("{query}")]
		public async Task<IActionResult> Query (string query, [FromBody] QueryRequestBody body)
		{
			var trend = mongoService.FindLatestTrendQuery(query);
			var querySibling = new QuerySibling
			{
				 Query = query,
				 Siblings = body.Siblings
			};

			await mongoService.InsertSiblingRecord(querySibling);

			if (trend?.QueriedAt != null && DateTime.Compare(DateTime.UtcNow, trend.QueriedAt.AddSeconds(READ_FROM_CACHE_SECONDS)) < 0)
			{
				var trendDto = mapper.MapToDto(trend);
				return Ok(trendDto);
			}

			var tweets = await twitter.GetTweets(query);
			var ids = tweets.Data?.Select(t => t.author_id);

			if (ids != null)
			{
				var users = await twitter.GetUsers(ids);
				var twitterTrend = mapper.Map(query, tweets, users);

				await mongoService.InsertTrend(twitterTrend);

				var twitterTrendDto = mapper.MapToDto(twitterTrend);

				return Ok(twitterTrendDto);
			}
		
			return NoContent();
		}
	}
}
