namespace simple_todo_web_app.Common.Constants
{
	public static class EmailConstants
	{
		public static class RegisterConfirmation
		{
			public const string Subject = "【習慣化の勇者】メールアドレス確認のご案内";
			public const string BodyTemplate = "以下のURLからメールアドレスを確認してください。\n\n{0}\n\nこのURLの有効期限は1日です。";
		}

		public static class PasswordReset
		{
			public const string Subject = "【習慣化の勇者】パスワード再設定のご案内";
			public const string BodyTemplate = "以下のURLからパスワードを再設定してください。\n\n{0}\n\nこのURLの有効期限は1日です。";
		}
	}
}
