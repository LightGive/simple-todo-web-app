using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using simple_todo_web_app.Data;
using simple_todo_web_app.Models;
using simple_todo_web_app.Models.Entities;
using simple_todo_web_app.Models.Enums;
using simple_todo_web_app.Models.Parameters;
using System.Diagnostics;

namespace simple_todo_web_app.Controllers
{
	public class HomeController : Controller
	{
		readonly UserManager<ApplicationUser> _userManager;
		readonly ApplicationDbContext _context;

		public HomeController(
			UserManager<ApplicationUser> userManager,
			ApplicationDbContext context)
		{
			_userManager = userManager;
			_context = context;
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
		public async Task<IActionResult> Home()
		{
			var userId = _userManager.GetUserId(User);
			if (userId == null)
			{
				return RedirectToAction("Login", "Account");
			}

			// Include でナビゲーションプロパティを一括取得
			var user = await _context.Users
				.Include(u => u.TaskList)
				.Include(u => u.CharacterStats)
				.Include(u => u.UnallocatedPoints)
				.FirstOrDefaultAsync(u => u.Id == userId);

			if (user == null)
			{
				return RedirectToAction("Login", "Account");
			}

			// 初期設定未完了の場合は初期設定画面へ
			if (!user.IsInit)
			{
				return RedirectToAction("InitialSetup");
			}

			// レベル = タスク完了ログの総件数
			var level = await _context.TaskCompletionLogs
				.CountAsync(l => l.UserId == userId);

			var model = new HomeViewModel
			{
				DisplayName = user.DisplayName,
				Level = level,
				ToDoTaskList = user.TaskList,
				CharacterStats = user.CharacterStats,
				UnallocatedPoints = user.UnallocatedPoints,
			};

			return View("Home", model);
		}

		[Authorize]
		[HttpPost("/api/tasks/{taskId}/complete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CompleteTask(int taskId)
		{
#if DEBUG
			Console.WriteLine($"タスク完了　タスクID:{taskId}");
#endif
			// 保存処理
			var userId = _userManager.GetUserId(User);
			if (userId == null)
			{
				return RedirectToAction("Login", "Account");
			}


			ToDoTask? todoTask = null;
			UnallocatedPoints? point = null;
			try
			{
				// DBアクセス
				todoTask = await _context.Tasks
					.Where(t => t.TaskId == taskId && t.UserId == userId)
					.FirstOrDefaultAsync();
				point = await _context.UnallocatedPoints
					.Where(x => x.UserId == userId)
					.SingleAsync();
			}
			catch(InvalidOperationException)
			{
				// 指定のUserIdのUnallocatedPointsが存在しない場合
				return RedirectToAction("Home");
			}

			if (todoTask == null)
			{
				// UserId, TaskId に該当するタスクが存在しない
				return RedirectToAction("Home");

			}
			if (todoTask.IsCompletedToday)
			{
				// 本日のタスクが既に完了している
				return RedirectToAction("Home");
			}

#if DEBUG
			Console.WriteLine("タスク完了処理の開始");
#endif
			// タスクの完了処理
			todoTask.CompleteTask();

			// ステータスポイントを付与
			switch (todoTask.Category)
			{
				case TaskCategory.Exercise: point.AddPoints(1, 0, 0); break;
				case TaskCategory.Study: point.AddPoints(0, 1, 0); break;
				case TaskCategory.Housework: point.AddPoints(0, 0, 1); break;
				default:
					throw new UnreachableException($"未定義のカテゴリ: {todoTask.Category}");
			}

#if DEBUG
			Console.WriteLine("タスク完了ログの保存");
#endif
			// タスク完了ログの保存
			var log = new TaskCompletionLog(userId, taskId);
			_context.TaskCompletionLogs.Add(log);

			// 保存
			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateException)
			{
				// 保存エラー時はホームに戻す
				return RedirectToAction("Home");
			}

			return RedirectToAction("Home");
		}

		[Authorize]
		[HttpPost("/api/character/allocate")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Allocate([FromBody] StatAllocation status)
		{
			if (!ModelState.IsValid)
			{
				// ポイントが負の値の場合
				return BadRequest();
			}

			// 保存処理
			var userId = _userManager.GetUserId(User);
			if (userId == null)
			{
				return BadRequest();
			}

			UnallocatedPoints? unallocatedPoints = null;
			CharacterStats? characterStats = null;
			try
			{
				// DBアクセス
				unallocatedPoints = await _context.UnallocatedPoints
					.Where(x => x.UserId == userId)
					.SingleAsync();
				characterStats = await _context.CharacterStats
					.Where(x => x.UserId == userId)
					.SingleAsync();
			}
			catch (InvalidOperationException)
			{
				// 指定のUserIdのUnallocatedPointsまたは CharacterStatsが存在しない場合
				return BadRequest();
			}

			try
			{
				// 未割当のポイントを使用
				unallocatedPoints.UsePoints(status.ExercisePointsCost, status.StudyPointsCost, status.HouseworkPointsCost);
			}
			catch (InvalidOperationException)
			{
				// ポイントが不足している場合はエラー
				return BadRequest();
			}

			characterStats.AddStatus(status);

			// 保存
			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateException)
			{
				// 保存エラー時はホームに戻す
				return StatusCode(500);
			}

			return Ok();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
