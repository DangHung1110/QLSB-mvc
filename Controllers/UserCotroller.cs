using Dotnet_OngPhuong.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Dotnet_OngPhuong.Models;
using Dotnet_OngPhuong.Filters;

namespace Dotnet_OngPhuong.Controllers
{
    [AuthFilter]
    public class UserController : Controller
    {
        private readonly AppDBContext _context;

        public UserController(AppDBContext context)
        {
            _context = context;
        }

        private int GetLoggedInUserId()
        {
            var idString = HttpContext.Session.GetString("ID");
            if (string.IsNullOrEmpty(idString)) return -1;

            return int.Parse(idString);
        }

        public IActionResult Index()
        {
            var today = DateTime.Today;

            var fields = _context.Fields
                .Include(f => f.Bookings)
                .Select(f => new
                {
                    f.IDField,
                    f.FieldName,
                    f.PricePerHour,
                    f.Description,
                    IsBooked = f.Bookings.Any(b => b.StartTime.Date == today)
                })
                .ToList();

            var fieldList = fields.Select(f => new Field
            {
                IDField = f.IDField,
                FieldName = f.FieldName,
                PricePerHour = f.PricePerHour,
                Description = f.Description,
            }).ToList();

            // Lấy lịch sử đặt sân của user
            var userIdStr = HttpContext.Session.GetString("ID");
            var bookingList = new List<Booking>();

            if (!string.IsNullOrEmpty(userIdStr))
            {
                int userId = int.Parse(userIdStr);
                bookingList = _context.Bookings
                    .Include(b => b.Field)
                    .Where(b => b.UserId == userId)
                    .OrderByDescending(b => b.StartTime)
                    .ToList();
            }

            ViewBag.Bookings = bookingList;

            return View("Index", fieldList);
        }

        public IActionResult BookingHistory()
        {
            var userId = GetLoggedInUserId();

            var history = _context.BookingHistories
                .Include(h => h.Field)
                .Include(h => h.Booking)
                .Where(h => h.UserId == userId &&
                            (h.Booking.Status == BookingStatus.Cancelled || h.Booking.Status == BookingStatus.Finished))
                .OrderByDescending(h => h.ActionDate)
                .ToList();

            Console.WriteLine(history);
            
            return View("History", history);           
        }
    }
}