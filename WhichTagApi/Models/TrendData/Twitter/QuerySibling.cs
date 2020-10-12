using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace WhichTagApi.Models.TrendData.Twitter
{
	[BsonIgnoreExtraElements]
	public class QuerySibling
	{
		public string Query { get; set; }
		public IEnumerable<string> Siblings { get; set; }
	}
}
