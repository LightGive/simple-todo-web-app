using Microsoft.EntityFrameworkCore;
using simple_todo_web_app.Common.Utilities;
using System.ComponentModel.DataAnnotations;

namespace simple_todo_web_app.Models.Entities
{
	[Index(nameof(UserId), nameof(CompletedAt))]
	[Index(nameof(TaskId), nameof(CompletedAt), IsUnique = true)]
	public class TaskCompletionLog
	{
		/// <summary>
		/// 主キー
		/// </summary>
		[Key]
		public long LogId { get; private set; }

		/// <summary>
		/// 外部キー
		/// </summary>
		[Required]
		public string UserId { get; private set; } = string.Empty;

		/// <summary>
		/// 外部キー
		/// </summary>
		public int TaskId { get; private set; }

		/// <summary>
		/// タスクの完了日時
		/// </summary>
		public DateOnly CompletedAt { get; private set; }

		public TaskCompletionLog(string userId, int taskId)
		{
			UserId = userId;
			TaskId = taskId;
			CompletedAt = DateTimeUtility.UtcToJstDate(DateTime.UtcNow);
		}
	}
}