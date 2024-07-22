using Microsoft.EntityFrameworkCore;
using WebApiXd.Models;


namespace WebApiXd.Data
{

    public interface IAppDbContext
    {
        User GetUser(string username, string password);
        void AddUser(User user);
    }

    public class AppDbContext : DbContext, IAppDbContext
    {
        private readonly IConfiguration _configuration;

        public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");

                optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString),
                    options => options.EnableRetryOnFailure());
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Book>()
            .Property(b => b.Price)
            .HasColumnType("decimal(16, 2)"); // 16 toplam basamak, 2 ondalık basamak

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Book)
                .WithMany()
                .HasForeignKey(oi => oi.BookId);

            // UserName kolonunu benzersiz yap
            modelBuilder.Entity<User>()
                .HasIndex(b => b.UserName)
                .IsUnique();

            // User-Cart ilişkisi
            modelBuilder.Entity<Cart>()
                .HasOne(c => c.Buyer)
                .WithMany(u => u.Carts)
                .HasForeignKey(c => c.BuyerId);

            // Cart-CartItem ilişkisi
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.CartItems)
                .HasForeignKey(ci => ci.CartId);

            // CartItem-Book ilişkisi
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Book)
                .WithMany()
                .HasForeignKey(ci => ci.BookId);

            // Book-User ilişkisi
            modelBuilder.Entity<Book>()
                .HasOne(b => b.Seller)
                .WithMany(u => u.Books)
                .HasForeignKey(b => b.SellerId)
                .OnDelete(DeleteBehavior.Cascade); // Or your desired delete behavior
        }


        private readonly List<User> _users = new();

        public User GetUser(string username, string password)
        {
            return _users.SingleOrDefault(u => u.UserName == username && u.Password == password);
        }

        public void AddUser(User user)
        {
            _users.Add(user);
        }

    }

}
