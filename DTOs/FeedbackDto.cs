namespace SmartFeedbackPortal.API.DTOs
{
    public class FeedbackDto
    {
        public string Content { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;  // ✅ Required
    }
}
