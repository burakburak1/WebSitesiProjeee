using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApiXd.Data;
using WebApiXd.Models;
using WebApiXd.Services;

namespace WebApiXd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ICartService _cartService;

        public CartController(AppDbContext context, ICartService cartService)
        {
            _context = context;
            _cartService = cartService;
        }

        [Authorize(Roles = "Admin,Buyer")]
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var cart = await _cartService.GetCartByUserId(userId);
            if (cart == null)
            {
                return NotFound("Cart not found");
            }
            return Ok(cart);
        }

        [Authorize(Roles = "Admin,Buyer")]
        [HttpPost]
        public async Task<IActionResult> AddCartItem(CartItemDto cartItemDto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var result = await _cartService.AddCartItem(userId, cartItemDto);
            if (result == null)
            {
                return BadRequest("Failed to add item to cart");
            }
            return Ok(result);
        }

        [Authorize(Roles = "Admin,Buyer")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCartItem(int id, CartItemDto cartItemDto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var result = await _cartService.UpdateCartItem(userId, id, cartItemDto);
            if (result == null)
            {
                return NotFound("Cart item not found or not authorized");
            }
            return Ok(result);
        }

        [Authorize(Roles = "Admin,Buyer")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveCartItem(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var result = await _cartService.RemoveCartItem(userId, id);
            if (!result)
            {
                return NotFound("Cart item not found or not authorized");
            }
            return NoContent();
        }
    }
}

