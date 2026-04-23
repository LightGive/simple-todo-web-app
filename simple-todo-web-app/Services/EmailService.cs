using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using simple_todo_web_app.Configuration;

namespace simple_todo_web_app.Services
{
	/// <summary>
	/// メール関連の機能を提供するサービスクラス
	/// </summary>
	public class EmailService
	{
		private readonly EmailSettings _settings;

		public EmailService(IOptions<EmailSettings> options)
		{
			_settings = options.Value;
		}

		public async Task SendAsync(
			string to,
			string subject,
			string body)
		{
			var message = new MimeMessage();

			message.From.Add(
				new MailboxAddress(
					_settings.SenderName,
					_settings.SenderEmail));
			message.To.Add(
				MailboxAddress.Parse(to));
			message.Subject = subject;
			message.Body = new TextPart("plain")
			{
				Text = body
			};

#if DEBUG && false
			Console.WriteLine($"[DEBUG] Sending email to: {to}");
			Console.WriteLine($"[DEBUG] Smtp Server: {_settings.SmtpServer}, Port: {_settings.Port}");
			Console.WriteLine($"[DEBUG] Sender Email: {_settings.SenderEmail}");
			Console.WriteLine($"[DEBUG] Password: {_settings.Password}");
#endif

			using var client = new SmtpClient();
			await client.ConnectAsync(
				_settings.SmtpServer,
				_settings.Port,
				MailKit.Security.SecureSocketOptions.StartTls);
			await client.AuthenticateAsync(
				_settings.SenderEmail,
				_settings.Password);
			await client.SendAsync(message);
			await client.DisconnectAsync(true);
		}
	}
}