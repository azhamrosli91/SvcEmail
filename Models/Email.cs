using System.ComponentModel.DataAnnotations;

namespace SvcEmail.Models
{
    public class Email {

        [Required]
        public EmailData SenderDetails { get; set; }
        [Required]
        public EmailData EmailTo { get; set; }
        public List<EmailData> EmailCc { get; set; }
        public List<EmailData> EmailBcc { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

    }
}