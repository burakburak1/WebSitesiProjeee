using System.ComponentModel.DataAnnotations;

namespace WebApiXd.Models
{
    public class User
    {
        
        public int? UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }

        // Navigation Property
        public ICollection<Book> Books { get; set; } // Kullanıcının kitapları
        public ICollection<Cart> Carts { get; set; } // Kullanıcının sepetleri

    }
}
