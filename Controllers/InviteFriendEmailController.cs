using libMasterLibaryApi.Interface;
using libMasterObject;
using Microsoft.AspNetCore.Mvc;
using SendGrid;
using SvcEmail.Interface;
using SvcEmail.Models;
using System.Security.Claims;
using System.Text.Json;

namespace SvcEmail.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InviteFriendEmailController : ControllerBase
    {

        private readonly IEmail _email;
        private readonly ILogger<VerifyAccEmailController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IJwtToken _jwtToken;
        private readonly IApiURL _apiURL;

        public InviteFriendEmailController(ILogger<VerifyAccEmailController> logger, IConfiguration configuration, IEmail email
                                    , IWebHostEnvironment webHostEnvironment, IJwtToken jwtToken, IApiURL apiURL)
        {
            _logger = logger;
            _configuration = configuration;
            _email = email;
            _webHostEnvironment = webHostEnvironment;
            _jwtToken = jwtToken;
            _apiURL = apiURL;
        }

        [HttpGet(Name = "InviteFriendEmail")]
        public async Task<IActionResult> Get([FromBody] EmailValidate value)
        {

            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] EmailInvitationFriend value) 
        {

            Email model = new Email();
            model.SenderDetails = new EmailData { Email = _configuration["EmailSetting:emailFrom"], Name = _configuration["EmailSetting:nameFrom"] };
            model.EmailTo = new EmailData { Email = value.UserEmail, Name = value.Name };
            model.Subject = "[Epsilon Sigma] " + value.InvitationName + " is waiting for you to join them";

            string urlNavigate = await _apiURL.GetApiURL("InviteFriend");
            string jsonFormatter = JsonSerializer.Serialize<EmailInvitationFriend>(value);
            Claim[] claims = new[]
            {
                new Claim("token", jsonFormatter)
            };

            string token = _jwtToken.GenerateJWTToken(claims, 1);

            urlNavigate += "?token=" + token;


            string webRootPath = _webHostEnvironment.WebRootPath;
            string contentRootPath = _webHostEnvironment.ContentRootPath;
            string path = "";
            path = Path.Combine(webRootPath, "Templates", "InviteFriend.html");

            model.Body = System.IO.File.ReadAllText(path);
            model.Body = model.Body.Replace("[VerifyURLData]", urlNavigate);
            model.Body = model.Body.Replace("[InvitationName]", value.InvitationName);
            model.Body = model.Body.Replace("[CompanyName]", value.CompanyName);

            var respone = await _email.sentEmailAsync<Response>(model);

            if (respone.IsSuccessStatusCode)
            {
                return Ok(respone);
            }
            else {
                return BadRequest(respone);
            }
        }
    }
}