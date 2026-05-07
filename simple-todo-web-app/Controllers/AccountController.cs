using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using simple_todo_web_app.Common.Constants;
using simple_todo_web_app.Models;
using simple_todo_web_app.Services;

namespace simple_todo_web_app.Controllers
{
	public class AccountController : Controller
	{
		readonly UserManager<ApplicationUser> _userManager;
		readonly SignInManager<ApplicationUser> _signInManager;
		readonly EmailService _emailService;

		public AccountController(
			UserManager<ApplicationUser> userManager,
			SignInManager<ApplicationUser> signInManager,
			EmailService emailServ)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_emailService = emailServ;
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

			var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
			var encodedToken = Uri.EscapeDataString(token);
			var resetUrl = $"{Request.Scheme}://{Request.Host}/account/confirm-email?userId={user.Id}&token={encodedToken}";

			// メール送信
			await _emailService.SendAsync(
				user.Email,
				EmailConstants.RegisterConfirmation.Subject,
				string.Format(EmailConstants.RegisterConfirmation.BodyTemplate, resetUrl));

			// 登録完了画面にリダイレクト
			return RedirectToAction("RegisterConfirmation");
		}

		/// <summary>
		/// メールアドレス認証
		/// </summary>
		[HttpGet("/account/confirm-email")]
		public async Task<IActionResult> ConfirmEmail(string? userId, string? token)
		{
			if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
			{
				TempData["ErrorMessage"] = "認証リンクが無効です。再度登録をお試しください。";
				return RedirectToAction("Login");
			}

			var user = await _userManager.FindByIdAsync(userId);
			if (user == null)
			{
				TempData["ErrorMessage"] = "認証リンクが無効です。再度登録をお試しください。";
				return RedirectToAction("Login");
			}

			var result = await _userManager.ConfirmEmailAsync(user, token);
			if (!result.Succeeded)
			{
				TempData["ErrorMessage"] = "認証リンクが無効または有効期限切れです。再度登録をお試しください。";
				return RedirectToAction("Login");
			}

			TempData["SuccessMessage"] = "メールアドレスの認証が完了しました。ログインしてください。";
			return RedirectToAction("Login");
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
			if (user != null && !string.IsNullOrEmpty(user.Email))
			{
				var token = await _userManager.GeneratePasswordResetTokenAsync(user);
				var encodedToken = Uri.EscapeDataString(token);
				var resetUrl = $"{Request.Scheme}://{Request.Host}/account/reset-password?email={user.Email}&token={encodedToken}";

				// メール送信
				await _emailService.SendAsync(
					user.Email,
					EmailConstants.PasswordReset.Subject,
					string.Format(EmailConstants.PasswordReset.BodyTemplate, resetUrl));
#if DEBUG
				Console.WriteLine($"[DEBUG] パスワード再設定URL: /account/reset-password?email={user.Email}&token={encodedToken}");
#endif
			}

			return RedirectToAction("ForgotPasswordConfirmation");
		}

		/// <summary>
		/// パスワード再設定画面表示
		/// </summary>
		[HttpGet("/account/reset-password")]
		public IActionResult ResetPassword(string? email, string? token)
		{
			if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
			{
				return RedirectToAction("Login");
			}

			var model = new ResetPasswordViewModel
			{
				Email = email,
				Token = token,
			};
			return View("ResetPassword", model);
		}

		/// <summary>
		/// パスワード再設定
		/// </summary>
		[HttpPost("/account/reset-password")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var user = await _userManager.FindByEmailAsync(model.Email);
			if (user == null)
			{
				// セキュリティ上、ユーザーの存在有無を明かさずログイン画面へ遷移
				return RedirectToAction("Login");
			}

			var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
			if (!result.Succeeded)
			{
				foreach (var error in result.Errors)
				{
					var message = error.Code == "InvalidToken"
						? "再設定リンクの有効期限が切れています。再度リクエストしてください。"
						: "再設定処理中にエラーが発生しました。しばらく経ってから再度お試しください。";
					ModelState.AddModelError(string.Empty, message);
				}
				return View(model);
			}

			return RedirectToAction("Login");
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
