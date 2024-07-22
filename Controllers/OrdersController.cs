using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApiXd.Data;
using WebApiXd.Models;
using WebApiXd.Services;

namespace WebApiXd.Controllers
{

    [Authorize(Roles = "Admin,Buyer")]
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ICartService _cartService;

        public OrdersController(AppDbContext context, ICartService cartService)
        {
            _context = context;
            _cartService = cartService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Book)
                .ToListAsync();

            var orderDtos = orders.Select(o => new OrderDto
            {
                OrderId = o.OrderId,
                TotalAmount = o.OrderItems.Sum(oi => oi.Quantity * oi.Price),
                OrderItems = o.OrderItems.Select(oi => new OrderItemDto
                {
                    BookId = oi.BookId,
                    Title = oi.Book.Title,
                    Quantity = oi.Quantity,
                    Price = oi.Price
                }).ToList()
            }).ToList();

            return Ok(orderDtos);
        }

        [HttpPost]
        public async Task<IActionResult> PostOrder()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var cart = await _cartService.GetCartByUserId(userId);
            if (cart == null || !cart.CartItems.Any())
            {
                return BadRequest("Cart is empty.");
            }

            var orderItems = new List<OrderItem>();
            decimal totalAmount = 0;

            foreach (var cartItem in cart.CartItems)
            {
                var book = await _context.Books.FindAsync(cartItem.BookId);
                if (book == null || book.Stock < cartItem.Quantity)
                {
                    return BadRequest($"Not enough stock for book: {book?.Title ?? cartItem.BookId.ToString()}");
                }

                orderItems.Add(new OrderItem
                {
                    BookId = cartItem.BookId,
                    Quantity = cartItem.Quantity,
                    Price = cartItem.Book.Price
                });

                totalAmount += cartItem.Quantity * cartItem.Book.Price;
                book.Stock -= cartItem.Quantity;
            }

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                TotalAmount = totalAmount,
                OrderItems = orderItems
            };

            _context.Orders.Add(order);
            _context.Carts.Remove(cart); 
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrders), new { id = order.OrderId }, order);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == id && o.UserId == userId);

            if (order == null)
            {
                return NotFound();
            }

            foreach (var orderItem in order.OrderItems)
            {
                var book = await _context.Books.FindAsync(orderItem.BookId);
                if (book != null)
                {
                    book.Stock += orderItem.Quantity;
                }
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

