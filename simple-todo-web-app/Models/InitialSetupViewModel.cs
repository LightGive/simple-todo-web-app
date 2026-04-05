using System.ComponentModel.DataAnnotations;

namespace simple_todo_web_app.Models
{
	public class InitialSetupViewModel
	{
		[Required(ErrorMessage = "勇者の名前を入力してください。")]
		public string DisplayName { get; set; } = string.Empty;

		[Required(ErrorMessage = "運動タスク名を入力してください")]
		public string TaskNameExercise { get; set; } = string.Empty;

		[Required(ErrorMessage = "勉強タスク名を入力してください。")]
		public string TaskNameStudy { get; set; } = string.Empty;

		[Required(ErrorMessage = "家事タスク名を入力してください。")]
		public string TaskNameHousework { get; set; } = string.Empty;
	}
}