using System.Linq;
using System.Security.Cryptography.X509Certificates;
using WhichTagApi.Models.TrendData.Twitter;

namespace WhichTagApi.Services
{
	public class ScoreCalculator
	{
		public double GetScore (TwitterTrend twitterTrend)
		{
			var span = (twitterTrend.QueriedAt - twitterTrend.OldestTweetCreatedAt).Value.TotalSeconds;
			var uniqueTweeters = twitterTrend.Tweets.Select(t => t.User).Distinct().Count();
			var tweetsPerSecond = uniqueTweeters / span;
			var timeTo100 = twitterTrend.Tweets.Count() / tweetsPerSecond;

			var score = 0.00;

			if (timeTo100 <= 60)
			{
				score = -0.17 * timeTo100 + 100.00;
			}
			else if (timeTo100 <= 60 * 60)
			{
				score = -0.00282 * timeTo100 + 90;
			}
			else if (timeTo100 <= 60 * 60 * 24)
			{
				score = -0.000121 * timeTo100 + 80;
			}
			else
			{
				score = -0.0001157 * timeTo100 + 70;
			}

			return score;
		}
	}
}
