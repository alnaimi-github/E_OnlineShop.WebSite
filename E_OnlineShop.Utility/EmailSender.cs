using Azure.Communication.Email;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;

namespace E_OnlineShop.Utility
{
	public class EmailSender : IEmailSender
	{
		private readonly IConfiguration _configuration;

		public EmailSender(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		private async Task<string> GetConnectionString()
		{
			return await Task.FromResult(_configuration["EmailSettings:ConnectionString"]!);
		}

		private async Task<string> GetSenderEmailAddress()
		{
			return await Task.FromResult(_configuration["EmailSettings:SenderEmailAddress"]!);
		}

		public async Task SendEmailAsync(string email, string subject, string htmlMessage)
		{
			var client = new EmailClient(await GetConnectionString());

			try
			{
				var emailSendOperation = await client.SendAsync(
					Azure.WaitUntil.Completed,
					await GetSenderEmailAddress(), // Use the sender email address provided in the constructor
					email, // Use the recipient email address passed as a parameter
					subject,
					htmlMessage
				);

				var sendResult = emailSendOperation.Value;
				Console.WriteLine(sendResult.Status.ToString());
			}
			catch (Exception ex)
			{
				// Handle the exception appropriately
				Console.WriteLine($"Failed to send email: {ex.Message}");
			}
		}
	}
}
