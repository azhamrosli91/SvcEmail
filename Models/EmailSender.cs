namespace SvcEmail.Models
{
    public class EmailSender
    {
        public string emailTo { get; set; }
        public List<string> emailCc { get; set; }
        public List<string> emailBcc { get; set; }
        public string subject { get; set; }
    }
}