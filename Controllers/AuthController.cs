using Dotnet_OngPhuong.Data;
using Dotnet_OngPhuong.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dotnet_OngPhuong.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDBContext _context;

        public AuthController(AppDBContext context)
        {
            _context = context;
        }

        // GET: /Auth/Login
        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu";
                return View();
            }

            // Trim inputs to prevent whitespace issues
            username = username.Trim();
            password = password.Trim();

            var user = _context.Users.FirstOrDefault(u => u.UserName == username);
            
            if (user == null)
            {
                ViewBag.Error = "Sai tài khoản hoặc mật khẩu";
                return View();
            }

            if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                ViewBag.Error = "Sai tài khoản hoặc mật khẩu";
                return View();
            }

            HttpContext.Session.SetString("ID", user.UserId.ToString());
            HttpContext.Session.SetString("Role", user.Role);

            if(user.Role == "User")
                return RedirectToAction("Index", "User");

            if (user.Role == "Admin")
                return RedirectToAction("Index", "Admin");
             
            return RedirectToAction("Index", "Home");
        }

        // GET: /Auth/Register
        public IActionResult Register() => View();

        [HttpPost]
        public IActionResult Register(User user)
        {
            if (!ModelState.IsValid)
    {
        // Log hoặc xem các lỗi cụ thể
        foreach (var state in ModelState)
        {
            foreach (var error in state.Value.Errors)
            {
                Console.WriteLine($"Field: {state.Key}, Error: {error.ErrorMessage}");
            }
        }
        return View(user);
    }

            // Kiểm tra tên người dùng đã tồn tại
            var exists = _context.Users.Any(u => u.UserName == user.UserName);
            if (exists)
            {
                ViewBag.Error = "Tên người dùng đã tồn tại";
                return View(user);
            }

            user.Role = "User"; // Gán mặc định vai trò là User

            // Mã hóa mật khẩu bằng BCrypt
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            _context.Users.Add(user);
            _context.SaveChanges();

            return RedirectToAction("Login");
        }


        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        
    }
}
