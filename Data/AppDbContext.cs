using Microsoft.EntityFrameworkCore;
using WebApiXd.Models;


namespace WebApiXd.Data
{

    public interface IAppDbContext
    {
        User GetUser(string username, string password);
        void AddUser(User user);
    }

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Book> Books { get; set; }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // UserName kolonunu benzersiz yap
            modelBuilder.Entity<User>()
                .HasIndex(b => b.UserName)
                .IsUnique();
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
