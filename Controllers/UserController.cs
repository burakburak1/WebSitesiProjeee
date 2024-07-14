using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiXd.Data;
using WebApiXd.Models;
using WebApiXd.Services;
using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;

namespace WebApiXd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IJwtService _jwtService;

        public UsersController(AppDbContext context, IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }



        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto userDto)
        {


            if (await _context.Users.AnyAsync(b => b.UserName == userDto.UserName))
            {
                return BadRequest("A user with the same username already exists.");
            }

            var user = new User
            {
               UserName = userDto.UserName,
               Password = userDto.Password,
               Role = userDto.Role
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
                Password = userDto.Password
            };

            var dbUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == user.UserName);
            if (dbUser == null || !BCrypt.Net.BCrypt.Verify(user.Password, dbUser.Password))
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }

            var token = _jwtService.GenerateToken(dbUser);
            return Ok(new { token });
        }

        [Authorize(Roles ="Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.UserId }, user);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.UserId)
            {
                return BadRequest();
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            _context.Entry(user).State = EntityState.Modified;


            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}
