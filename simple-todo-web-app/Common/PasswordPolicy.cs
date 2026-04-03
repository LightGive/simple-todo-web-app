namespace simple_todo_web_app.Common
{
	public static class PasswordPolicy
	{
		public const int RequiredLength = 8;
		public const bool RequireDigit = true;
		public const bool RequireLowercase = true;
		public const bool RequireUppercase = false;
		public const bool RequireNonAlphanumeric = false;
	}
}
