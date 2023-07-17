using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SendGrid;
using libMasterLibaryApi.Enum;
using libMasterLibaryApi.Helpers;
using libMasterLibaryApi.Interface;
using SvcEmail.Models;
using System.Net;
using System.Security.Claims;
using System.Text.Json;
using System.Xml.Linq;
using SvcEmail.Interface;

namespace SvcEmail.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ResetPasswordEmailController : ControllerBase
    {
        private readonly IEmail _email;
        private readonly ILogger<ResetPasswordEmailController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IJwtToken _jwtToken;


        public ResetPasswordEmailController(ILogger<ResetPasswordEmailController> logger, IConfiguration configuration, IEmail email
                                    , IWebHostEnvironment webHostEnvironment, IJwtToken jwtToken)
        {
            _logger = logger;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
            _jwtToken = jwtToken;
            _email = email;
        }

        [HttpGet(Name = "ResetPasswordEmail")]
        public async Task<IActionResult> Get([FromBody] EmailValidate value)
        {
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] EmailValidate value) 
        {

            Email model = new Email();
            model.SenderDetails = new EmailData { Email = _configuration["EmailSetting:emailFrom"], Name = _configuration["EmailSetting:nameFrom"] };
            model.EmailTo = new EmailData { Email = value.UserEmail, Name = value.Name };
            model.Subject = "[Epsilon Sigma] Change password for Epsilon Sigma";

            string urlNavigate = _configuration["RootURL:ResetPassword"];
            string jsonFormatter = JsonSerializer.Serialize<EmailValidate>(value);
            Claim[] claims = new[]
            {
                new Claim("token", jsonFormatter)
            };

            string token = _jwtToken.GenerateJWTToken(claims, 1);

            urlNavigate += "?token=" + token;


            string webRootPath = _webHostEnvironment.WebRootPath;
            string contentRootPath = _webHostEnvironment.ContentRootPath;
            string path = "";
            path = Path.Combine(webRootPath, "Templates", "ResetPassword.html");

            model.Body = System.IO.File.ReadAllText(path);
            model.Body = model.Body.Replace("[VerifyURLData]", urlNavigate);
            model.Body = model.Body.Replace("[EmailValue]", value.UserEmail);

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