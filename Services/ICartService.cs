using System.Threading.Tasks;
using WebApiXd.Models;

namespace WebApiXd.Services
{
    public interface ICartService
    {
        Task<Cart> GetCartByUserId(int userId);
        Task<CartItem> AddCartItem(int userId, CartItemDto cartItemDto);
        Task<CartItem> UpdateCartItem(int userId, int cartItemId, CartItemDto cartItemDto);
        Task<bool> RemoveCartItem(int userId, int cartItemId);
    }
}
