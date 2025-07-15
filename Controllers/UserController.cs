using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartFeedbackPortal.API.Data;
using SmartFeedbackPortal.API.DTOs;
using SmartFeedbackPortal.API.Models;
using SmartFeedbackPortal.API.Services;

namespace SmartFeedbackPortal.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly JwtService _jwtService;

        public UserController(AppDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        // ✅ POST: api/User/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            try
            {
                if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
                {
                    return BadRequest("Username already exists");
                }

                var user = new User
                {
                    Username = dto.Username,
                    Password = dto.Password, // In production, always hash this!
                    Role = dto.Role
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Registration Error: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // ✅ POST: api/User/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == dto.Username && u.Password == dto.Password);

                if (user == null)
                {
                    return Unauthorized("Invalid credentials");
                }

                var token = _jwtService.GenerateToken(user.Id, user.Username, user.Role);

                return Ok(new
                {
                    token,
                    role = user.Role
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Login Error: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
