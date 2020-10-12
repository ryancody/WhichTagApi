using Microsoft.Extensions.Logging;
using MongoConfiguration;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhichTagApi.Models.TrendData.Twitter;

namespace WhichTagApi.Services
{
	public class MongoService
	{
		private readonly IMongoCollection<TwitterTrend> twitterTrends;
		private readonly IMongoCollection<QuerySibling> querySiblings;

		public MongoService (ILogger<MongoService> logger, MongoClientConfiguration mongoConfig)
		{
			var client = new MongoClient($"mongodb+srv://{mongoConfig.Username}:{mongoConfig.Password}@{mongoConfig.ClusterUrl}/{mongoConfig.DbName}?retryWrites=true&w=majority");
			var database = client.GetDatabase(mongoConfig.DbName);

			twitterTrends = database.GetCollection<TwitterTrend>(mongoConfig.TwitterTrendCollectionName);
			querySiblings = database.GetCollection<QuerySibling>(mongoConfig.QuerySiblingCollectionName);
		}

		public TwitterTrend FindLatestTrendQuery (string query) 
		{
			return twitterTrends?.AsQueryable()
					.Where(q => q.Query.Equals(query))
					.OrderByDescending(q => q.QueriedAt)
					.FirstOrDefault();
		}

		public Task Create (TwitterTrend twitterTrend)
		{
			return twitterTrends.InsertOneAsync(twitterTrend);
		}

		public Task InsertSiblingRecord (QuerySibling querySibling)
		{
			return querySiblings.InsertOneAsync(querySibling);
		}
	}
}
