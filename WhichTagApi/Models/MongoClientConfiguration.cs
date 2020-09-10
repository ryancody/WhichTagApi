using System;
using System.Collections.Generic;
using System.Text;

namespace MongoConfiguration
{
	public class MongoClientConfiguration
	{
		public string Username { get; set; }
		public string Password { get; set; }
		public string DbName { get; set; }
		public bool IsValid =>
			!string.IsNullOrEmpty(Username)
			&& !string.IsNullOrEmpty(Password)
			&& !string.IsNullOrEmpty(DbName);
	}
}
