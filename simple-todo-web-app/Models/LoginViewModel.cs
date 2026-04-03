using System.ComponentModel.DataAnnotations;

namespace simple_todo_web_app.Models
{
	public class LoginViewModel
	{
		[Required(ErrorMessage = "メールアドレスを入力してください。")]
		[EmailAddress(ErrorMessage = "メールアドレスの形式が正しくありません。")]
		public string Email { get; set; } = string.Empty;

		[Required(ErrorMessage = "パスワードを入力してください。")]
		public string Password { get; set; } = string.Empty;
	}
}
