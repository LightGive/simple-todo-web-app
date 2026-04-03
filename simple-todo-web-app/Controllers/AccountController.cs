using Microsoft.AspNetCore.Mvc;
using simple_todo_web_app.Models;

namespace simple_todo_web_app.Controllers
{
	public class AccountController : Controller
	{
		[HttpGet("/account/login")]
		public IActionResult Login()
		{
			return View("Login");
		}

		[HttpGet("/account/register")]
		public IActionResult Register()
		{
			return View("Register");
		}

		[HttpGet("/account/register-confirmation")]
		public IActionResult RegisterConfirmation()
		{
			return View("RegisterConfirmation");
		}

		[HttpGet("/account/forgot-password-confirmation")]
		public IActionResult ForgotPasswordConfirmation()
		{
			return View("ForgotPasswordConfirmation");
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Login(LoginViewModel model)
		{
			// 入力チェック
			if (!ModelState.IsValid)
			{
				var displayMessage = "";
				foreach (var state in ModelState)
				{
					var key = state.Key;
					foreach (var error in state.Value.Errors)
					{
						displayMessage += $"{error.ErrorMessage}";
						Console.WriteLine($"ValidationError: {key}: {error.ErrorMessage}");
					}
				}

				return View(model);
			}

			Console.WriteLine($"Email: {model.Email}, Password: {model.Password}");

			// ASP.NetIdentityを使用してユーザー認証
			// 認証失敗時は以下でエラーを追加する
			// ModelState.AddModelError(string.Empty, "メールアドレスまたはパスワードが正しくありません。");

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Register(RegisterViewModel model)
		{
			// 入力チェック
			if (!ModelState.IsValid)
			{
				var displayMessage = "";
				foreach (var state in ModelState)
				{
					var key = state.Key;
					foreach (var error in state.Value.Errors)
					{
						displayMessage += $"{error.ErrorMessage}";
						Console.WriteLine($"ValidationError: {key}: {error.ErrorMessage}");
					}
				}

				return View(model);
			}

			// ユーザー登録

			// 登録完了画面にリダイレクト
			return RedirectToAction("RegisterConfirmation");
		}
	}
}
