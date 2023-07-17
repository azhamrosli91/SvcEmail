namespace SvcEmail.Models
{
    public class EmailInvitationFriend
    {
        public int UserID { get; set; }
        public string Name { get; set; }
        public string UserEmail { get; set; }
        public string InvitationName { get; set; }
        public string CompanyName { get; set; }
        public DateTime DateTimeExp { get; set; }
    }
}