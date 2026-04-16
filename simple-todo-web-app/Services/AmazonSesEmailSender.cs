using Amazon;
using Amazon.SimpleEmailV2;
using Amazon.SimpleEmailV2.Model;

namespace simple_todo_web_app.Services
{
	public class AmazonSesEmailSender : IEmailSender
	{
		readonly string _fromEmail;
		readonly AmazonSimpleEmailServiceV2Client _client;

		public AmazonSesEmailSender(IConfiguration configuration)
		{
			var accessKey = configuration["Aws:AccessKeyId"] ??
				throw new InvalidOperationException("Aws:AccessKeyIdが設定されていません。");
			var secretKey = configuration["Aws:SecretAccessKey"] ??
				throw new InvalidOperationException("Aws:SecretAccessKeyが設定されていません。");
			var region = configuration["Aws:Region"] ??
				throw new InvalidOperationException("Aws:Regionが設定されていません。");
			_fromEmail = configuration["Aws:FromEmail"] ??
				throw new InvalidOperationException("Aws:FromEmailが設定されていません。");

			_client = new AmazonSimpleEmailServiceV2Client(
				accessKey,
				secretKey,
				RegionEndpoint.GetBySystemName(region));
		}

		public async Task SendAsync(string toEmail, string subject, string body)
		{
			var request = new SendEmailRequest
			{
				FromEmailAddress = _fromEmail,
				Destination = new Destination
				{
					ToAddresses = [toEmail]
				},
				Content = new EmailContent
				{
					Simple = new Message
					{
						Subject = new Content { Data = subject },
						Body = new Body
						{
							Text = new Content { Data = body }
						}
					}
				}
			};

			await _client.SendEmailAsync(request);
		}
	}
}