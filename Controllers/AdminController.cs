using Dotnet_OngPhuong.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Dotnet_OngPhuong.Models;
namespace Dotnet_OngPhuong.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDBContext _context;

        public AdminController(AppDBContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var today = DateTime.Today;
            var fields = _context.Fields.ToList();
            var todayBookings = _context.Bookings
                .Where(b => b.StartTime.Date == today)
                .ToList();

            var fieldViewModels = fields.Select(field =>
            {
                var timeSlots = new List<TimeSlot>();

                for (int hour = 7; hour < 19; hour++)
                {
                    var slotStart = new TimeSpan(hour, 0, 0);
                    var isBooked = todayBookings.Any(b =>
                        b.FieldId == field.IDField &&
                        b.StartTime.TimeOfDay <= slotStart &&
                        b.EndTime.TimeOfDay > slotStart);

                    timeSlots.Add(new TimeSlot
                    {
                        StartTime = slotStart,
                        IsBooked = isBooked
                    });
                }

                return new BookingTimeSlotViewModel
                {
                    IDField = field.IDField,
                    FieldName = field.FieldName,
                    PricePerHour = field.PricePerHour,
                    Description = field.Description,
                    Date = today,
                    TimeSlots = timeSlots
                };
            }).ToList();

            // Thống kê
            decimal totalRevenue = todayBookings.Sum(b =>
                (decimal)(b.EndTime - b.StartTime).TotalHours * b.Field.PricePerHour);

            ViewBag.TotalFields = fields.Count;
            ViewBag.TodayBookings = todayBookings.Count;
            ViewBag.TodayRevenue = totalRevenue;

            return View(fieldViewModels);
        }

    }
}
