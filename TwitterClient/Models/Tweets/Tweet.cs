using System;

namespace WhichTag.TwitterClient.Models.Tweets
{
	public class Tweet
	{
		public string id { get; set; }
		public string text { get; set; }
		public DateTime created_at { get; set; }
		public string author_id { get; set; }
		public PublicMetrics public_metrics { get; set; }
	}
}
