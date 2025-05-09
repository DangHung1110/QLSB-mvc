using Dotnet_OngPhuong.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

            // Lấy danh sách sân
            var fields = _context.Fields.ToList();

            // Lấy danh sách đặt sân hôm nay
            var todayBookings = _context.Bookings
                .Include(b => b.Field)
                .Where(b => b.StartTime.Date == today)
                .ToList();

            // Tính doanh thu = tổng số giờ * giá mỗi giờ
            decimal totalRevenue = 0;
            foreach (var booking in todayBookings)
            {
                var hours = (decimal)(booking.EndTime - booking.StartTime).TotalHours;
                totalRevenue += hours * booking.Field.PricePerHour;
            }

            ViewBag.TotalFields = fields.Count;
            ViewBag.TodayBookings = todayBookings.Count;
            ViewBag.TodayRevenue = totalRevenue;
            ViewBag.Fields = fields;

            return View();
        }
    }
}
