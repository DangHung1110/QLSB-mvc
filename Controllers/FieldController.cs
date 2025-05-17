using Dotnet_OngPhuong.Data;
using Dotnet_OngPhuong.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Dotnet_OngPhuong.Filters;

namespace Dotnet_OngPhuong.Controllers
{
    [AuthFilter]
    public class FieldController : Controller
    {
        private readonly AppDBContext _context;

        public FieldController(AppDBContext context)
        {
            _context = context;
        }

        // GET: Field/Create
        public IActionResult Create()
        {
            return View("CreateField");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("FieldName,PricePerHour,Description")] Field field)
        {
            if (string.IsNullOrEmpty(field.FieldName))
            {
                ModelState.AddModelError("FieldName", "Vui lòng nhập tên sân");
            }

            if (field.PricePerHour <= 0)
            {
                ModelState.AddModelError("PricePerHour", "Giá thuê phải lớn hơn 0");
            }

            // Check for duplicate field name
            if (_context.Fields.Any(f => f.FieldName == field.FieldName))
            {
                ModelState.AddModelError("FieldName", "Tên sân đã tồn tại");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Fields.Add(field);
                    _context.SaveChanges();
                    TempData["Success"] = "Thêm sân thành công";
                    return RedirectToAction("Index", "Admin");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra khi thêm sân. Vui lòng thử lại.");
                    Console.WriteLine($"Error creating field: {ex.Message}");
                }
            }
            return View("CreateField", field);
        }

        // GET: Field/Edit/5
        public IActionResult Edit(int id)
        {
            var field = _context.Fields.Find(id);
            if (field == null)
            {
                return NotFound();
            }
            return View("EditField", field);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("IDField,FieldName,PricePerHour,Description")] Field updatedField)
        {
            if (id != updatedField.IDField)
            {
                return NotFound();
            }

            // Check for duplicate field name, excluding current field
            if (_context.Fields.Any(f => f.FieldName == updatedField.FieldName && f.IDField != id))
            {
                ModelState.AddModelError("FieldName", "Tên sân đã tồn tại");
                return View("EditField", updatedField);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var field = _context.Fields.Find(id);
                    if (field == null) return NotFound();

                    field.FieldName = updatedField.FieldName;
                    field.PricePerHour = updatedField.PricePerHour;
                    field.Description = updatedField.Description;

                    _context.Update(field);
                    _context.SaveChanges();
                    TempData["Success"] = "Cập nhật sân thành công";
                    return RedirectToAction("Index", "Admin");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FieldExists(id))
                    {
                        return NotFound();
                    }
                    throw;
                }
            }
            return View("EditField", updatedField);
        }

        // GET: Field/Delete/5
        public IActionResult Delete(int id)
        {
            var field = _context.Fields.Find(id);
            if (field == null)
            {
                return NotFound();
            }
            return View("DeleteField", field);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var field = _context.Fields.Find(id);
            if (field == null)
            {
                return NotFound();
            }

            // Check if field has any bookings using the correct property name
            if (_context.Bookings.Any(b => b.FieldId == id))
            {
                TempData["Error"] = "Không thể xóa sân đang có lịch đặt";
                return RedirectToAction("Index", "Admin");
            }

            try
            {
                _context.Fields.Remove(field);
                _context.SaveChanges();
                TempData["Success"] = "Xóa sân thành công";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Có lỗi xảy ra khi xóa sân";
                Console.WriteLine($"Error deleting field: {ex.Message}");
            }

            return RedirectToAction("Index", "Admin");
        }

        private bool FieldExists(int id)
        {
            return _context.Fields.Any(e => e.IDField == id);
        }
    }
}