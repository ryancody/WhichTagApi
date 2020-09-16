using System;

namespace WhichTagApi.Models.TrendData.Twitter
{
	public class Tweet
	{
		public string Text { get; set; }
		public User User { get; set; }
		public Metrics Metrics { get; set; }
		public DateTime CreatedAt { get; set; }
	}
}
