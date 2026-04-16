namespace simple_todo_web_app.Services
{
	public interface IEmailSender
	{
		Task SendAsync(string toEmail, string subject, string body);
	}
}
