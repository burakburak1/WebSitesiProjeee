using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApiXd.Data;
using WebApiXd.Models;

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

            return Ok(user);
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdateProfile(AppDbContext context, ProfileUpdateDto profileDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _context.Users
                .Include(u => u.Books)
                .FirstOrDefaultAsync(u => u.UserId == int.Parse(userId));

            if (user == null)
            {
                return NotFound("User not found.");
            }

            user.UserName = profileDto.UserName;
            user.Password = BCrypt.Net.BCrypt.HashPassword(profileDto.Password);

            // Mevcut kitapları güncelle
            foreach (var bookDto in profileDto.Books)
            {
                var existingBook = user.Books.FirstOrDefault(b => b.BookId == bookDto.BookId);
                if (existingBook != null)
                {
                    existingBook.Title = bookDto.Title;
                    existingBook.Author = bookDto.Author;
                    existingBook.Genre = bookDto.Genre;
                    existingBook.Description = bookDto.Description;
                    existingBook.Price = bookDto.Price;
                    existingBook.Stock = bookDto.Stock;
                }
                else
                {
                    var newBook = new Book
                    {
                        Title = bookDto.Title,
                        Author = bookDto.Author,
                        Genre = bookDto.Genre,
                        Description = bookDto.Description,
                        Price = bookDto.Price,
                        Stock = bookDto.Stock,
                        SellerId = (int)user.UserId // Satıcı ID'si kullanıcı ID'sine eşitlenir
                    };
                    user.Books.Add(newBook);
                }
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            var result = new
            {
                user.UserId,
                user.UserName,
                Books = user.Books.Select(b => new
                {
                    b.BookId,
                    b.Title,
                    b.Author,
                    b.Genre,
                    b.Description,
                    b.Price,
                    b.Stock
                })
            };

            return Ok(result);
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

