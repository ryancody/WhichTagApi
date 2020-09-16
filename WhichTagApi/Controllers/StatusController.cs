using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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
