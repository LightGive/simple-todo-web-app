using simple_todo_web_app.Common.Constants;
using simple_todo_web_app.Common.Utilities;
using simple_todo_web_app.Models.Enums;
using System.ComponentModel.DataAnnotations;

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
		/// 最終完了日
		/// </summary>
		public DateOnly? LastCompletedDate { get; private set; }

		/// <summary>
		/// 論理削除フラグ
		/// </summary>
		public bool IsDeleted { get; private set; }

		public ToDoTask(string userId, TaskCategory category)
		{
			UserId = userId;
			Category = category;
			TaskName = string.Empty;
			LastCompletedDate = null;
		}

		/// <summary>
		/// タスク名を設定する
		/// </summary>
		/// <param name="taskName"></param>
		/// <exception cref="ArgumentException"></exception>
		public void SetTaskName(string taskName)
		{
			if (string.IsNullOrWhiteSpace(taskName))
			{
				throw new ArgumentException("タスク名は必須です。", nameof(taskName));
			}
			if (taskName.Length > TaskConstants.NameMaxLength)
			{
				throw new ArgumentException($"タスク名は{TaskConstants.NameMaxLength}文字以内で入力してください。", nameof(taskName));
			}
			TaskName = taskName;
		}

		public void CompleteTask()
		{
			LastCompletedDate = DateTimeUtility.UtcToJstDate(DateTime.UtcNow);
		}

		public void DeleteTask()
		{
			IsDeleted = true;
		}
	}
}