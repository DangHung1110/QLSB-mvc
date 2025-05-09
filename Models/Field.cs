using System.ComponentModel.DataAnnotations;
namespace Dotnet_OngPhuong.Models
{
    public class Field
    {
        [Key]
        public int IDField { get; set; }

        [Required(ErrorMessage = "Tên sân là bắt buộc")]
        public string FieldName { get; set; }

        [Required(ErrorMessage = "Giá thuê là bắt buộc")]
        [Range(1, double.MaxValue, ErrorMessage = "Giá thuê phải lớn hơn 0")]
        public decimal PricePerHour { get; set; }

        public string Description { get; set; }

        public string Status { get; set; }  = "Free"; 

        public List<Booking> Bookings { get; set; } = new List<Booking>(); // Khởi tạo danh sách Bookings để tránh NullReferenceException
    }
}
