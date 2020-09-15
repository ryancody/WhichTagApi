using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoConfiguration;
using MongoDB.Driver;
using Twitter;


namespace WhichTagApi
{
	public class Startup
	{
		private readonly string allowAllPolicy = "AllowAll";

		public Startup (IWebHostEnvironment environment)
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(environment.ContentRootPath)
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: false)
				.AddJsonFile("appsettings.local.json", optional: true)
				.AddEnvironmentVariables();

			Configuration = builder.Build();
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices (IServiceCollection services)
		{
			services.AddControllers();
			services.AddCors(c =>
			{
				c.AddPolicy(allowAllPolicy, options => options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
			});

			var twitterConfig = Configuration.GetSection("Twitter").Get<TwitterClientConfiguration>();
			twitterConfig.Key = Environment.GetEnvironmentVariable("TWITTER_KEY");
			twitterConfig.Secret = Environment.GetEnvironmentVariable("TWITTER_SECRET");
			services.AddTwitterClient(twitterConfig);

			var mongoConfig = Configuration.GetSection("MongoDB").Get<MongoClientConfiguration>();
			mongoConfig.Username = Environment.GetEnvironmentVariable("MONGO_USERNAME");
			mongoConfig.Password = Environment.GetEnvironmentVariable("MONGO_PASSWORD");
			mongoConfig.DbName = Environment.GetEnvironmentVariable("MONGO_DATABASENAME");
			var mongoClient = new MongoClient($"mongodb+srv://{mongoConfig.Username}:{mongoConfig.Password}@cluster0.6lvjj.mongodb.net/{mongoConfig.DbName}?retryWrites=true&w=majority");
			services.AddSingleton(mongoClient);
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure (IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseAuthorization();

			app.UseCors(allowAllPolicy);

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
