namespace WebApiXd.Models
{
    public class CartItem
    {
        public int CartItemId { get; set; }
        public int CartId { get; set; }
        public Cart Cart { get; set; } // Navigation property for Cart

        public int BookId { get; set; }
        public Book Book { get; set; } // Navigation property for Book
        public int Quantity { get; set; }
    }
}
