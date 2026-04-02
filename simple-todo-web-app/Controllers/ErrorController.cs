using Microsoft.AspNetCore.Mvc;

namespace simple_todo_web_app.Controllers
{
	public class ErrorController : Controller
	{
		[HttpGet("/error/service-unavailable")]
		public IActionResult ServiceUnavailable()
		{
			return View();
		}
	}
}
