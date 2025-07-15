namespace SmartFeedbackPortal.API.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public string Role { get; set; } = "User";

        // Navigation property
        public List<Feedback> Feedbacks { get; set; } = new();
    }
}
