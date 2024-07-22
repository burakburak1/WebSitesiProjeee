using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiXd.Data;
using WebApiXd.Models;

[Route("api/[controller]")]
[ApiController]
public class BooksController : ControllerBase
{
    private readonly AppDbContext _context;

    public BooksController(AppDbContext context)
    {
        _context = context;
    }

    [Authorize]
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<Book>>> Search([FromQuery] string Genre)
    {
        if (string.IsNullOrEmpty(Genre))
        {
            return BadRequest("Genre must be provided");
        }

        var books = await _context.Books
        .Where(b => b.Genre.ToLower() == Genre.ToLower())
        .ToListAsync();


        if (books == null || !books.Any())
        {
            return NotFound("No books found for the specified genre");
        }

        return Ok(books);
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IQueryable<Book>>> GetBooks()
    {

        var books = await (from b in _context.Books
                           select new Book()
                           {
                               BookId = b.BookId,
                               Title = b.Title,
                               Author = b.Author,
                               Genre = b.Genre,
                               Price = b.Price,
                               Stock = b.Stock,
                               SellerId = b.SellerId,
                           }).ToListAsync();

        return Ok(books);

    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<Book>> GetBook(int id)
    {
        var book = await _context.Books.FindAsync(id);

        if (book == null)
        {
            return NotFound();
        }

        return book;
    }

    [Authorize(Roles = "Admin,Seller")]
    [HttpPost]
    public async Task<ActionResult<Book>> PostBook(BookPostDto bookDto)
    {
        // JWT token'dan kullanıcı kimliğini al
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized("User ID not found in token.");
        }

        var userId = int.Parse(userIdClaim);

        // Kullanıcı kimliğini kitap sellerId olarak ayarla

        var book = new Book
        {
            Title = bookDto.Title,
            Author = bookDto.Author,
            Genre = bookDto.Genre,
            Description = bookDto.Description,
            Price = bookDto.Price,
            Stock = bookDto.Stock,
            CreatedAt = bookDto.CreatedAt,
            SellerId = userId // SellerId'yi JWT token'dan alınan userId ile ayarlama
        };

        // Kullanıcının var olduğunu doğrulama
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);

        if (user == null)
        {
            return NotFound("User not found.");
        }

        _context.Books.Add(book);

        await _context.SaveChangesAsync();

        return CreatedAtAction("GetBook", new { id = book.BookId }, book);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> PutBook(int id, BookUpdateDto bookDto)
    {
        var book = await _context.Books.FindAsync(id);
        if (book == null)
        {
            return NotFound();
        }

        book.Title = bookDto.Title;
        book.Author = bookDto.Author;
        book.Genre = bookDto.Genre;
        book.Price = bookDto.Price;
        book.Stock = bookDto.Stock;
        book.UpdatedAt = bookDto.UpdatedAt;

        _context.Entry(book).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (! _context.Books.Any(e => e.BookId == id))
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


    [Authorize(Roles = "Admin")] 
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBook(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book == null)
        {
            return NotFound();
        }

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();

        return NoContent();
    }


}