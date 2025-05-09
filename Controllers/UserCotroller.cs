using Dotnet_OngPhuong.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Dotnet_OngPhuong.Models;

namespace Dotnet_OngPhuong.Controllers
{
    public class UserController : Controller
    {
        private readonly AppDBContext _context;

        public UserController(AppDBContext context)
        {
            _context = context;
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

            var viewModel = fields.Select(f => new Field
            {
                IDField = f.IDField,
                FieldName = f.FieldName,
                PricePerHour = f.PricePerHour,
                Description = f.Description,
            })
            .ToList();

            return View("Index", viewModel);
        }
    }
}