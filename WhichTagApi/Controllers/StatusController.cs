using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Twitter;
using Twitter.Models;

namespace WhichTagApi.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class StatusController : ControllerBase
	{
		private readonly ILogger<TwitterController> logger;

		public StatusController (ILogger<TwitterController> logger)
		{
			this.logger = logger;
		}

		[HttpGet]
		public IActionResult Get ()
		{
			return Ok();
		}
	}
}
