using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WebApiXd.Models;
using WebApiXd.Services;

namespace WebApiXd.Models
{
    public class CartItemDto
    {
        public int BookId { get; set; }
        public int Quantity { get; set; }
    }
}
