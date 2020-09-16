namespace WhichTag.TwitterClient.Models
{
	public class TwitterClientConfiguration
	{
		public string BaseUrl { get; set; }
		public string AuthUrl { get; set; }
		public string Key { get; set; }
		public string Secret { get; set; }

		public bool IsValid =>
			!string.IsNullOrEmpty(BaseUrl)
			&& !string.IsNullOrEmpty(AuthUrl)
			&& !string.IsNullOrEmpty(Key)
			&& !string.IsNullOrEmpty(Secret);
	}
}