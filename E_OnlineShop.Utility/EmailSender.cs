using Azure.Communication;
using Azure.Communication.Email;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace E_OnlineShop.Utility
{
    public class EmailSender : IEmailSender
    {
        private  string _connectionString= "endpoint=https://emailtest121.unitedstates.communication.azure.com/;accesskey=aPCyBHt0l39eV47UHPeXHjrn4azit9AgVVJbZm7KHcyYDapoaLIPlin8Gd2PuvaGVUYdyvyS3tR4Vnkxrf9kPg==";
        private  string _senderEmailAddress= "DoNotReply@516295dd-f102-4282-add4-21d3e364cb6f.azurecomm.net";


        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new EmailClient(_connectionString);

            try
            {
                var emailSendOperation = await client.SendAsync(
                    Azure.WaitUntil.Completed,
                    _senderEmailAddress, // Use the sender email address provided in the constructor
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
