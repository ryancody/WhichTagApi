using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WhichTagApi.Models.TrendData.Twitter
{
	public class QueryRequestBody
	{
		public IEnumerable<string> Siblings { get; set; }
	}
}
