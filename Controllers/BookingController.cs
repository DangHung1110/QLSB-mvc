using Dotnet_OngPhuong.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Dotnet_OngPhuong.Models;
using Dotnet_OngPhuong.Filters;

namespace Dotnet_OngPhuong.Controllers
{
    [AuthFilter]
    public class BookingController : Controller
    {
        private readonly AppDBContext _context;

        public BookingController(AppDBContext context)
        {
            _context = context;
        }

        private int GetLoggedInUserId()
        {
            var idString = HttpContext.Session.GetString("ID");
            if (string.IsNullOrEmpty(idString)) return -1;

            return int.Parse(idString);
        }
        public IActionResult Create(int id, DateTime? date)
        {
            var selectedDate = date ?? DateTime.Today;
            var field = _context.Fields.Include(f => f.Bookings).FirstOrDefault(f => f.IDField == id);

            var slots = Enumerable.Range(6, 13).Select(hour =>
            {
                var start = new TimeSpan(hour, 0, 0);
                var booked = field?.Bookings.Any(b =>
                    b.StartTime.Date == selectedDate.Date &&
                    b.StartTime.TimeOfDay == start &&
                    b.Status != BookingStatus.Cancelled ) ?? false;

                return new TimeSlot
                {
                    StartTime = start,
                    IsBooked = booked
                };
            }).ToList();

            if (field == null)
            {
                return NotFound("Field not found.");
            }

            var viewModel = new BookingTimeSlotViewModel
            {
                IDField = field.IDField,
                FieldName = field.FieldName,
                Date = selectedDate,
                TimeSlots = slots
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult ConfirmBooking(int FieldId, DateTime Date, int StartHour)
        {
            var userId = GetLoggedInUserId();
            var startTime = Date.Date.AddHours(StartHour);
            var endTime = startTime.AddHours(1);
            var now = DateTime.Now;

            if (startTime <= now)
            {
                TempData["Error"] = "Không thể đặt sân trong quá khứ.";
                return RedirectToAction("Create", new { id = FieldId, date = Date.ToString("yyyy-MM-dd") });
            }

            var isBooked = _context.Bookings.Any(b => b.FieldId == FieldId &&
                                                 b.StartTime == startTime &&
                                                 b.Status != BookingStatus.Cancelled);
            if (isBooked)
            {
                TempData["Error"] = "Sân đã được đặt. Vui lòng chọn thời gian khác.";
                return RedirectToAction("Create", new { id = FieldId, date = Date.ToString("yyyy-MM-dd") });
            }

            var booking = new Booking
            {
                FieldId = FieldId,
                UserId = userId,
                StartTime = startTime,
                EndTime = endTime,
                IsConfirmed = true,
                IsPaid = false,
                Status = BookingStatus.Active
            };

            _context.Bookings.Add(booking);
            _context.SaveChanges();


            TempData["Success"] = $"Đặt sân thành công: {booking.StartTime:HH:mm dd/MM/yyyy}";
            return RedirectToAction("Index", "User");
        }


        [HttpPost]
        public IActionResult CancelBooking(int bookingId)
        {
            var userId = int.Parse(HttpContext.Session.GetString("ID"));

            var booking = _context.Bookings
                .Include(b => b.Field)
                .FirstOrDefault(b => b.BookingId == bookingId && b.UserId == userId);

            if (booking == null)
            {
                TempData["Error"] = "Không tìm thấy lượt đặt sân.";
                return RedirectToAction("Index", "User");
            }

            // Kiểm tra xem booking có phải đã hủy hay đã hoàn thành chưa
            if (booking.Status == BookingStatus.Cancelled || booking.Status == BookingStatus.Finished)
            {
                TempData["Error"] = "Lượt đặt sân này đã bị hủy hoặc đã hoàn thành.";
                return RedirectToAction("Index", "User");
            }

            // Kiểm tra nếu đặt sân đã diễn ra hoặc đang diễn ra, không thể hủy
            if (booking.StartTime <= DateTime.Now)
            {
                TempData["Error"] = "Không thể hủy lượt đặt sân đã diễn ra hoặc đang diễn ra.";
                return RedirectToAction("Index", "User");
            }

            // Cập nhật trạng thái booking thành hủy
            booking.Status = BookingStatus.Cancelled;
            _context.SaveChanges();

            // Lưu vào lịch sử với chỉ hành động "Cancelled"
            var history = new BookingHistory
            {
                BookingId = booking.BookingId,
                UserId = booking.UserId,
                FieldId = booking.FieldId,
                ActionDate = DateTime.Now,
                ActionType = "Cancelled",
                Details = $"Người dùng hủy đặt sân {booking.Field.FieldName} vào lúc {booking.StartTime:HH:mm dd/MM/yyyy}"
            };

            _context.BookingHistories.Add(history);
            _context.SaveChanges();

            TempData["Success"] = "Hủy đặt sân thành công.";
            return RedirectToAction("Index", "User");
        }

    }
}