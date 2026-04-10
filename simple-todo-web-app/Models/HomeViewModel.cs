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
		[MaxLength(CharacterConstants.NameMaxLength)]
		public string DisplayName { get; set; } = string.Empty;
		public int Level { get; set; } = 0;
		public List<ToDoTask> ToDoTaskList { get; set; } = new();
		public CharacterStats? CharacterStats { get; set; } = null;
		public UnallocatedPoints? UnallocatedPoints { get; set; } = null;
	}
}
