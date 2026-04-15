using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using simple_todo_web_app.Models;

namespace simple_todo_web_app.Controllers
{
	public class AccountController : Controller
	{
		readonly UserManager<ApplicationUser> _userManager;
		readonly SignInManager<ApplicationUser> _signInManager;

		public AccountController(
			UserManager<ApplicationUser> userManager,
			SignInManager<ApplicationUser> signInManager)
		{
			_userManager = userManager;
			_signInManager = signInManager;
		}

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

		/// <summary>
		/// ログイン
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost("/account/login")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginViewModel model)
		{
			// 入力チェック
			if (!ModelState.IsValid)
			{
#if DEBUG
				foreach (var state in ModelState)
				{
					var key = state.Key;
					foreach (var error in state.Value.Errors)
					{
						Console.WriteLine($"ValidationError: {key}: {error.ErrorMessage}");
					}
				}
#endif
				return View(model);
			}

			var result = await _signInManager.PasswordSignInAsync(
				model.Email,
				model.Password,
				true,
				false);

#if DEBUG
			Console.WriteLine($"Succeeded: {result.Succeeded}");
			Console.WriteLine($"IsLockedOut: {result.IsLockedOut}");
			Console.WriteLine($"IsNotAllowed: {result.IsNotAllowed}");
			Console.WriteLine($"RequiresTwoFactor: {result.RequiresTwoFactor}");
#endif

			if (!result.Succeeded)
			{
				ModelState.AddModelError(string.Empty, "メールアドレスまたはパスワードが正しくありません。");
				return View(model);
			}

			var user = await _userManager.FindByEmailAsync(model.Email);
			if (user == null)
			{
				// 本来あり得ないがサインアウトしてエラー表示
				await _signInManager.SignOutAsync();
				ModelState.AddModelError(string.Empty, "ログイン処理中にエラーが発生しました。しばらく経ってから再度お試しください。");
				return View(model);
			}

			if (user.IsInit)
			{
#if DEBUG
				Console.WriteLine("サインイン成功、初期化済のためホーム画面へ");
#endif
				return RedirectToAction("Home", "Home");
			}
			else
			{
#if DEBUG
				Console.WriteLine("サインイン成功、未初期化のため初期設定画面へ");
#endif
				return RedirectToAction("InitialSetup", "Home");
			}
		}

		/// <summary>
		/// ユーザー登録
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost("/account/register")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Register(RegisterViewModel model)
		{
			// 入力チェック
			if (!ModelState.IsValid)
			{
				foreach (var state in ModelState)
				{
					var key = state.Key;
					foreach (var error in state.Value.Errors)
					{
#if DEBUG
						Console.WriteLine($"ValidationError: {key}: {error.ErrorMessage}");
#endif
					}
				}

				return View(model);
			}

			// ユーザー登録
			var user = new ApplicationUser
			{
				UserName = model.Email,
				Email = model.Email,
			};
			var result = await _userManager.CreateAsync(user, model.Password);

			if (!result.Succeeded)
			{
				var errorMessage = string.Empty;
				foreach (var error in result.Errors)
				{
					switch (error.Code)
					{
						case "DuplicateEmail":
						case "DuplicateUserName":
							errorMessage = "このメールアドレスは既に使用されています。";
							break;
						case "InvalidUserName":
							errorMessage = "ユーザー名に使用できない文字が含まれています。";
							break;
						default:
							errorMessage = "ユーザー登録中にエラーが発生しました。\nしばらく経ってから再度お試しください。";
							break;
					}

					ModelState.AddModelError(string.Empty, errorMessage);
				}
				return View(model);
			}

			// 登録完了画面にリダイレクト
			return RedirectToAction("RegisterConfirmation");
		}

		[HttpGet("/account/forgot-password")]
		public IActionResult ForgotPassword()
		{
			return View("ForgotPassword");
		}

		/// <summary>
		/// パスワード再設定メール送信
		/// </summary>
		[HttpPost("/account/forgot-password")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			// 登録済みの場合のみトークンを生成する
			// ユーザー列挙攻撃対策: 未登録メールでも完了画面へ遷移する
			var user = await _userManager.FindByEmailAsync(model.Email);
			if (user != null)
			{
				var token = await _userManager.GeneratePasswordResetTokenAsync(user);
				// TODO: メール送信処理（未実装）
				// 再設定URLは /account/reset-password?email={email}&token={token} 形式
#if DEBUG
				var encodedToken = Uri.EscapeDataString(token);
				Console.WriteLine($"[DEBUG] パスワード再設定URL: /account/reset-password?email={user.Email}&token={encodedToken}");
#endif
			}

			return RedirectToAction("ForgotPasswordConfirmation");
		}
		[HttpPost("/account/logout")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Logout()
		{
#if DEBUG
			Console.WriteLine("ログアウト処理開始");
#endif
			await _signInManager.SignOutAsync();
			return RedirectToAction("Login");
		}
	}
}
