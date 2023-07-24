using SendGrid.Helpers.Mail;
using SendGrid;
using SvcEmail.Interface;
using SvcEmail.Models;
using libMasterLibaryApi.Helpers;
using libMasterLibaryApi.Interface;
using libMasterObject;
using libMasterLibaryApi.Models;
using Newtonsoft.Json.Linq;

namespace SvcEmail.Repository
{
    public class EmailSendGridRepo : IEmail
    {
        private readonly IConfiguration _configuration;
        private readonly IDbService _dbService;

        public EmailSendGridRepo(IConfiguration configuration, IDbService dbService)
        {
            _configuration = configuration;
            _dbService = dbService;
        }

        public Task<T> configEmail<T>()
        {
            throw new NotImplementedException();
        }
        private async Task<string> GetApiKey() {
            try
            {
                string ApiKeyString = "";

                RepoData repoData = new RepoData()
                {
                    //Query = @"insert into com_user_linking (companyid,userid) values (@companyid,@userid)",
                    Query = @"select * from api_keys where type=@type",
                    Param = new
                    {
                        type = "SentGridApiKey"
                    }
                };

                ApiKey apiKey = await _dbService.GetAsync<ApiKey>(repoData);

                if (apiKey == null) return "";

                ApiKeyString=EncryptDecrypt.Decrypt(apiKey.Key);


                return ApiKeyString;
            }
            catch (Exception)
            {
                return "";
            }
        }
        public async Task<T> sentEmailAsync<T>(Email Email)
        {
            var apiKey = await GetApiKey();
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
