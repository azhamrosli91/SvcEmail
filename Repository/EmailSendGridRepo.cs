using SendGrid.Helpers.Mail;
using SendGrid;
using SvcEmail.Interface;
using SvcEmail.Models;
using libMasterLibaryApi.Helpers;

namespace SvcEmail.Repository
{
    public class EmailSendGridRepo : IEmail
    {
        private readonly IConfiguration _configuration;

        public EmailSendGridRepo(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task<T> configEmail<T>()
        {
            throw new NotImplementedException();
        }

        public async Task<T> sentEmailAsync<T>(Email Email)
        {
            var apiKey = EncryptDecrypt.Decrypt(_configuration["SentGridApiKey:key"]);
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(Email.SenderDetails.Email, Email.SenderDetails.Name);
            var subject = Email.Subject;
            var to = new EmailAddress(Email.EmailTo.Email, Email.EmailTo.Name);

            if (Email.EmailCc != null && Email.EmailCc.Count > 0) {
                foreach (EmailData item in Email.EmailCc)
                {
                    
                }
            }
            if (Email.EmailBcc != null && Email.EmailBcc.Count > 0)
            {
                foreach (EmailData item in Email.EmailBcc)
                {

                }
            }

            var plainTextContent = "";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, Email.Body);
            var response = await client.SendEmailAsync(msg);

            return (T)(object)response;

        }
        
    }
}
