using System.ComponentModel.DataAnnotations;

namespace simple_todo_web_app.Models
{
	public class ForgotPasswordViewModel
	{
		[Required(ErrorMessage = "メールアドレスを入力してください。")]
		[EmailAddress(ErrorMessage = "有効なメールアドレスを入力してください。")]
		public string Email { get; set; } = string.Empty;
	}
}
