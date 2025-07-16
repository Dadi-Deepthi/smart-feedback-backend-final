namespace SmartFeedbackPortal.API.Models
{
    public class Feedback
{
    public int Id { get; set; }
    public string Content { get; set; }
    public string Sentiment { get; set; }

    public string Department { get; set; } = string.Empty; // ✅ Add this

    public DateTime SubmittedAt { get; set; }

    // Foreign key
    public int UserId { get; set; }

    // Navigation property
    public User? User { get; set; }
}

}
