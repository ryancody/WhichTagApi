using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoConfiguration;
using WhichTag.TwitterClient.Models;
using WhichTag.TwitterClient;
using Microsoft.Extensions.Options;
using WhichTagApi.Services;
using WhichTagApi.Mappers;

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

			services.Configure<MongoClientConfiguration>(a => 
			{
				a.ClusterUrl = mongoConfig.ClusterUrl;
				a.DbName = mongoConfig.DbName;
				a.TwitterTrendCollectionName = mongoConfig.TwitterTrendCollectionName;
				a.QuerySiblingCollectionName = mongoConfig.QuerySiblingCollectionName;
				a.Username = Environment.GetEnvironmentVariable("MONGO_USERNAME");
				a.Password = Environment.GetEnvironmentVariable("MONGO_PASSWORD");
				a.DbName = Environment.GetEnvironmentVariable("MONGO_DATABASENAME");
			});

			services.AddSingleton(sp => sp.GetRequiredService<IOptions<MongoClientConfiguration>>().Value);

			services.AddSingleton<MongoService>();

			services.AddControllers().AddNewtonsoftJson();

			var scoreCalculator = new ScoreCalculator();
			services.AddSingleton(scoreCalculator);

			var twitterDataMapper = new TwitterDataMapper(scoreCalculator);
			services.AddSingleton(twitterDataMapper);
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
