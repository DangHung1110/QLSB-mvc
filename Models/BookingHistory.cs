using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Dotnet_OngPhuong.Models;

namespace Dotnet_OngPhuong.Models
{
    public class BookingHistory
    {
        [Key]
        public int BHId { get; set; }
        public int? BookingId { get; set; }
        [ForeignKey("BookingId")]
        public Booking Booking { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
        public int FieldId { get; set; }
        [ForeignKey("FieldId")]
        public Field Field { get; set; }
        public DateTime ActionDate { get; set; }
        public string ActionType { get; set; }
        public string Details { get; set; }
    }

}