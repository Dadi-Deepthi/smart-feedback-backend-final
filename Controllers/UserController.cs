using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartFeedbackPortal.API.Data;
using SmartFeedbackPortal.API.DTOs;
using SmartFeedbackPortal.API.Models;
using SmartFeedbackPortal.API.Services;
using System;
using BCrypt.Net;


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

                // 🔐 Hash the password before saving
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

                var user = new User
                {
                    Username = dto.Username,
                    Password = hashedPassword,
                    Role = dto.Role ?? "User"
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return Ok(new { message = "User registered successfully" });
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
                    .FirstOrDefaultAsync(u => u.Username == dto.Username);

                if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
                {
                    return Unauthorized("Invalid credentials");
                }

                var token = _jwtService.GenerateToken(user.Id, user.Username, user.Role);

                return Ok(new
                {
                    token,
                    role = user.Role,
                    username = user.Username
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
