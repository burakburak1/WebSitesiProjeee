using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApiXd.Data;
using WebApiXd.Services;

namespace WebApiXd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProfileController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet()]
        public async Task<IActionResult> GetProfile()
        {
            // Oturum açmış kullanıcının UserId'sini al
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            // Kullanıcının bilgilerini al
            var user = await _context.Users
                .Include(u => u.Books) // Kullanıcının kitaplarını da dahil et
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                return NotFound("Invalid user ID.");
            }

            // Kullanıcının kitaplarını getir
            var books = await _context.Books
                .Where(b => b.SellerId == userId)
                .ToListAsync();

            return Ok(new
            {
                User = user,
                Books = books
            });
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdateProfile(AppDbContext context, ProfileUpdateDto profileDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _context.Users.FindAsync(int.Parse(userId));

            if (user == null)
            {
                return NotFound("User not found.");
            }

            user.UserName = profileDto.UserName;
            if (!string.IsNullOrEmpty(profileDto.Password))
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(profileDto.Password);
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok(user);
        }

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> DeleteProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _context.Users.FindAsync(int.Parse(userId));
            if (user == null)
            {
                return NotFound("User not found.");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User deleted successfully" });
        }
    }
}

