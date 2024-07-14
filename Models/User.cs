using System.ComponentModel.DataAnnotations;

namespace WebApiXd.Models
{
    public class User
    {
        
        public int UserId { get; set; }

        [Required]
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }

    }
}
