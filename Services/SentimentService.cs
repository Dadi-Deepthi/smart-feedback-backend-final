
namespace SmartFeedbackPortal.API.Services
{
    public class SentimentService
    {
        public string Analyze(string content)
        {
            if (content.Contains("good") || content.Contains("great") || content.Contains("excellent"))
                return "Positive";

            if (content.Contains("bad") || content.Contains("worst") || content.Contains("terrible"))
                return "Negative";

            return "Neutral";
        }
    }
}
