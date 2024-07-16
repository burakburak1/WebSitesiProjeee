namespace WebApiXd.Models
{
    public class Cart
    {
        public int CartId { get; set; }

        public int BuyerId { get; set; }

        public User Buyer { get; set; } // Navigation property for User
        public ICollection<CartItem> CartItems { get; set; } // Sepet öğeleri
    }
}
