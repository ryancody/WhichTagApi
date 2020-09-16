using System.Text.Json.Serialization;

namespace WhichTag.TwitterClient.Models.Tweets
{
	public class TweetsResponse
	{
		public Tweet[] Data { get; set; }
		public Meta Meta { get; set; }
	}
}