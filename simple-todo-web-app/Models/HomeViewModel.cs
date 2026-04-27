using simple_todo_web_app.Common.Constants;
using simple_todo_web_app.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace simple_todo_web_app.Models
{
	/// <summary>
	/// ホーム画面に使用するViewModel
	/// </summary>
	public class HomeViewModel
	{
		/// <summary>
		/// 1日分のタスクの履歴を表すクラス
		/// </summary>
		public class TaskHistoryEntry
		{
			public DateOnly CompleteDate { get; set; } = new();
			public List<int> CategoryList { get; set; } = new();
		}

		[MaxLength(CharacterConstants.NameMaxLength)]
		public string DisplayName { get; set; } = string.Empty;
		public int Level { get; set; } = 0;
		public List<ToDoTask> ToDoTaskList { get; set; } = new();
		public CharacterStats? CharacterStats { get; set; } = null;
		public UnallocatedPoints? UnallocatedPoints { get; set; } = null;
		public List<TaskHistoryEntry> TaskHistoryList { get; set; } = new();
		public DateOnly? Today { get; set; } = null;
		public bool IsValid => CharacterStats != null && UnallocatedPoints != null;
	}
}
