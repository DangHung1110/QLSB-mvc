using Dotnet_OngPhuong.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Dotnet_OngPhuong.Models;
using Dotnet_OngPhuong.Filters;
namespace Dotnet_OngPhuong.Controllers
{
    [AuthFilter]
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

            //  Chỉ tính revenue từ các booking đã thanh toán
            decimal totalRevenue = todayBookings
                .Where(b => b.IsPaid == true) 
                .Sum(b => (decimal)(b.EndTime - b.StartTime).TotalHours * b.Field.PricePerHour);

            ViewBag.TotalFields = fields.Count;
            ViewBag.TodayBookings = todayBookings.Count;
            ViewBag.TodayRevenue = totalRevenue;

            return View(fieldViewModels);
        }

        public IActionResult TodayBookings()
        {
            var today = DateTime.Today;

            var bookings = _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Field)
                .Where(b => b.StartTime.Date == today && b.IsConfirmed && !b.IsPaid && b.Status == BookingStatus.Active)
                .OrderBy(b => b.StartTime)
                .ToList();

            return View(bookings);
        }

        [HttpPost]
        public IActionResult ConfirmPayment(int bookingId)
        {
            var booking = _context.Bookings
                .Include(b => b.Field)
                .Include(b => b.User)
                .FirstOrDefault(b => b.BookingId == bookingId);

            if (booking == null)
            {
                TempData["Error"] = "Không tìm thấy lượt đặt.";
                return RedirectToAction("TodayBookings");
            }

            booking.IsPaid = true;
            booking.Status = BookingStatus.Finished;

            // Ghi vào lịch sử trước khi xóa
            var history = new BookingHistory
            {
                BookingId = booking.BookingId,
                UserId = booking.UserId,
                FieldId = booking.FieldId,
                ActionDate = DateTime.Now,
                ActionType = "Confirmed",
                Details = $"Admin xác nhận thanh toán cho sân {booking.Field.FieldName} - {booking.StartTime:HH:mm dd/MM/yyyy}"
            };

            _context.BookingHistories.Add(history);
            _context.SaveChanges();

            TempData["Success"] = "Đã xác nhận thanh toán.";
            return RedirectToAction("TodayBookings");
        }
    }
}
