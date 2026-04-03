using simple_todo_web_app.Attributes;
using simple_todo_web_app.Common;
using System.ComponentModel.DataAnnotations;

namespace simple_todo_web_app.Models
{
	public class RegisterViewModel
	{
		[Required(ErrorMessage = "メールアドレスを入力してください。")]
		[EmailAddress(ErrorMessage = "メールアドレスの形式が正しくありません。")]
		public string Email { get; set; } = string.Empty;

		[Required(ErrorMessage = "パスワードを入力してください。")]
		[MinLength(PasswordPolicy.RequiredLength, ErrorMessage = "パスワードの長さは{1}文字以上にして下さい")]
		[RegularExpression(@"^(?=.*[a-z])(?=.*\d).*$",ErrorMessage = "パスワードは英小文字と数字をそれぞれ1文字以上含めてください")]
		public string Password { get; set; } = string.Empty;

		[Required(ErrorMessage = "パスワード（確認）を入力してください。")]
		[Compare("Password", ErrorMessage = "パスワードと確認用パスワードが一致しません。")]
		public string ConfirmPassword { get; set; } = string.Empty;
	}
}
