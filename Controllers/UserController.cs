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

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
                return BadRequest("Username already exists");

            var user = new User
            {
                Username = dto.Username,
                Password = dto.Password, // Plaintext for now
                Role = dto.Role
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("Registration successful");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.Username == dto.Username && u.Password == dto.Password);

            if (user == null)
                return Unauthorized("Invalid credentials");

            var token = _jwtService.GenerateToken(user.Id, user.Username, user.Role);

            return Ok(new
            {
                token,
                role = user.Role
            });
        }
    }
}