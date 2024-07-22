using System.Text.Json.Serialization;

namespace WebApiXd.Models
{
    public class Cart
    {
        public int CartId { get; set; }

        public int BuyerId { get; set; }
        [JsonIgnore]
        public User Buyer { get; set; } 
        public ICollection<CartItem> CartItems { get; set; } 
    }
    public class CartItem
    {
        public int CartItemId { get; set; }
        public int CartId { get; set; }
        [JsonIgnore]
        public Cart Cart { get; set; } 

        public int BookId { get; set; }
        [JsonIgnore]
        public Book Book { get; set; } 
        public int Quantity { get; set; }
    }
    public class CartItemDto
    {
        public int BookId { get; set; }
        public int Quantity { get; set; }
    }
}
