using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
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
    [HttpGet]
    public async Task<ActionResult<IQueryable<BookGetDto>>> GetBooks()
    {
        //return await _context.Books.ToListAsync();

        var books = await (from b in _context.Books
                           select new BookGetDto()
                           {
                               BookId = b.BookId,
                               Title = b.Title,
                               Author = b.Author,
                               Genre = b.Genre,
                               Price = b.Price,
                               SellerId = b.SellerId
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

        var book = new Book
        {
            Title = bookDto.Title,
            Author = bookDto.Author,
            Genre = bookDto.Genre,
            Description = bookDto.Description,
            Price = bookDto.Price,
            Stock = bookDto.Stock,
            CreatedAt = bookDto.CreatedAt,
            SellerId = bookDto.SellerId
        };

        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetBook", new { id = book.BookId }, book);
    }

    [Authorize(Roles = "Admin,Seller")] //burada seller'ın sadece kendi kitaplarını update edebilmesi gerekiyor.
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
            if (!BookExists(id))
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


    [Authorize(Roles = "Admin,Seller")] //burada seller'ın sadece kendi kitaplarını silebilmesi gerekiyor.
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

    private bool BookExists(int id)
    {
        return _context.Books.Any(e => e.BookId == id);
    }
}