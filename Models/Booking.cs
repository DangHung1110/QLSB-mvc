using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dotnet_OngPhuong.Models
{
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public int FieldId { get; set; }
        [ForeignKey("FieldId")]
        public Field Field { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }

        public bool IsConfirmed { get; set; } = false;
        public bool IsPaid { get; set; } = false;

        public BookingStatus Status { get; set; } = BookingStatus.Active;
    }
}