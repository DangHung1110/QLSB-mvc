using Microsoft.EntityFrameworkCore;
using Dotnet_OngPhuong.Models;
namespace Dotnet_OngPhuong.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Field> Fields { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingHistory> BookingHistories { get; set; }

        public static void SeedData(AppDBContext context)
        {
            if (!context.Users.Any(u => u.UserName == "ChuSan")) // check if the ChuSan user already exists
            {
                var ChuSan = new User
                {
                    UserName = "ChuSan",
                    Password = BCrypt.Net.BCrypt.HashPassword("Muvodich"),
                    Role = "Admin",
                };

                context.Users.Add(ChuSan);
                context.SaveChanges();
            }
        }
    }
}
