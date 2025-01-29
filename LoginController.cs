using Microsoft.AspNetCore.Mvc;

namespace AuthApp.Controllers
{
    public class LoginController : Controller
    {
        // GET /login (рендеринг страницы авторизации)
        [HttpGet("login")]
        public IActionResult LoginForm()
        {
            return View("Login");
        }

        // POST /login (обработка авторизации)
        [HttpPost("login")]
        public IActionResult Login([FromForm] string username, [FromForm] string password)
        {
            if (username == "admin" && password == "password")
            {
                return Redirect("/dashboard"); // Перенаправление на dashboard
            }

            ViewData["ErrorMessage"] = "Неверное имя пользователя или пароль";
            return View("Login"); // Возврат на страницу с ошибкой
        }

        // GET /dashboard (проверка авторизации)
        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
            return View("Dashboard");
        }
    }
}
