using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebApiXd.Data;
using WebApiXd.Models;

namespace WebApiXd.Services
{
    public class CartService : ICartService
    {
        private readonly AppDbContext _context;

        public CartService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Cart> GetCartByUserId(int userId)
        {
            return await _context.Carts.Include(c => c.CartItems)
                                       .ThenInclude(ci => ci.Book)
                                       .FirstOrDefaultAsync(c => c.BuyerId == userId);
        }

        public async Task<CartItem> AddCartItem(int userId, CartItemDto cartItemDto)
        {
            var cart = await GetCartByUserId(userId);
            if (cart == null)
            {
                cart = new Cart { BuyerId = userId };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            var cartItem = new CartItem
            {
                CartId = cart.CartId,
                BookId = cartItemDto.BookId,
                Quantity = cartItemDto.Quantity
            };

            _context.CartItems.Add(cartItem);
            await _context.SaveChangesAsync();

            return cartItem;
        }

        public async Task<CartItem> UpdateCartItem(int userId, int cartItemId, CartItemDto cartItemDto)
        {
            var cart = await GetCartByUserId(userId);
            if (cart == null) return null;

            var cartItem = await _context.CartItems.FirstOrDefaultAsync(ci => ci.CartItemId == cartItemId && ci.CartId == cart.CartId);
            if (cartItem == null) return null;

            cartItem.BookId = cartItemDto.BookId;
            cartItem.Quantity = cartItemDto.Quantity;

            _context.CartItems.Update(cartItem);
            await _context.SaveChangesAsync();

            return cartItem;
        }

        public async Task<bool> RemoveCartItem(int userId, int cartItemId)
        {
            var cart = await GetCartByUserId(userId);
            if (cart == null) return false;

            var cartItem = await _context.CartItems.FirstOrDefaultAsync(ci => ci.CartItemId == cartItemId && ci.CartId == cart.CartId);
            if (cartItem == null) return false;

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
