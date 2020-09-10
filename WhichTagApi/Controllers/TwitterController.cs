using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Twitter;
using Twitter.Models;

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

		[HttpGet]
		public async Task<TwitterResult> Get (string query)
		{
			return await twitter.GetTweetResults(query);
		}

		[HttpGet("{query}/data")]
		public async Task<TwitterResult> GetQuery (string query)
		{
			return await twitter.GetTweetResults(query);
		}
	}
}
