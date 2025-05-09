namespace Dotnet_OngPhuong.Models
{
    public class BookingTimeSlotViewModel
    {
        public int IDField { get; set; }
        public string FieldName { get; set; }
        public decimal PricePerHour { get; set; }
        public string Description { get; set; }

        public DateTime Date { get; set; }
        public List<TimeSlot> TimeSlots { get; set; }
    }
}