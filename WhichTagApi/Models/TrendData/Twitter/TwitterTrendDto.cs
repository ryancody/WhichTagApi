using System;
using System.Collections.Generic;

namespace WhichTagApi.Models.TrendData.Twitter
{
	public class TwitterTrendDto
	{
		public string Query { get; set; }
		public IEnumerable<Tweet> Tweets { get; set; }
		public DateTime QueriedAt { get; set; }
		public double Score { get; set; }
		public DateTime? OldestTweetCreatedAt { get; set; }
	}
}
