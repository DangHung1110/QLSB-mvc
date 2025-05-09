using Dotnet_OngPhuong.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Dotnet_OngPhuong.Models;

namespace Dotnet_OngPhuong.Controllers
{
    public class BookingController : Controller
    {
        private readonly AppDBContext _context;

        public BookingController(AppDBContext context)
        {
            _context = context;
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
                    b.StartTime.TimeOfDay == start) ?? false;

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
public IActionResult Book(int fieldId, DateTime startTime, DateTime endTime)
{
    var isBooked = _context.Bookings.Any(b =>
        b.FieldId == fieldId &&
        b.StartTime < endTime &&
        b.EndTime > startTime
    );

    if (isBooked)
    {
        TempData["Error"] = "Khung giờ này đã có người đặt.";
        return RedirectToAction("Index", "User");
    }

    var userId = int.Parse(HttpContext.Session.GetString("ID"));
    var booking = new Booking
    {
        FieldId = fieldId,
        UserId = userId,
        StartTime = startTime,
        EndTime = endTime,
        IsConfirmed = true // hoặc bỏ luôn, vì đặt xong là xác nhận luôn
    };

    _context.Bookings.Add(booking);
    _context.SaveChanges();

    return RedirectToAction("MyBookings", "User");
}


    }
}