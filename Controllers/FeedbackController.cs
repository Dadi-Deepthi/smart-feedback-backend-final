using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartFeedbackPortal.API.Data;
using SmartFeedbackPortal.API.DTOs;
using SmartFeedbackPortal.API.Models;
using SmartFeedbackPortal.API.Services;
using System.Security.Claims;

namespace SmartFeedbackPortal.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class FeedbackController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly SentimentService _sentiment;

        public FeedbackController(AppDbContext context, SentimentService sentiment)
        {
            _context = context;
            _sentiment = sentiment;
        }

        // ✅ POST: api/Feedback/submit
        [Authorize(Roles = "User")]
        [HttpPost("submit")]
        public IActionResult SubmitFeedback([FromBody] FeedbackDto dto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                    return Unauthorized("Invalid token");

                var userId = int.Parse(userIdClaim.Value);

                var sentiment = _sentiment.Analyze(dto.Content);

                var feedback = new Feedback
                {
                    Content = dto.Content,
                    Sentiment = sentiment,
                    Department = dto.Department,
                    UserId = userId,
                    SubmittedAt = DateTime.UtcNow
                };

                _context.Feedbacks.Add(feedback);
                _context.SaveChanges();

                return Ok(new { message = "Feedback submitted successfully", sentiment });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error submitting feedback: {ex.Message}");
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        // ✅ GET: api/Feedback/all
        [HttpGet("all")]
        public IActionResult GetAllFeedback()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                var roleClaim = User.FindFirst(ClaimTypes.Role);

                if (userIdClaim == null || roleClaim == null)
                    return Unauthorized("Invalid token claims");

                var userId = int.Parse(userIdClaim.Value);
                var role = roleClaim.Value;

                IQueryable<Feedback> query = _context.Feedbacks.Include(f => f.User);

                if (role == "Admin")
                {
                    // ✅ Group feedbacks by department for Admin
                    var groupedFeedback = query
                        .ToList()
                        .GroupBy(f => f.Department)
                        .Select(group => new
                        {
                            Department = group.Key,
                            Feedbacks = group.Select(f => new
                            {
                                f.Id,
                                f.Content,
                                f.Sentiment,
                                f.SubmittedAt,
                                f.Department,
                                Username = f.User != null ? f.User.Username : "Unknown"
                            }).ToList()
                        }).ToList();

                    return Ok(groupedFeedback);
                }
                else
                {
                    // ✅ Normal user: return only their own feedbacks
                    var userFeedbacks = query
                        .Where(f => f.UserId == userId)
                        .Select(f => new
                        {
                            f.Id,
                            f.Content,
                            f.Sentiment,
                            f.SubmittedAt,
                            f.Department,
                            Username = f.User != null ? f.User.Username : "Unknown"
                        })
                        .ToList();

                    return Ok(userFeedbacks);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error retrieving feedback: {ex.Message}");
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
