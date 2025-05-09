using System.ComponentModel.DataAnnotations;
namespace Dotnet_OngPhuong.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        public string UserName { get; set; }    
        public string Password { get; set; }
        public string Role { get; set; } = "User"; 
        public List<Booking>? Bookings { get; set; }
    }
}
