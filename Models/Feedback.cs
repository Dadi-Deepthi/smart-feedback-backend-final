namespace SmartFeedbackPortal.API.Models
{
    public class Feedback
    {
        public int Id { get; set; }
        public string Content { get; set; } = "";
        public string Sentiment { get; set; } = "";
        public DateTime SubmittedAt { get; set; }

        // Foreign Key
        public int UserId { get; set; }
        public User? User { get; set; }
    }
}
