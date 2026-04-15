using simple_todo_web_app.Common.Constants;
using System.ComponentModel.DataAnnotations;

namespace simple_todo_web_app.Models
{
	public class ResetPasswordViewModel
	{
		[Required]
		public string Email { get; set; } = string.Empty;

		[Required]
		public string Token { get; set; } = string.Empty;

		[Required(ErrorMessage = "パスワードを入力してください。")]
		[MinLength(PasswordConstants.RequiredLength, ErrorMessage = "パスワードは{1}文字以上で入力してください。")]
		[RegularExpression(@"^(?=.*[a-z])(?=.*\d).*$", ErrorMessage = "パスワードは英小文字と数字をそれぞれ1文字以上含めてください。")]
		public string NewPassword { get; set; } = string.Empty;

		[Required(ErrorMessage = "確認用パスワードを入力してください。")]
		[Compare("NewPassword", ErrorMessage = "パスワードが一致しません。")]
		public string ConfirmPassword { get; set; } = string.Empty;
	}
}
