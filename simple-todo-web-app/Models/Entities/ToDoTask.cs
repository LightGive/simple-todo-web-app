using simple_todo_web_app.Common.Constants;
using simple_todo_web_app.Common.Utilities;
using simple_todo_web_app.Models.Enums;
using simple_todo_web_app.Models.Parameters;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace simple_todo_web_app.Models.Entities
{
	/// <summary>
	/// タスク
	/// </summary>
	public class ToDoTask
	{
		/// <summary>
		/// 主キー
		/// </summary>
		[Key]
		public int TaskId { get; private set; }

		/// <summary>
		/// 外部キー
		/// </summary>
		[Required]
		public string UserId { get; private set; }

		/// <summary>
		/// カテゴリ
		/// </summary>
		public TaskCategory Category { get; private set; }

		/// <summary>
		/// タスク名
		/// </summary>
		[MaxLength(TaskConstants.NameMaxLength)]
		[Required]
		public string TaskName { get; private set; }

		/// <summary>
		/// 最終完了日（JST）
		/// </summary>
		public DateOnly? LastCompletedDate { get; private set; }

		/// <summary>
		/// 論理削除フラグ
		/// </summary>
		public bool IsDeleted { get; private set; }

		/// <summary>
		/// 本日のタスクが完了しているか（JST）
		/// </summary>
		[NotMapped]
		public bool IsCompletedToday => LastCompletedDate == DateTimeUtility.UtcToJstDate(DateTime.UtcNow);

		/// <summary>
		/// EF Core用のコンストラクタ（直接使用しないこと）
		/// </summary>
		private ToDoTask() { }

		public ToDoTask(string userId, TaskNameCategorySet taskNameCategorySet)
		{
			UserId = userId;
			Category = taskNameCategorySet.Category;
			TaskName = taskNameCategorySet.TaskName.TaskNameValue;
			LastCompletedDate = null;
		}

		/// <summary>
		/// タスクを完了させる
		/// </summary>
		public void CompleteTask()
		{
			LastCompletedDate = DateTimeUtility.UtcToJstDate(DateTime.UtcNow);
		}

		/// <summary>
		/// タスクを論理削除する
		/// </summary>
		public void DeleteTask()
		{
			IsDeleted = true;
		}
	}
}