namespace Dotnet_OngPhuong.Models
{
    public class TimeSlot
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime => StartTime.Add(TimeSpan.FromHours(1));
        public bool IsBooked { get; set; }
    }
}