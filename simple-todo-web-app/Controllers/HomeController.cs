using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using simple_todo_web_app.Models;
using simple_todo_web_app.Models.Enums;
using simple_todo_web_app.Models.Parameters;
using System.Diagnostics;

namespace simple_todo_web_app.Controllers
{
	public class HomeController : Controller
	{
		readonly UserManager<ApplicationUser> _userManager;

		public HomeController(UserManager<ApplicationUser> userManager)
		{
			_userManager = userManager;
		}

		[Authorize]
		[HttpGet("/home/initial-setup")]
		public async Task<IActionResult> InitialSetup()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				// [Authorize] が付与されているため通常はここに到達しない
				// 念のためエラーハンドリング
				return RedirectToAction("Login", "Account");
			}

			if (user.IsInit)
			{
				// 初期化済みの場合はホーム画面へ遷移
				return RedirectToAction("Home");
			}

			return View("InitialSetup");
		}

		/// <summary>
		/// 初期設定完了ボタン押下時
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[Authorize]
		[HttpPost("/home/initial-setup")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> InitialSetup(InitialSetupViewModel model)
		{
			// 入力チェック
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				// [Authorize] が付与されているため通常はここに到達しない
				// 念のためエラーハンドリング
				return View(model);
			}

			if (user.IsInit)
			{
				// 初期化済みの場合はホーム画面へ遷移
				return RedirectToAction("Home");
			}

			try
			{
				var taskExercise = new TaskNameCategorySet(TaskCategory.Exercise, new(model.TaskNameExercise));
				var taskStudy = new TaskNameCategorySet(TaskCategory.Study, new(model.TaskNameStudy));
				var taskHousework = new TaskNameCategorySet(TaskCategory.Housework, new(model.TaskNameHousework));
				user.Initialize(model.DisplayName, [taskExercise, taskStudy, taskHousework]);
				await _userManager.UpdateAsync(user);
			}
			catch (ArgumentException)
			{
				ModelState.AddModelError(string.Empty, "入力内容に誤りがあります");
				return View(model);
			}

			return RedirectToAction("Home");
		}

		[Authorize]
		[HttpGet("/home/home")]
		public IActionResult Home()
		{
			return View("Home");
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
