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
		[HttpPost]
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
				// TODO: ホーム画面（SCR008）の実装後にリダイレクト先を変更
				return View(model);
			}
			else
			{
#if DEBUG
				Console.WriteLine("サインイン成功、未初期化のため初期設定画面へ");
#endif
				// TODO: 初期設定画面（SCR007）の実装後にリダイレクト先を変更
				return View(model);
			}

		}

		/// <summary>
		/// ユーザー登録
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
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
				foreach (var error in result.Errors)
				{
					
					ModelState.AddModelError(string.Empty, error.Description);
				}
				return View(model);
			}

			// 登録完了画面にリダイレクト
			return RedirectToAction("RegisterConfirmation");
		}
	}
}
