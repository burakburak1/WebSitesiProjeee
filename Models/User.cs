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
    public class UserDto
    {
        public int? UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }

        public ICollection<Book>? Books { get; set; }
        public Cart? Cart { get; set; }
    }
    public class UserRegisterDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }

    }
    public class UserLoginDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
    public class ProfileUpdateDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }


        public LinkedList<BookGetDto> Books { get; set; }

    }
}
