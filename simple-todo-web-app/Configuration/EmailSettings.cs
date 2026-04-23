namespace simple_todo_web_app.Configuration
{
	/// <summary>
	/// appsettings.jsonのEmailSettingsセクションに対応するクラス
	/// </summary>
	public class EmailSettings
	{
		public string SmtpServer { get; set; } = "";
		public int Port { get; set; }
		public string SenderName { get; set; } = "";
		public string SenderEmail { get; set; } = "";
		public string Password { get; set; } = "";
	}
}
