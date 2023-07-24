using libMasterLibaryApi.Helpers;
using libMasterLibaryApi.Interface;
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
    public class VerifyAccEmailController : ControllerBase
    {

        private readonly IEmail _email;
        private readonly ILogger<VerifyAccEmailController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IJwtToken _jwtToken;


        public VerifyAccEmailController(ILogger<VerifyAccEmailController> logger, IConfiguration configuration, IEmail email
                                    , IWebHostEnvironment webHostEnvironment, IJwtToken jwtToken)
        {
            _logger = logger;
            _configuration = configuration;
            _email = email;
            _webHostEnvironment = webHostEnvironment;
            _jwtToken = jwtToken;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {      
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] EmailValidate value) 
        {

            Email model = new Email();
            model.SenderDetails = new EmailData { Email = _configuration["EmailSetting:emailFrom"], Name = _configuration["EmailSetting:nameFrom"] };
            model.EmailTo = new EmailData { Email = value.UserEmail, Name = value.Name };
            model.Subject = "[Epsilon Sigma] Please Verify Your Epsilon Sigma";

            string urlNavigate = _configuration["RootURL:VerifyAccount"];
            string jsonFormatter = JsonSerializer.Serialize<EmailValidate>(value);
            Claim[] claims = new[]
            {
                new Claim("token", jsonFormatter)
            };

            string token = _jwtToken.GenerateJWTToken(claims, 2);

            urlNavigate += "?token=" + token;


            string webRootPath = _webHostEnvironment.WebRootPath;
            string contentRootPath = _webHostEnvironment.ContentRootPath;
            string path = "";
            path = Path.Combine(webRootPath, "Templates", "VerifyAccount.html");

            model.Body = System.IO.File.ReadAllText(path);
            model.Body = model.Body.Replace("[VerifyURLData]", urlNavigate);

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