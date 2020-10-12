using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace WhichTagApi.Models.TrendData.Twitter
{
	[BsonIgnoreExtraElements]
	public class TwitterTrend
	{
		public string Query { get; set; }
		public IEnumerable<Tweet> Tweets { get; set; }
		public DateTime QueriedAt { get; set; }
		public Metrics MetricSummary { get; set; }
		public DateTime? OldestTweetCreatedAt { get; set; }
	}
}
