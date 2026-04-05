using Microsoft.AspNetCore.Mvc;
using simple_todo_web_app.Models;
using System.Diagnostics;

namespace simple_todo_web_app.Controllers
{
	public class HomeController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[HttpGet("/home/initial-setup")]
		public IActionResult InitialSetup()
		{
			return View("InitialSetup");
		}

		[HttpGet("/home/home")]
		public IActionResult Home()
		{
			return View("Index");
		}

		/// <summary>
		/// 初期設定完了ボタン押下時
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult InitialSetup(InitialSetupViewModel model)
		{
			// 入力チェック
			if (!ModelState.IsValid)
			{
				Console.WriteLine("初期設定完了");
				return View();
			}

			return RedirectToAction("Index");
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
