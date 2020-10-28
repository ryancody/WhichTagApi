using System;
using System.Linq;
using WhichTag.TwitterClient.Models.Tweets;
using WhichTag.TwitterClient.Models.Users;
using WhichTagApi.Models.TrendData.Twitter;
using WhichTagApi.Services;
using WhichTagTweet = WhichTagApi.Models.TrendData.Twitter.Tweet;
using WhichTagUser = WhichTagApi.Models.TrendData.Twitter.User;

namespace WhichTagApi.Mappers
{
	public class TwitterDataMapper
	{
		private readonly ScoreCalculator scoreCalculator;

		public TwitterDataMapper (ScoreCalculator scoreCalculator)
		{
			this.scoreCalculator = scoreCalculator;
		}

		public TwitterTrend Map (string query, TweetsResponse tweetsResponse, UsersResponse usersResponse)
		{
			var metricSummary = new Metrics();

			var users = usersResponse?.data.ToDictionary(u => u.id);
			var tweets = tweetsResponse?.Data.ToDictionary(i => i.id, t =>
			{
				if (users.TryGetValue(t.author_id, out var user))
				{
					metricSummary.Likes += t.public_metrics.like_count;
					metricSummary.Quotes += t.public_metrics.quote_count;
					metricSummary.Replies += t.public_metrics.reply_count;
					metricSummary.Retweets += t.public_metrics.retweet_count;
					metricSummary.PossibleViews += user.public_metrics.followers_count;

					return new WhichTagTweet
					{
						Text = t.text,
						User = new WhichTagUser
						{
							Id = user.id,
							Name = user.name,
							Username = user.username,
							FollowersCount = user.public_metrics.followers_count
						},
						CreatedAt = t.created_at,
						Metrics = new Metrics
						{
							Likes = t.public_metrics.like_count,
							Quotes = t.public_metrics.quote_count,
							Replies = t.public_metrics.reply_count,
							Retweets = t.public_metrics.retweet_count
						}
					};
				}
				else
				{
					return null;
				}
			});

			return new TwitterTrend
			{
				QueriedAt = DateTime.UtcNow,
				Query = query,
				Tweets = tweets.Values,
				MetricSummary = metricSummary,
				OldestTweetCreatedAt = tweets.TryGetValue(tweetsResponse.Meta.oldest_id, out var tweet) ? (DateTime?)tweet.CreatedAt : null
			};
		}

		public TwitterTrendDto MapToDto (TwitterTrend twitterTrend)
		{
			return new TwitterTrendDto
			{
				Query = twitterTrend.Query,
				Tweets = twitterTrend.Tweets,
				Score = scoreCalculator.GetScore(twitterTrend),
				OldestTweetCreatedAt = twitterTrend.OldestTweetCreatedAt,
				QueriedAt = twitterTrend.QueriedAt
			};
		}
	}
}
