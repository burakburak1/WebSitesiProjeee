using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiXd.Data;
using WebApiXd.Models;
using WebApiXd.Services;

namespace WebApiXd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IJwtService _jwtService;
         public AuthController(AppDbContext context, IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }



        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto userDto)
        {

            var validRoles = new List<string> { "Buyer", "Seller" };

            if (!validRoles.Contains(userDto.Role))
            {
                return BadRequest("Invalid role.Only 'Buyer' or 'Seller' roles are allowed.");
            }

            if (await _context.Users.AnyAsync(b => b.UserName == userDto.UserName))
            {
                return BadRequest("A user with the same username already exists.");
            }

            var user = new User
            {
                UserName = userDto.UserName,
                Password = userDto.Password,
                Role = userDto.Role,
            };

            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User registered successfully" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto userDto)
        {

            var user = new User
            {
                UserName = userDto.UserName,
                Password = userDto.Password,
            };

            var dbUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == user.UserName);
            if (dbUser == null || !BCrypt.Net.BCrypt.Verify(user.Password, dbUser.Password))
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }

            var token = _jwtService.GenerateToken(dbUser);
            return Ok(new { token });
        }
    }
}
