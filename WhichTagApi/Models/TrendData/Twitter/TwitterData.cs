using System;
using System.Collections.Generic;

namespace WhichTagApi.Models.TrendData.Twitter
{
	public class TwitterData
	{
		public string Query { get; set; }
		public IEnumerable<Tweet> Tweets { get; set; }
		public DateTime QueriedAt { get; set; }
	}
}
